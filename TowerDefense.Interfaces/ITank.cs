namespace TowerDefense.Interfaces
{
    public interface ITank : IEntity
    {
        string Name { get; }
        IFoe Update(IGameState gameState);
        IBullet GetBullet();
    }
}