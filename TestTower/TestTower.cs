using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Interfaces;

namespace TestTower
{
    public class TestTower : ITower
    {
        public double X { get; set; }
        public double Y { get; set; }

        public string Name{get{return "TestTower";}}

        public IFoe Update(GameState gameState)
        {
            return gameState.Foes.OrderBy(foe => GetDistance(foe)).First();
        }

        private double GetDistance(IFoe foe)
        {
            return Math.Sqrt(Math.Pow(X - foe.X, 2) + Math.Pow(Y - foe.Y, 2));
        }

        public Bullet GetBullet()
        {
            return new Bullet { Damage = 1, Range = 1 };
        }
    }
}
