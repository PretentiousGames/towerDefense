namespace TowerDefense.Interfaces
{
    public interface IGameBroadcaster
    {
        void BroadcastGameState(IGameState gameState);
    }
}