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
            X = 30;
            Y = 30;
            Id = 1;
            Size = new Size(32);
        }
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public Size Size { get; set; }

        public string Name{get{return "TestTank";}}

        public IFoe Update(IGameState gameState)
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

        public IBullet GetBullet()
        {
            return new Bullet { Damage = 1000 / 40, Range = 40 };
        }
    }
}
