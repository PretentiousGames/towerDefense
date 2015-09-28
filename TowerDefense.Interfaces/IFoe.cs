using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TowerDefense.Interfaces
{
    public interface IEntity
    {
        double X { get; }
        double Y { get; }
        double Size { get; }
    }

    public interface IFoe : IEntity
    {
        double Health { get; }
        double Speed { get; }
    }

    public interface IMonster : IFoe
    {
        double X { set; }
        double Y { set; }
        double Health { set; }
        double Speed { set; }
        double Size { set; }

        IFoe Update(GameState gameState);
    }

    public class Monster : IMonster
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Health { get; set; }
        public double Speed { get; set; }
        public double Size { get; set; }
        public IFoe Update(GameState gameState)
        {
            var pull = GeneratePull(gameState.Goals);

            if (CanMove(X + 1, Y + 1, gameState))
            {
                X++;
                Y++;
            }
            else if (CanMove(X + 1, Y, gameState))
            {
                X++;
            }
            else if (CanMove(X, Y + 1, gameState))
            {
                Y++;
            }
            else
            {
                X--;
                Y--;
            }
            return this;
        }

        private Vector GeneratePull(List<Goal> goals)
        {
            //foreach (var goal in goals)
            {
                
            }
            return new Vector();
        }

        private bool CanMove(double x, double y, GameState gameState)
        {
            return InBounds(x, y, gameState) &&
                NoObstacles(x, y, gameState);
        }

        private bool NoObstacles(double x, double y, GameState gameState)
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

        private bool InBounds(double x, double y, GameState gameState)
        {
            return x + Size < gameState.Size.Width && x > 0 &&
                   y + Size < gameState.Size.Height && Y > 0;
        }
    }

    internal class Vector
    {
    }
}