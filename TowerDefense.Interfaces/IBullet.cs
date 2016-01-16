namespace TowerDefense.Interfaces
{
    public interface IBullet
    {
        int Damage { get; }
        int Freeze { get; }
        int SplashRange { get; }
        long GetReloadTime(double range);
        int GravityDuration { get; }
        double GravityStrength { get; }
    }
}