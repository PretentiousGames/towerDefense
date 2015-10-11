namespace TowerDefense.Interfaces
{
    public interface IBullet
    {
        double Range { get; }
        int Damage { get; }
        int Freeze { get; }
        double ReloadTime { get; }
    }
}