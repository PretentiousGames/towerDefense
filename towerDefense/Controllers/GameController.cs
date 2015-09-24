using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
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

            if (game != null)
            {
                return View("Index", game);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult UploadFile(string playername, string gamename, HttpPostedFileBase file)
        {
            byte[] data = new byte[file.InputStream.Length];
            file.InputStream.Read(data, 0, data.Length);
            var assembly = Assembly.Load(data);

            var type = assembly.GetTypes().Single(t => t.GetInterfaces().Contains(typeof(ITower)));

            var constructor = type.GetConstructor(new Type[] { });

            var tower = (ITower) constructor.Invoke(new object[] { });

            var game = GameManager.Games.Single(x => x.Name == gamename);

            Player player = game.Players.FirstOrDefault(x => x.Name == playername);

            if (player != null)
            {
                player.Towers.Add(tower);
            }
            else
            {
                List<ITower> towers = new List<ITower>{tower};
                game.Players.Add(new Player
                {
                    Name = playername,
                    Towers = towers
                });
            }

            return RedirectToAction("../Game/" + gamename);
        }
    }
}