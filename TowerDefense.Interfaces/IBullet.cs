using TowerDefense.Business.Models;

namespace TowerDefense.Interfaces
{
    public interface IBullet
    {
        double Range { get; }
        int Damage { get; }
        int Freeze { get; }
        SplashBullet Splash { get; }
        double ReloadTime { get; }
    }
}