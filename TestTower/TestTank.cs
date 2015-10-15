using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Business.Models;
using TowerDefense.Interfaces;

namespace TestTower
{
    public class TestTank : Tank
    {
        public Bullet Bullet { get; set; }
        public override string Name { get { return "TestTank"; } }

        public TestTank()
            : base(500, 500)
        {
            this.Speed = 1;
        }
        public override TankUpdate Update(IGameState gameState)
        {
            TankUpdate tankUpdate = new TankUpdate();

            if (gameState.Foes.Any() && gameState.Goals.Any())
            {
                tankUpdate.Target = gameState.Foes.OrderBy(foe => GetDistance(foe)).First();
                ChangeBulletPower(tankUpdate.Target);

                var x = (gameState.Foes.Average(foe => foe.X) + 99 * gameState.Goals.Average(goal => goal.X)) / 100;
                var y = (gameState.Foes.Average(foe => foe.Y) + 99 * gameState.Goals.Average(goal => goal.Y)) / 100;
                //tankUpdate.MovementTarget = LocationProvider.GetLocation(x, y);
            }

            return tankUpdate;
        }

        private double GetDistance(IFoe foe)
        {
            var xDistance = (this.X + this.Size.Width) - (foe.X + foe.Size.Width);
            var yDistance = (this.Y + this.Size.Height) - (foe.Y + foe.Size.Height);
            return Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));
        }
        private void ChangeBulletPower(IFoe foe)
        {
            var range = GetDistance(foe) + 1;
            var damage = (int)(1000 / range);
            var freeze = 0;

            var splash = new SplashBullet
            {
                Damage = 10,
                Range = 100,
                Target = new Point((int)foe.Location.X, (int)foe.Location.Y)
            };
            
            if (damage < foe.Health)
            {
                damage /= 2;
                freeze = damage;
                //damage = 1;
            }
            Bullet = new Bullet { Damage = damage, Range = range, Freeze = freeze, Splash = splash };
        }

        public override IBullet GetBullet()
        {
            return Bullet; //new Bullet { Damage = 1000 / 400, Range = 400 };
        }
    }
}
