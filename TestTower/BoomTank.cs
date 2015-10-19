using System;
using System.Drawing;
using System.Linq;
using TowerDefense.Business.Models;
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

        public BoomTank()
            : base(700, 700)
        {
            _xTarget = 400;
            _yTarget = 400;
        }
        public override TankUpdate Update(IGameState gameState)
        {
            TankUpdate tankUpdate = new TankUpdate();

            if (gameState.Foes.Any() && gameState.Goals.Any())
            {
                tankUpdate.ShotTarget = this.Center;
                ChangeBulletPower(tankUpdate.ShotTarget);


                UpdateMovementTarget(tankUpdate, gameState);
                //var x = (gameState.Foes.Average(foe => foe.X) +  gameState.Goals.Average(goal => goal.X)) / 2;
                //var y = (gameState.Foes.Average(foe => foe.Y) +  gameState.Goals.Average(goal => goal.Y)) / 2;
                //tankUpdate.MovementTarget = LocationProvider.GetLocation(x, y);
            }

            return tankUpdate;
        }

        private void UpdateMovementTarget(TankUpdate tankUpdate, IGameState gameState)
        {
            //int maxFoes = GetFoesInRange(_xTarget, _yTarget, gameState);

            //for (int i = 0; i < 20; i++)
            //{
            //    var y = _rng.NextDouble() * gameState.Size.Height;
            //    var x = _rng.NextDouble() * gameState.Size.Width;
            //    var f = GetFoesInRange(x, y, gameState);
            //    if (f > maxFoes)
            //    {
            //        maxFoes = f;
            //        _xTarget = x;
            //        _yTarget = y;
            //    }
            //}

            _xTarget = 375;
            _yTarget = 375;
            tankUpdate.MovementTarget = LocationProvider.GetLocation(_xTarget, _yTarget);
        }

        private int GetFoesInRange(double xTarget, double yTarget, IGameState gameState)
        {
            var foes = 0;
            var range = 50;
            foreach (var foe in gameState.Foes)
            {
                if (Math.Abs(foe.X - xTarget) < range && Math.Abs(foe.Y - yTarget) < range)
                {
                    foes++;
                }
            }
            return foes;
        }

        private void ChangeBulletPower(ILocation target)
        {
            var range = GetDistanceFromTank(target) + 1;
            var damage = 5;
            Bullet = new Bullet { Damage = damage, Range = range, Freeze = 0, SplashRange = 100 };
        }

        public override IBullet GetBullet()
        {
            return Bullet; //new Bullet { Damage = 1000 / 400, Range = 400 };
        }
    }
}