using System;
using TowerDefense.Interfaces;

namespace TestTower
{
    public class ErrorTank : Tank
    {
        public override string Name { get { return "ErrorTank"; } }

        public ErrorTank()
            : base(400, 400)
        {
        }
        public override TankUpdate Update(IGameState gameState)
        {
            throw new AccessViolationException();
        }
    }
}