using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Ninject;
using towerDefense.Hubs;
using TowerDefense.Business;
using TowerDefense.Business.Models;
using TowerDefense.Interfaces;

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

            var type = assembly.GetTypes().Single(t => t.GetInterfaces().Contains(typeof(ITower)));

            var constructor = type.GetConstructor(new Type[] { });

            var tower = (ITower)constructor.Invoke(new object[] { });

            var game = GameManager.Games.Single(x => x.Name == gamename);

            Player player = game.Players.FirstOrDefault(x => x.Name == playername);

            if (player != null)
            {
                player.Towers.Add(tower);
            }
            else
            {
                List<ITower> towers = new List<ITower> { tower };
                game.Players.Add(new Player
                {
                    Name = playername,
                    Towers = towers
                });
            }

            return RedirectToAction("../Game/" + gamename);
        }

        [HttpPost]
        public JsonResult Carp()
        {
            IHubConnectionContext<dynamic> clients = GlobalHost.ConnectionManager.GetHubContext<GameHub>().Clients;
            GameBroadcaster gameBroadcaster = new GameBroadcaster(clients);
            Random r = new Random();

            Monster monster = new Monster { X = r.Next(800), Y = r.Next(800), Id = r.Next(8000) };
            for (int i = 0; i < 1000; i++)
            {
                gameBroadcaster.BroadcastGameState(new GameState { Foes = new List<IFoe> { monster } });
                monster.Update(new GameState());
            }
            return Json("Carp");
        }
    }
}