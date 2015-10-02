using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Business.Models;
using TowerDefense.Interfaces;

namespace TestTower
{
    public class TestTank : ITank
    {
        public TestTank()
        {
            X = 50;
            Y = 50;
        }
        public double X { get; set; }
        public double Y { get; set; }
        public Size Size { get; set; }

        public string Name{get{return "TestTank";}}

        public IFoe Update(IGameState gameState)
        {
            return gameState.Foes.OrderBy(foe => GetDistance(foe)).First();
        }

        private double GetDistance(IFoe foe)
        {
            return Math.Sqrt(Math.Pow(X - foe.X, 2) + Math.Pow(Y - foe.Y, 2));
        }

        public IBullet GetBullet()
        {
            return new Bullet { Damage = 1, Range = 100 };
        }
    }
}
