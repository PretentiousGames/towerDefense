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
            var game = GameManager.GetGame(gameName);
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

            var tower = (Tank)constructor.Invoke(new object[] { });

            var game = GameManager.GetGame(gamename);
            if (game != null)
            {
                Player player = game.Players.FirstOrDefault(x => x.Name == playername);

                if (player != null)
                {
                    player.Tanks.Add(tower);
                }
                else
                {
                    List<Tank> towers = new List<Tank> { tower };
                    game.Players.Add(new Player
                    {
                        Name = playername,
                        Tanks = towers
                    });
                }
            }
            return RedirectToAction("../Game/" + gamename);
        }

        [HttpPost]
        public ActionResult Start(string gameName)
        {
            var game = GameManager.GetGame(gameName);

            if (game == null)
            {
                return RedirectToAction("../Game/" + gameName);
            }

            IHubConnectionContext<dynamic> clients = GlobalHost.ConnectionManager.GetHubContext<GameHub>().Clients;
            GameBroadcaster gameBroadcaster = new GameBroadcaster(clients);
            game.StartNewGame(gameBroadcaster);

            return Json("start");
        }

        [HttpPost]
        public ActionResult StopGame(string gameName)
        {
            var game = GameManager.GetGame(gameName);

            if (game == null)
            {
                return RedirectToAction("../Game/" + gameName);
            }

            IHubConnectionContext<dynamic> clients = GlobalHost.ConnectionManager.GetHubContext<GameHub>().Clients;
            GameBroadcaster gameBroadcaster = new GameBroadcaster(clients);
            game.ClearGameOut(gameBroadcaster);

            return Json("start");
        }
    }
}