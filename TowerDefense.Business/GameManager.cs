using System.Collections.Generic;
using TowerDefense.Business.Models;

namespace TowerDefense.Business
{
    public class GameManager
    {
        public static List<Game> Games { get; set; }

        static GameManager()
        {
            Games = new List<Game>();
        }
    }
}
