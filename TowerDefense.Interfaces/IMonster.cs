namespace TowerDefense.Interfaces
{
    public interface IMonster : IFoe
    {
        double X { get; }
        double Y { get; }
        double Health { get; }
        double Speed { get; }
        double Size { get; }

        IFoe Update(IGameState gameState);
    }
}