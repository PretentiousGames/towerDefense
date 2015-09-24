namespace TowerDefense.Interfaces
{
    public interface ITower
    {
        double X { get; set; }
        double Y { get; set; }
        string Name { get; }
        IFoe Update(GameState gameState);
        Bullet GetBullet();
    }
}