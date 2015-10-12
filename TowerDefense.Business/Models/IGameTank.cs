using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public interface IGameTank
    {
        double Heat { get; set; }
        string Owner { get; set; }
        Tank Tank { get; set; }
        Location Location { get; set; }
        Monster Target { get; set; }
        bool Shooting { get; set; }
        int Killed { get; set; }
    }
}