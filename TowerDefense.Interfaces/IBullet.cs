namespace TowerDefense.Interfaces
{
    public interface IBullet
    {
        double Range { get; }
        double Damage { get; }
        double ReloadTime { get; }
    }
}