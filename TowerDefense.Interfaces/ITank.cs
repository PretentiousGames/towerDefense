namespace TowerDefense.Interfaces
{
    public interface ITank : IEntity
    {
        string Name { get; }
        TankUpdate Update(IGameState gameState);
    }
}