using System.Collections.Generic;
using System.Linq;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class GameState : IGameState
    {
        public IEnumerable<IFoe> Foes { get; set; }
        public IEnumerable<IEntity> Entities
        {
            get { return Foes.Select(foe => (IEntity)foe); }
        }
        public Size Size { get; set; }
        public List<IGoal> Goals { get; set; }
    }
}
