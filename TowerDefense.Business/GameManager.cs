using System.Collections.Generic;
using System.Linq;
using TowerDefense.Business.Models;

namespace TowerDefense.Business
{
    public class GameManager
    {
        public static Game GetGame(string gamename)
        {
            return Games.SingleOrDefault(x => x.Name == gamename);
        }
        public static List<Game> Games { get; set; }

        static GameManager()
        {
            Games = new List<Game>();
        }
    }
}
