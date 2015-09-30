using System.Collections.Generic;

namespace TowerDefense.Interfaces
{
    public interface IGameState
    {
        List<IFoe> Foes { get; }
        IEnumerable<IEntity> Entities { get; }
        Size Size { get; }
        List<IGoal> Goals { get; }
    }
}