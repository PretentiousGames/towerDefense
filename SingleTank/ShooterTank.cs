﻿using System;
using System.Drawing;
using System.Linq;
using TowerDefense.Interfaces;

namespace SingleTank
{
    public class ShooterTank : Tank
    {
        private Random _rng = new Random();
        public Bullet Bullet { get; set; }
        public override string Name { get { return "Mr. Shooty"; } }
        private double _xTarget;
        private double _yTarget;

        public ShooterTank()
            : base(400, 400)
        {
            _xTarget = 400;
            _yTarget = 400;
        }
        public override TankUpdate Update(IGameState gameState)
        {
            TankUpdate tankUpdate = new TankUpdate();
            tankUpdate.TankColor = ConvertColorToHexString(Color.Red);

            if (gameState.Foes.Any() && gameState.Goals.Any())
            {
                var target = gameState.Foes
                    .OrderBy(foe => GetDistanceFromTank(foe) + ModifyDistanceByDanger(foe))
                    .FirstOrDefault();

                if (target != null)
                {
                    tankUpdate.ShotTarget = target.Center;
                    ChangeBulletPower(target);
                    tankUpdate.Bullet = Bullet;
                }

                UpdateMovementTarget(tankUpdate, gameState);
            }

            return tankUpdate;
        }

        private double ModifyDistanceByDanger(IFoe foe)
        {
            return foe.AbilityType == AbilityType.Healing || foe.AbilityType == AbilityType.RangedHeat ? -100 : 0;
        }

        private void UpdateMovementTarget(TankUpdate tankUpdate, IGameState gameState)
        {
            int maxFoes = GetFoesInRange(_xTarget, _yTarget, gameState);

            for (int i = 0; i < 20; i++)
            {
                var y = _rng.NextDouble() * gameState.Size.Height;
                var x = _rng.NextDouble() * gameState.Size.Width;
                var f = GetFoesInRange(x, y, gameState);
                if (f > maxFoes)
                {
                    maxFoes = f;
                    _xTarget = x;
                    _yTarget = y;
                }
            }

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

        private void ChangeBulletPower(IFoe foe)
        {
            var range = GetDistanceFromTank(foe) + 1;
            var damage = (int)(1000 / range);
            Bullet = new Bullet { Damage = damage, Freeze = 0, SplashRange = 0 };
        }
    }
}
