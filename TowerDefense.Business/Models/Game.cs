using System.Collections.Generic;

namespace TowerDefense.Business.Models
{
    public class Game
    {
        public string Name { get; set; }
        public List<Player> Players { get; set; }
    }
}