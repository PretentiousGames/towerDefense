using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public interface IGameTank
    {
        double Heat { get; set; }
        ITank Tank { get; set; }
        Bullet Bullet { get; set; }
    }
}