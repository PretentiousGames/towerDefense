using TowerDefense.Business.Models;

namespace TowerDefense.Interfaces
{
    public interface IBullet
    {
        double Range { get; }
        int Damage { get; }
        int Freeze { get; }
        double SplashRange { get; }
        double ReloadTime { get; }
        double Gravity { get; }
    }
}