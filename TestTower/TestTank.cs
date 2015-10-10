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
        public override string Name { get { return "TestTank"; } }

        public TestTank()
            : base(150, 150)
        {
        }
        public override TankUpdate Update(IGameState gameState)
        {
			TankUpdate tankUpdate = new TankUpdate();

            if (gameState.Foes.Any())
            {
				tankUpdate.Target = gameState.Foes.OrderBy(foe => GetDistance(foe)).First();
				//tankUpdate.MoveDirection = Movement.EAST;
            }

            return tankUpdate;
        }

        private double GetDistance(IFoe foe)
        {
            return Math.Sqrt(Math.Pow(X - foe.X, 2) + Math.Pow(Y - foe.Y, 2));
        }

        public override IBullet GetBullet()
        {
            return new Bullet { Damage = 1000 / 400, Range = 400 };
        }
    }
}
