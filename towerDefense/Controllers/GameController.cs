using System;
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
            var game = GameManager.Games.Single(x => x.Name == gameName);
            return View("Index", game);
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
            game.Players.Add(new Player
            {
                Name = playername,
                Tower = tower
            });

            return RedirectToAction("../Game/" + gamename);
        }
    }
}