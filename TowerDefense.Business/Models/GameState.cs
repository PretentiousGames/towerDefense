using System.Collections.Generic;
using System.Linq;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class GameState : IGameState
    {
        public List<IFoe> Foes { get; set; }
        public List<IGoal> Goals { get; set; }
        public List<IGravityEntity> GravityEntities { get; set; }
        public List<IGameTank> GameTanks { get; set; }

        public IEnumerable<IEntity> Entities
        {
            get
            {
                var entities = new List<IEntity>();
                if (Foes != null)
                {
                    entities.Concat(Foes);
                }
                if (Goals != null)
                {
                    entities.Concat(Goals);
                }
                if (GameTanks != null)
                {
                    entities.Concat(GameTanks.Select(tank => tank.Tank));
                }
                if (GravityEntities != null)
                {
                    entities.Concat(GravityEntities);
                }
                return entities;
            }
        }

        public Size Size { get; set; }
        public bool Lost { get; set; }
        public int Wave { get; set; }
    }
}
