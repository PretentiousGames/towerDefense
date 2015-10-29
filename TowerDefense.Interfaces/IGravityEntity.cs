namespace TowerDefense.Interfaces
{
    public interface IGravityEntity : IEntity
    {
        double Duration { get; }
        double Strength { get; }
    }
}