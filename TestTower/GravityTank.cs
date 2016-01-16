using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TowerDefense.Interfaces;

namespace TestTower
{
    public class GravityTank : Tank
    {
        private Random _rng = new Random();
        public Bullet Bullet { get; set; }
        public override string Name { get { return "Mr. Gravity"; } }

        public GravityTank()
            : base(200, 200)
        {
        }
        public override TankUpdate Update(IGameState gameState)
        {
            TankUpdate tankUpdate = new TankUpdate();

            tankUpdate.TankColor = GetRandomColor();

            if (gameState.Foes.Any() && gameState.Goals.Any())
            {
                var target = LocationProvider.GetLocation(375,375);

                if (target != null)
                {
                    tankUpdate.ShotTarget = target;
                    Bullet = new Bullet
                    {
                        Damage = 0,
                        Freeze = 0,
                        SplashRange = 0,
                        GravityDuration = 0.1,
                        GravityStrength = 0.01
                    };
                    tankUpdate.Bullet = Bullet;
                }
            }

            return tankUpdate;
        }

        private string GetRandomColor()
        {
            Color c = Color.FromArgb(_rng.Next(0, 255), _rng.Next(0, 255), _rng.Next(0, 255));
            return ConvertColorToHexString(c);
        }
    }
}
