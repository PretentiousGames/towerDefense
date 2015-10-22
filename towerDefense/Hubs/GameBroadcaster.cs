using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using TowerDefense.Business.Models;
using TowerDefense.Interfaces;

namespace towerDefense.Hubs
{
    public class GameBroadcaster : IGameBroadcaster
    {
        private IHubConnectionContext<dynamic> Clients { get; set; }

        public GameBroadcaster(IHubConnectionContext<dynamic> clients)
        {
            JsonConvert.SerializeObject(typeof(IGameState), Formatting.None, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            Clients = clients;
        }

        public void BroadcastGameState(IGameState gameState)
        {
            Clients.All.updateGameState(gameState);
        }
    }
}