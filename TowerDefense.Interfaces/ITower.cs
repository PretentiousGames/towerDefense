namespace TowerDefense.Interfaces
{
    public interface ITower
    {
        double X { get; set; }
        double Y { get; set; }
        IFoe Update(GameState gameState);
        Bullet GetBullet();
    }
}