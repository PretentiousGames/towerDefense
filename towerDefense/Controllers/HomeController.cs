using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using TowerDefense.Interfaces;

namespace towerDefense.Controllers
{
    public class HomeController : Controller
    {
        private static List<Game> _games;

        static HomeController()
        {
            _games = new List<Game>();
        }

        // GET: Home
        public ActionResult Index()
        {
            return View("Index", _games);
        }

        [HttpPost]
        public ActionResult CreateGame(string creatorName, string gameName)
        {
            _games.Add(new Game { Name = gameName, Players = new List<Player> { new Player { Name = creatorName } } });
            return RedirectToAction("../Game/" + gameName);
        }

        [HttpPost]
        public ActionResult UploadFile(string name, HttpPostedFileBase file)
        {
            byte[] data = new byte[file.InputStream.Length];
            file.InputStream.Read(data, 0, data.Length);
            var assembly = Assembly.Load(data);

            var type = assembly.GetTypes().Single(t => t.GetInterfaces().Contains(typeof(ITower)));

            var constructor = type.GetConstructor(new Type[] { });

            var tower = constructor.Invoke(new object[] { });

            ((ITower)tower).GetBullet();

            return null;
        }
    }
}