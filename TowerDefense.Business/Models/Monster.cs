using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class Monster : IMonster
    {
        private static int _id = 0;
        public const int Width = 16;
        public const int Height = 16;
        protected int _heat = 0;
        public AbilityType AbilityType { get; protected set; }
        public Func<IGameState, Monster, bool> OnDeathListener { get; protected internal set; } 
        protected static Random _random = new Random();
        protected int _gravityConstant = 500;

        [JsonIgnore]
        public Dictionary<AbilityType, Func<IGameState, AbilityResult>> AbilitiesDictionary { get; set; }
        
        public AbilityResult AbilityResult { get; set; }
        public int Id { get; }
        public double X { get { return Location.X; } }
        public double Y { get { return Location.Y; } }
        public ILocation Location { get; set; }
        public ILocation Center
        {
            get { return new Location(X + Size.Width / 2, Y + Size.Height / 2); }
        }
        public Vector V { get; set; }
        public int MaxHealth { get; }
        public int Health { get; set; }
        public FoeType FoeType { get; protected set; }
        public double Speed { get; set; }
        public double MaxSpeed { get; set; }
        public Size Size { get; set; }


        public Monster(int MonsterMaxHealth) : this(MonsterMaxHealth, AbilityType.Kamakaze)
        {
        }

        public Monster(int MonsterMaxHealth, AbilityType abilityType)
        {
            V = new Vector(GetRandomVDelta() * 20, GetRandomVDelta() * 20);
            Size = new Size(Width, Height);
            Id = _id++;
            Health = MaxHealth = MonsterMaxHealth;
            Speed = MaxSpeed = 1;
            CreateAbilities();
            AbilityType = abilityType;
            Ability = AbilitiesDictionary[AbilityType];
        }


        private void CreateAbilities()
        {
            AbilitiesDictionary = new Dictionary<AbilityType, Func<IGameState, AbilityResult>>
            {
                {
                    AbilityType.Kamakaze,
                    gameState => {
                        var goal = IsAtGoal(gameState.Goals);
                        if (goal != null)
                        {
                            ((Goal)goal).Health -= (Health / 10);
                            Health = 0;
                        }

                        return new AbilityResult() {Heat = 0, AbilityType = AbilityType.Kamakaze};
                    }
                },
                {
                    AbilityType.RangedHeat,
                    gameState => {
                        var range = (int)Math.Log(Health, 1.5);
                        foreach (var tank in ((GameState)gameState).GameTanks)
                        {
                            if (IsEntityInRange(range, tank.Tank))
                            {
                                tank.Heat += 10;
                            }
                        }

                        foreach (var g in ((GameState)gameState).Goals)
                        {
                            var goal = (Goal) g;
                            if (IsEntityInRange(range, goal))
                            {
                                goal.Health -= 1;
                            }
                        }

                        return  new AbilityResult() {Heat = 2, AbilityType = AbilityType.RangedHeat, Range=range};
                    }
                },
                {
                    AbilityType.Healing,
                    gameState => {
                        var range = 100;
                        foreach (var foe in ((IGameState)gameState).Foes)
                        {
                            if (IsEntityInRange(range, foe))
                            {
                                var monster = (Monster) foe;
                                if (monster.Health < monster.MaxHealth*1.5)
                                {
                                    monster.Health++;
                                }
                            }
                        }

                        return new AbilityResult() {Heat = 10, AbilityType = AbilityType.Healing, Range=range};
                    }
                },
                {
                    AbilityType.Splitter,
                    gameState => {
                        return new AbilityResult() {Heat = 0, AbilityType = AbilityType.Splitter};
                    }
                },
                {
                    AbilityType.Splitling,
                    gameState => {
                        var goal = IsAtGoal(gameState.Goals);
                        if (goal != null)
                        {
                            ((Goal)goal).Health -= (Health / 10);
                            Health = 0;
                        }

                        return new AbilityResult() {Heat = 0, AbilityType = AbilityType.Splitling};
                    }
                }
            };
        }

        public void Update(IGameState gameState)
        {
            var pull = GeneratePull(gameState.Goals, gameState.GravityEntities);
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

            if (_heat <= 0)
            {
                var abilityResult = ExecuteAbility(gameState);
                _heat += abilityResult.Heat;
                AbilityResult = abilityResult;
            }
            else
            {
                _heat--;
            }
        }
        [JsonIgnore]
        public Func<IGameState, AbilityResult> Ability { get; protected set; }

        private static double GetRandomVDelta()
        {
            return _random.NextDouble() * .1 - .05;
        }

        private Vector GeneratePull(List<IGoal> goals, List<IGravityEntity> gravityEntities)
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

            foreach (var gravityEntity in gravityEntities)
            {
                var xComponent = gravityEntity.X + gravityEntity.Size.Width / 2 - X;
                var yComponent = gravityEntity.Y + gravityEntity.Size.Height / 2 - Y;
                var distanceSquared = xComponent * xComponent + yComponent * yComponent;
                var angle = Math.Atan2(yComponent, xComponent);
                var magnitude = _gravityConstant / distanceSquared;
                magnitude *= gravityEntity.Strength;
                pull += new Vector(Math.Cos(angle) * magnitude, Math.Sin(angle) * magnitude);
            }

            return pull;
        }

        private bool CanMove(double x, double y, IGameState gameState)
        {
            return InBounds(x, y, gameState);
        }

        private bool InBounds(double x, double y, IGameState gameState)
        {
            return x + Size.Width < gameState.Size.Width && x > 0 &&
                   y + Size.Height < gameState.Size.Height && y > 0;
        }

        public AbilityResult ExecuteAbility(IGameState gameState)
        {
            return Ability(gameState);
        }
        protected bool IsEntityInRange(int range, IEntity entity)
        {
            return Math.Abs(entity.Center.X - Center.X) < range && Math.Abs(entity.Center.Y - Center.Y) < range;
        }
        public IGoal IsAtGoal(List<IGoal> goals)
        {
            foreach (var goal in goals)
            {
                if (((Center.X) > goal.X) && (Center.X < (goal.X + goal.Size.Width)) &&
                    ((Center.Y) > goal.Y) && (Center.Y < (goal.Y + goal.Size.Height)))
                {
                    return goal;
                }
            }

            return null;
        }
    }
}