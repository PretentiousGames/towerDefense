using Microsoft.AspNet.SignalR.Hubs;
using TowerDefense.Business.Models;
using TowerDefense.Interfaces;

namespace towerDefense.Hubs
{
    public class GameBroadcaster
    {
        private IHubConnectionContext<dynamic> Clients { get; set; }

        public GameBroadcaster(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
        }
        
        public void BroadcastGameState(GameState gameState)
        {
            Clients.All.updateGameState(gameState);
        }
    }
}