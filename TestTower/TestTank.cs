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
        public override IFoe Update(IGameState gameState)
        {
            if (gameState.Foes.Any())
            {
                return gameState.Foes.OrderBy(foe => GetDistance(foe)).First();
            }
            return null;
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
