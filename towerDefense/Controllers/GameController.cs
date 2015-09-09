using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace towerDefense.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        public ActionResult Index(string gameName)
        {
            return View("Index", (object)gameName);
        }
    }
}