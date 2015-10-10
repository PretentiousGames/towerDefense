using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class GameTank : IGameTank
    {
        public GameTank(ITank tank, string owner)
        {
            Tank = tank;
            Owner = owner;
            Bullet = (Bullet)tank.GetBullet();
        }

        public Bullet Bullet { get; set; }
        public Monster Target { get; set; }
        public bool Shooting { get; set; }
        public int Killed { get; set; }

        public double Heat { get; set; }
        public string Owner { get; set; }
        public ITank Tank { get; set; }
    }
}