namespace TowerDefense.Interfaces
{
    public interface IBullet
    {
        int Damage { get; }
        int Freeze { get; }
        double SplashRange { get; }
        double GetReloadTime(double range);
        double GravityDuration { get; }
        double GravityStrength { get; }
    }
}