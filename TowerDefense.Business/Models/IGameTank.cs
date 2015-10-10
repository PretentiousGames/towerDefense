using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public interface IGameTank
    {
        double Heat { get; set; }
        string Owner { get; set; }
        ITank Tank { get; set; }
        Bullet Bullet { get; set; }
        Monster Target { get; set; }
        bool Shooting { get; set; }
        int Killed { get; set; }
    }
}