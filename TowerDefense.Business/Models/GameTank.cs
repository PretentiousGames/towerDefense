using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class GameTank : IGameTank
    {
        public GameTank(Tank tank, string owner)
        {
            Tank = tank;
            Owner = owner;
            Location = (Location)tank.Location;
        }

        public ILocation Location { get; set; }
        public ILocation ShotTarget { get; set; }
        public bool Shooting { get; set; }
        public Bullet Bullet { get; set; }
        public int Killed { get; set; }

        public double Heat { get; set; }
        public string Owner { get; set; }
        public Tank Tank { get; set; }
    }
}