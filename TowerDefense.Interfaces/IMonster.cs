using System;
using TowerDefense.Business.Models;

namespace TowerDefense.Interfaces
{
    public interface IMonster : IFoe
    {
        void Update(IGameState gameState);

        Func<IGameState, AbilityResult> Ability { get; }
    }
}