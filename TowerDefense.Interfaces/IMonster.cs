using System;

namespace TowerDefense.Interfaces
{
    public interface IMonster : IFoe
    {
        IFoe Update(IGameState gameState);

        Func<IGameState, int> Ability { get; }
    }
}