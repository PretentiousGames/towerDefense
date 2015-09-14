using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerDefense.Interfaces;

namespace TestTower
{
    public class TestTower: ITower
    {
        public IFoe Update(GameState gameState)
        {
            return null;
        }

        public Bullet GetBullet()
        {
            return new Bullet();
        }
    }
}
