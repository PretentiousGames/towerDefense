using System.Collections.Generic;

namespace TowerDefense.Interfaces
{
    public interface IGameState
    {
        IEnumerable<IFoe> Foes { get; }
        IEnumerable<IEntity> Entities { get; }
        Size Size { get; }
        List<IGoal> Goals { get; }
    }
}