namespace TowerDefense.Interfaces
{
    public interface IKillable
    {
        int MaxHealth { get; }
        int Health { get; }
    }
}