using System;
using System.Collections.Generic;
using System.Linq;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class Monster : IMonster
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Health { get; set; }
        public double Speed { get; set; }
        public double Size { get; set; }
        public IFoe Update(IGameState gameState)
        {
            var pull = GeneratePull(gameState.Goals);
            if (CanMove(X + pull.X, Y + pull.Y, gameState))
            {
                X += pull.X;
                Y += pull.Y;
            }
            return this;
        }

        private Vector GeneratePull(List<IGoal> goals)
        {
            var pull = new Vector();
            foreach (var goal in goals)
            {
                var xComponent = goal.X - X;
                var yComponent = goal.Y - Y;
                var distanceSquared = xComponent * xComponent + yComponent * yComponent;
                var angle = Math.Atan2(yComponent, xComponent);
                var magnitude = 100000 / distanceSquared;
                pull += new Vector(Math.Cos(angle) * magnitude, Math.Sin(angle) * magnitude);
            }
            return pull;
        }

        private bool CanMove(double x, double y, IGameState gameState)
        {
            return InBounds(x, y, gameState);
        }

        private bool NoObstacles(double x, double y, IGameState gameState)
        {
            foreach (var obstacle in gameState.Entities.Where(entity => this != entity))
            {
                if ((x < obstacle.X + obstacle.Size) &&
                    (x + Size < obstacle.X) &&
                    (y > obstacle.Y + obstacle.Size) &&
                    (y + Size < obstacle.Y))
                {
                    return false;
                }
            }
            return true;
        }

        private bool InBounds(double x, double y, IGameState gameState)
        {
            return x + Size < gameState.Size.Width && x > 0 &&
                   y + Size < gameState.Size.Height && Y > 0;
        }
    }
}