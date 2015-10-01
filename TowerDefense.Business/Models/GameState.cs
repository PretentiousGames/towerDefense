using System.Collections.Generic;
using System.Linq;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class GameState : IGameState
    {
        public List<IFoe> Foes { get; set; }
        public List<IGoal> Goals { get; set; }
        public List<ITank> Tanks { get; set; }

        public IEnumerable<IEntity> Entities
        {
            get { return Foes.Select(foe => (IEntity)foe).Concat(Goals).Concat(Tanks); }
        }

        public Size Size { get; set; }
    }
}
