using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json.Serialization;
using Ninject;
using towerDefense.Hubs;
using TowerDefense.Business;
using TowerDefense.Business.Models;
using TowerDefense.Interfaces;
using Size = TowerDefense.Interfaces.Size;

namespace towerDefense.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        public ActionResult Index(string gameName)
        {
            var game = GameManager.Games.SingleOrDefault(x => x.Name == gameName);
            return game == null ? (ActionResult)RedirectToAction("Index", "Home") : View("Index", game);
        }

        [HttpPost]
        public ActionResult UploadFile(string playername, string gamename, HttpPostedFileBase file)
        {
            byte[] data = new byte[file.InputStream.Length];
            file.InputStream.Read(data, 0, data.Length);
            var assembly = Assembly.Load(data);

            var type = assembly.GetTypes().Single(t => t.GetInterfaces().Contains(typeof(ITank)));

            var constructor = type.GetConstructor(new Type[] { });

            var tower = (ITank)constructor.Invoke(new object[] { });

            var game = GameManager.Games.Single(x => x.Name == gamename);

            Player player = game.Players.FirstOrDefault(x => x.Name == playername);

            if (player != null)
            {
                player.Towers.Add(tower);
            }
            else
            {
                List<ITank> towers = new List<ITank> { tower };
                game.Players.Add(new Player
                {
                    Name = playername,
                    Towers = towers
                });
            }

            return RedirectToAction("../Game/" + gamename);
        }

        private static Thread _gameThread;
        [HttpPost]
        public JsonResult Start()
        {
            if (_gameThread != null && _gameThread.IsAlive)
            {
                _gameThread.Abort();
            }
            _gameThread = new Thread(ThreadStart);
            _gameThread.Start();

            return Json("start");
        }

        private static void ThreadStart()
        {
            IHubConnectionContext<dynamic> clients = GlobalHost.ConnectionManager.GetHubContext<GameHub>().Clients;
            GameBroadcaster gameBroadcaster = new GameBroadcaster(clients);
            //Random r = new Random();

            var height = Game.DefaultSize.Height;
            var width = Game.DefaultSize.Width;
            var gameState = new GameState
            {
                Foes = new List<IFoe>(),
                Size = new Size { Height = height, Width = width },
                Goals = new List<IGoal>
                {
                    new Goal {X = 0, Y = 0},
                    new Goal {X = width - Goal.Width, Y = 0},
                    new Goal {X = 0, Y = height - Goal.Height},
                    new Goal {X = width - Goal.Width, Y = height - Goal.Height}
                }
            };

            int foeCount = 10;
            while (true)
            {
                if (gameState.Foes.Count < foeCount)
                {
                    Monster m = new Monster { X = 400 - 8, Y = 400 - 8 };
                    gameState.Foes.Add(m);
                }

                gameBroadcaster.BroadcastGameState(gameState);
                for (int j = 0; j < gameState.Foes.Count; j++)
                {
                    var monster = (IMonster)gameState.Foes[j];
                    monster.Update(gameState);
                    var goal = IsMonsterAtGoal(monster, gameState.Goals);
                    if (goal != null)
                    {
                        gameState.Foes.Remove(monster);
                        j--;
                    }
                }
                Thread.Sleep(10);
            }
        }

        public static IGoal IsMonsterAtGoal(IFoe monster, List<IGoal> goals)
        {
            foreach (var goal in goals)
            {
                if (((monster.X - monster.Size.Width / 2) > goal.X) && (monster.X + monster.Size.Width / 2 < (goal.X + goal.Size.Width)) &&
                    ((monster.Y - monster.Size.Height / 2) > goal.Y) && (monster.Y + monster.Size.Height / 2 < (goal.Y + goal.Size.Height)))
                {
                    return goal;
                }
            }

            return null;
        }
    }
    public class LowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}