using System.Collections.Generic;
using System.Web.Mvc;
using TowerDefense.Business;
using TowerDefense.Business.Models;

namespace towerDefense.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View("Index", GameManager.Games);
        }

        [HttpPost]
        public ActionResult CreateGame(string creatorName, string gameName)
        {
            GameManager.Games.Add(new Game {Name = gameName, Players = new List<Player>()});
            return RedirectToAction("../Game/" + gameName);
        }
    }
}