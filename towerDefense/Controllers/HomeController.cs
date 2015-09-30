using System.Collections.Generic;
using System.Linq;
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
            var gameName = "the game";
            if (GameManager.Games.Any())
            {
                gameName = GameManager.Games.First().Name;
            }
            else
            {
                GameManager.Games.Add(new Game { Name = gameName });
            }
            return RedirectToAction("../Game/" + gameName);
            //return View("Index", GameManager.Games);
        }

        [HttpPost]
        public ActionResult CreateGame(string creatorName, string gameName)
        {
            GameManager.Games.Add(new Game { Name = gameName });
            return RedirectToAction("../Game/" + gameName);
        }
    }
}