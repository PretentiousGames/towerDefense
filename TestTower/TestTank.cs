using System;
using System.Collections.Generic;
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
        public override string Name { get { return "Mr. Shooty"; } }
        private double _xTarget;
        private double _yTarget;
        
        public TestTank()
            : base(400, 400)
        {
            this.Speed = 1;
            _xTarget = 400;
            _yTarget = 400;
        }
        public override TankUpdate Update(IGameState gameState)
        {
            TankUpdate tankUpdate = new TankUpdate();

            if (gameState.Foes.Any() && gameState.Goals.Any())
            {
                tankUpdate.Target = gameState.Foes
                    .Where(foe => 1000 / GetDistanceFromTank(foe) >= 1)
                    .OrderBy(GetDistanceFromTank)
                    .FirstOrDefault();

                if (tankUpdate.Target != null)
                {
                    ChangeBulletPower(tankUpdate.Target);
                }
                
                UpdateMovementTarget(tankUpdate, gameState);
            }

            return tankUpdate;
        }

        private void UpdateMovementTarget(TankUpdate tankUpdate, IGameState gameState)
        {
            int maxFoes = GetFoesInRange(_xTarget, _yTarget, gameState);

            tankUpdate.MovementTarget = LocationProvider.GetLocation(_xTarget, _yTarget);
        }

        private int GetFoesInRange(double xTarget, double yTarget, IGameState gameState)
        {
            foreach (var foe in gameState.Foes)
            {
                if (foe.X)
            }
        }

        private void ChangeBulletPower(IFoe foe)
        {
            var range = GetDistanceFromTank(foe) + 1;
            var damage = (int)(1000 / range);
            Bullet = new Bullet { Damage = damage, Range = range, Freeze = 0 };
        }

        public override IBullet GetBullet()
        {
            return Bullet; //new Bullet { Damage = 1000 / 400, Range = 400 };
        }
    }
}
