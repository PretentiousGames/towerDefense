using System.Collections.Generic;

namespace TowerDefense.Interfaces
{
    public class GameState
    {
        public IEnumerable<IFoe> Foes { get; set; }
    }
}
