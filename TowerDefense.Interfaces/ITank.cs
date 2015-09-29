namespace TowerDefense.Interfaces
{
    public interface ITank
    {
        double X { get; }
        double Y { get; }
        string Name { get; }
        IFoe Update(IGameState gameState);
        IBullet GetBullet();
    }
}