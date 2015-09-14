namespace TowerDefense.Interfaces
{
    public interface ITower
    {
        IFoe Update(GameState gameState);
        Bullet GetBullet();
    }
}