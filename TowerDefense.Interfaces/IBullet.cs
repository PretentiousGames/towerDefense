namespace TowerDefense.Interfaces
{
    public interface IBullet
    {
        double Range { get; }
        int Damage { get; }
        double ReloadTime { get; }
    }

    public interface IGameBroadcaster
    {
        void BroadcastGameState(IGameState gameState);
    }
}