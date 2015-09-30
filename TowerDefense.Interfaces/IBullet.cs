namespace TowerDefense.Interfaces
{
    public interface IBullet
    {
        double Range { get; }
        double Damage { get; }
        double ReloadTime { get; }
    }

    public interface IGameBroadcaster
    {
        void BroadcastGameState(IGameState gameState);
    }
}