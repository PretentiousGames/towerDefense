using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using TowerDefense.Business.Models;
using TowerDefense.Interfaces;

namespace towerDefense.Hubs
{
    [HubName("gameHub")]
    public class GameHub : Hub
    {
        public GameState getGameState()
        {
            return new GameState { Foes = new List<IFoe> { new Monster { X = 400, Y = 400 } }, Goals = new List<IGoal> { new Goal { X = 0, Y = 0 } } };
        }
    }
}