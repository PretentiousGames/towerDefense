namespace TowerDefense.Interfaces
{
    public interface IGravityEntity : IEntity
    {
        int Duration { get; }
        double Strength { get; }
    }
}