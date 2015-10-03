using System;
using System.Collections.Generic;
using System.Linq;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class Monster : IMonster
    {
        private static int _id = 0;
        public const int Width = 16;
        public const int Height = 16;
        public static int MonsterMaxHealth = 100;

        public Monster()
        {
            V = new Vector(GetRandomVDelta() * 10, GetRandomVDelta() * 10);
            Size = new Size(Width, Height);
            Id = _id++;
            Health = MaxHealth = MonsterMaxHealth;
        }

        private static Random _random = new Random();
        private int _gravityConstant = 500;

        public int Id { get; private set; }
        public double X { get; set; }
        public double Y { get; set; }
        public Vector V { get; set; }
        public int MaxHealth { get; private set; }
        public int Health { get; set; }
        public double Speed { get; set; }
        public Size Size { get; set; }

        public IFoe Update(IGameState gameState)
        {
            var pull = GeneratePull(gameState.Goals);
            var randomComponent = new Vector(GetRandomVDelta(), GetRandomVDelta());
            pull += randomComponent;
            V += pull;
            var xMovement = Math.Max(Math.Min(V.X, 1), -1);
            var yMovement = Math.Max(Math.Min(V.Y, 1), -1);

            if (CanMove(X + xMovement, Y, gameState))
            {
                X += xMovement;
            }
            else
            {
                V.X /= 2;
            }

            if (CanMove(X, Y + yMovement, gameState))
            {
                Y += yMovement;
            }
            else
            {
                V.Y /= 2;
            }

            return this;
        }

        private static double GetRandomVDelta()
        {
            return _random.NextDouble() * .1 - .05;
        }

        private Vector GeneratePull(List<IGoal> goals)
        {
            var pull = new Vector();
            foreach (var goal in goals)
            {
                var xComponent = goal.X + goal.Size.Width / 2 - X;
                var yComponent = goal.Y + goal.Size.Height / 2 - Y;
                var distanceSquared = xComponent * xComponent + yComponent * yComponent;
                var angle = Math.Atan2(yComponent, xComponent);
                var magnitude = _gravityConstant / distanceSquared;
                pull += new Vector(Math.Cos(angle) * magnitude, Math.Sin(angle) * magnitude);
            }
            return pull;
        }

        private bool CanMove(double x, double y, IGameState gameState)
        {
            return InBounds(x, y, gameState);
        }

        //private bool NoObstacles(double x, double y, IGameState gameState)
        //{
        //    foreach (var obstacle in gameState.Entities.Where(entity => this != entity))
        //    {
        //        if ((x < obstacle.X + obstacle.Size.Width) &&
        //            (x + Size.Width < obstacle.X) &&
        //            (y > obstacle.Y + obstacle.Size.Height) &&
        //            (y + Size.Height < obstacle.Y))
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        private bool InBounds(double x, double y, IGameState gameState)
        {
            return x + Size.Width < gameState.Size.Width && x > 0 &&
                   y + Size.Height < gameState.Size.Height && y > 0;
        }
    }
}