using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TowerDefense.Interfaces;

namespace TestTower
{
    public class BoomTank : Tank
    {
        private Random _rng = new Random();
        public Bullet Bullet { get; set; }
        public override string Name { get { return "Mr. Boomy"; } }
        private double _xTarget;
        private double _yTarget;
        private int _range;

        public BoomTank()
            : base(300, 300)
        {
            _xTarget = 400;
            _yTarget = 400;
            _range = 10;
        }
        public override TankUpdate Update(IGameState gameState)
        {
            TankUpdate tankUpdate = new TankUpdate();

            if (gameState.Foes.Any() && gameState.Goals.Any())
            {
                tankUpdate.ShotTarget = LocationProvider.GetLocation(_xTarget, _yTarget);
                ChangeBulletPower(tankUpdate.ShotTarget, gameState);

                var range = GetDistanceFromTank(new GravityEntity { X = _yTarget, Y = _yTarget, Size = new TowerDefense.Interfaces.Size(1, 1) }) + 1;
                if (Bullet.GetReloadTime((int)range) < 1000 || range < 100)
                {
                    tankUpdate.Bullet = Bullet;
                }
                else
                {
                    tankUpdate.Bullet = new Bullet();
                }

                UpdateMovementTarget(tankUpdate, gameState);
            }

            tankUpdate.TankColor = ConvertColorToHexString(Color.CornflowerBlue);

            return tankUpdate;
        }

        private void UpdateMovementTarget(TankUpdate tankUpdate, IGameState gameState)
        {
            double maxFoes = GetFoeCountInRange(_xTarget, _yTarget, gameState, _range) / (_range * Bullet.SplashHeatMultiplier);

            for (int range = 10; range < 200; range += 20)
            {
                for (int i = 0; i < 20; i++)
                {
                    var y = _rng.NextDouble() * gameState.Size.Height;
                    var x = _rng.NextDouble() * gameState.Size.Width;
                    var f = GetFoeCountInRange(x, y, gameState, range);
                    var fd = f / (range * Bullet.SplashHeatMultiplier);
                    if (fd > maxFoes)
                    {
                        maxFoes = f;
                        _xTarget = x;
                        _yTarget = y;
                        _range = range;
                    }
                }
            }

            tankUpdate.MovementTarget = LocationProvider.GetLocation(_xTarget, _yTarget);
        }

        private int GetFoeCountInRange(double xTarget, double yTarget, IGameState gameState, int range)
        {
            var foesInRange = GetFoesInRange(xTarget, yTarget, gameState, range);
            return foesInRange.Count();
        }

        private static IEnumerable<IFoe> GetFoesInRange(double xTarget, double yTarget, IGameState gameState, int range)
        {
            return gameState.Foes.Where(foe => Math.Abs(foe.X - xTarget) < range && Math.Abs(foe.Y - yTarget) < range);
        }

        private void ChangeBulletPower(ILocation target, IGameState gameState)
        {
            var foes = GetFoesInRange(target.X, target.Y, gameState, _range);
            if (foes.Any())
            {
                var damage = foes.Max(foe => foe.Health);
                var range = GetDistanceFromTank(target) + 1;
                Bullet = new Bullet { Damage = damage, Freeze = -0, SplashRange = _range };
            }
            else
            {
                Bullet = new Bullet { Damage = 0, Freeze = 0, SplashRange = 0 };
            }
        }
    }
}