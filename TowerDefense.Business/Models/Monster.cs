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
        public static int MonsterMaxHealth = 10;

        public Monster()
        {
            V = new Vector(GetRandomVDelta() * 10, GetRandomVDelta() * 10);
            Size = new Size(Width, Height);
            Id = _id++;
            Health = MaxHealth = MonsterMaxHealth;
            Speed = 1;
        }

        private static Random _random = new Random();
        private int _gravityConstant = 500;

        public int Id { get; }
        public double X { get { return Location.X; } }
        public double Y { get { return Location.Y; } }
        public ILocation Location { get; set; }
        public Vector V { get; set; }
        public int MaxHealth { get; }
        public int Health { get; set; }
        public double Speed { get; set; }
        public Size Size { get; set; }

        public IFoe Update(IGameState gameState)
        {
            var pull = GeneratePull(gameState.Goals);
            var randomComponent = new Vector(GetRandomVDelta(), GetRandomVDelta());
            pull += randomComponent;
            V += pull;
            var angle = Math.Atan2(V.Y, V.X);
            var speed = Math.Min(Speed, Math.Sqrt(V.X * V.X + V.Y * V.Y));
            var xMovement = speed * Math.Cos(angle);
            var yMovement = speed * Math.Sin(angle);

            if (CanMove(X + xMovement, Y, gameState))
            {
                ((Location)Location).X += xMovement;
            }
            else
            {
                V.X /= 2;
            }

            if (CanMove(X, Y + yMovement, gameState))
            {
                ((Location)Location).Y += yMovement;
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