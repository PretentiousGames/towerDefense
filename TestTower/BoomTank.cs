using System;
using System.Drawing;
using System.Linq;
using TowerDefense.Business.Models;
using TowerDefense.Interfaces;

namespace TestTower
{
    public class BoomTank : Tank
    {
        public Bullet Bullet { get; set; }
        public override string Name { get { return "Mr. Boomy"; } }

        public BoomTank()
            : base(700, 700)
        {
        }
        public override TankUpdate Update(IGameState gameState)
        {
            TankUpdate tankUpdate = new TankUpdate();

            if (gameState.Foes.Any() && gameState.Goals.Any())
            {
                tankUpdate.ShotTarget = this.Center;
                ChangeBulletPower(tankUpdate.ShotTarget);

                var x = (gameState.Foes.Average(foe => foe.X) +  gameState.Goals.Average(goal => goal.X)) / 2;
                var y = (gameState.Foes.Average(foe => foe.Y) +  gameState.Goals.Average(goal => goal.Y)) / 2;
                tankUpdate.MovementTarget = LocationProvider.GetLocation(x, y);
            }

            return tankUpdate;
        }
        private void ChangeBulletPower(ILocation target)
        {
            var range = GetDistanceFromTank(target) + 1;
            var damage = (int)(10 / range);
            var splash = new SplashBullet
            {
                Range = 400
            };
            Bullet = new Bullet { Damage = damage, Range = range, Freeze = 0, Splash = splash };
        }

        public override IBullet GetBullet()
        {
            return Bullet; //new Bullet { Damage = 1000 / 400, Range = 400 };
        }
    }
}