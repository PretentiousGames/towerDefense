namespace TowerDefense.Interfaces
{
    public interface IMonster : IFoe
    {
        IFoe Update(IGameState gameState);
    }
}