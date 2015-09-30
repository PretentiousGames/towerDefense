using System.Collections.Generic;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class Game
    {
        public Game()
        {
            Players = new List<Player>();
            Size = DefaultSize;
        }

        public static Size DefaultSize = new Size(800, 800);

        public string Name { get; set; }
        public List<Player> Players { get; set; }
        public Size Size { get; set; }
    }
}