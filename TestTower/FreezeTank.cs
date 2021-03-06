using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TowerDefense.Interfaces;

namespace TestTower
{
    public class FreezeTank : Tank
    {
        public Bullet Bullet { get; set; }
        public override string Name { get { return "Mr. Freeze"; } }

        public FreezeTank()
            : base(500, 500)
        {
        }
        public override TankUpdate Update(IGameState gameState)
        {
            TankUpdate tankUpdate = new TankUpdate();
            tankUpdate.TankColor = ConvertColorToHexString(Color.Blue);

            if (gameState.Foes.Any() && gameState.Goals.Any())
            {
                var target = gameState.Foes
                    .Where(foe => 1000 / GetDistanceFromTank(foe) >= 2)
                    .OrderBy(foe => GetDistanceToGoal(foe, gameState.Goals) + GetTimeToGoal(foe, gameState.Goals) + GetDistanceFromTank(foe))
                    .FirstOrDefault();

                if (target != null)
                {
                    tankUpdate.ShotTarget = target.Center;
                    ChangeBulletPower(target);
                    tankUpdate.Bullet = Bullet;
                }

                var x = (gameState.Foes.Max(foe => foe.X) + 9 * gameState.Goals.Last().X) / 10;
                var y = (gameState.Foes.Max(foe => foe.Y) + 9 * gameState.Goals.Last().Y) / 10;
                tankUpdate.MovementTarget = LocationProvider.GetLocation(x, y);
            }

            return tankUpdate;
        }
        
        private void ChangeBulletPower(IFoe foe)
        {
            var range = GetDistanceFromTank(foe) + 1;
            var damage = (int)(2000 / range);
            var freeze = 0;
            if (damage < foe.Health)
            {
                //damage /= 2;
                //freeze = damage;
                freeze = damage - 1;
                damage = 1;
            }
            Bullet = new Bullet { Damage = damage, Freeze = freeze };
        }
    }
}