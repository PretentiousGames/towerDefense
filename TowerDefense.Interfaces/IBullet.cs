namespace TowerDefense.Interfaces
{
    public interface IBullet
    {
        double Range { get; }
        int Damage { get; }
        int Freeze { get; }
		int SplashDamage { get; }
		int SplashRange { get; }
        double ReloadTime { get; }
    }
}