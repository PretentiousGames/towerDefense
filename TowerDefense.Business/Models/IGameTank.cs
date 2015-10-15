using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public interface IGameTank
    {
        double Heat { get; set; }
        string Owner { get; set; }
        Tank Tank { get; set; }
        ILocation Location { get; set; }
        ILocation ShotTarget { get; set; }
        bool Shooting { get; set; }
        int Killed { get; set; }
    }
}