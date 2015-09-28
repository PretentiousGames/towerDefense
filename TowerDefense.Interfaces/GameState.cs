using System.Collections.Generic;
using System.Linq;

namespace TowerDefense.Interfaces
{
    public class GameState
    {
        public IEnumerable<IFoe> Foes { get; set; }
        public IEnumerable<IEntity> Entities
        {
            get { return Foes.Select(foe => (IEntity)foe); }
        }
        public Size Size { get; set; }
        public List<Goal> Goals { get; set; }
    }

    public class Goal
    {
    }

    public class Size
    {
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
