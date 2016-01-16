using System;

namespace TowerDefense.Interfaces
{
    public interface IMonster : IFoe
    {
        void Update(IGameState gameState);

        Func<IGameState, AbilityResult> Ability { get; }
    }
}