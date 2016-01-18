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
        [JsonIgnore]
        public Action<IGameState> OnDeathAbility { get; protected internal set; }
        [JsonIgnore]
        public Action<IGameState, int> OnHitAbility { get; protected internal set; }
        [JsonIgnore]
        public Action<IGameState> DoMovement { get; protected internal set; }
        [JsonIgnore]
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
        public Vector Velocity { get; set; }
        public int MaxHealth { get; }
        public int Health { get; set; }
        public FoeType FoeType { get; protected set; }
        public double Speed { get; set; }
        public double MaxSpeed { get; set; }
        public Size Size { get; set; }
        public int Generation { get; set; }
        private double _xTarget = 400;
        private double _yTarget = 400;
        [JsonIgnore]
        public Func<IGameState, AbilityResult> Ability { get; protected set; }

        public Monster(int monsterMaxHealth) : this(monsterMaxHealth, AbilityType.Kamakaze)
        {
        }

        public Monster(int monsterMaxHealth, AbilityType abilityType)
        {
            Velocity = new Vector(GetRandomVDelta() * 40, GetRandomVDelta() * 40);
            Size = new Size(Width, Height);
            Id = _id++;
            Health = MaxHealth = GetMonsterMaxHealth(monsterMaxHealth, abilityType);
            Speed = MaxSpeed = GetMaxSpeed(abilityType);
            Generation = 1;
            CreateAbilities();
            AbilityType = abilityType;
            Ability = AbilitiesDictionary[AbilityType];
            SetOnDeathAbilities();
            SetOnHitAbilities();
            SetMovementTypes();
            SetMonsterSize();
        }

        private static int GetMonsterMaxHealth(int monsterMaxHealth, AbilityType abilityType)
        {
            return abilityType == AbilityType.Fast ? monsterMaxHealth / 3 : monsterMaxHealth;
        }

        private static int GetMaxSpeed(AbilityType abilityType)
        {
            return abilityType == AbilityType.Fast ? 3 : 1;
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
                            return new AbilityResult() {Heat = _heat, AbilityType = AbilityType.Kamakaze};
                        }

                        return new AbilityResult() {Heat = _heat, AbilityType = AbilityType.None};
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

                        _heat += 2;
                        return  new AbilityResult() {Heat = _heat, AbilityType = AbilityType.RangedHeat, Range=range};
                    }
                },
                {
                    AbilityType.Healing,
                    gameState =>
                    {
                        var range = 10*(int) Math.Log(Health, 1.5);
                        if (FoeType == FoeType.Boss)
                        {
                            Health += 10;
                        }
                        else
                        {
                            foreach (var foe in ((IGameState) gameState).Foes)
                            {
                                if (IsEntityInRange(range, foe))
                                {
                                    var monster = (Monster) foe;
                                    if (monster.Health < monster.MaxHealth*1.5)
                                    {
                                        monster.Health += 5;
                                    }
                                }
                            }
                        }

                        var goal = IsAtGoal(gameState.Goals);
                        if (goal != null)
                        {
                            ((Goal)goal).Health -= (Health / 20);
                            Health = 0;
                            return new AbilityResult() {Heat = _heat, AbilityType = AbilityType.Kamakaze};
                        }

                        _heat += 1;
                        return new AbilityResult() {Heat = _heat, AbilityType = AbilityType.Healing, Range=range};
                    }
                },
                {
                    AbilityType.Splitter,
                    gameState => {
                        var goal = IsAtGoal(gameState.Goals);
                        if (goal != null)
                        {
                            ((Goal)goal).Health -= (Health / 10);
                            Health = 0;

                            return new AbilityResult() {Heat = _heat, AbilityType = AbilityType.Kamakaze};
                        }

                        return new AbilityResult() {Heat = _heat, AbilityType = AbilityType.None};
                    }
                },
                {
                    AbilityType.Fast,
                    gameState => {
                        var goal = IsAtGoal(gameState.Goals);
                        if (goal != null)
                        {
                            ((Goal)goal).Health -= (Health / 10);
                            Health = 0;
                            return new AbilityResult() {Heat = _heat, AbilityType = AbilityType.Kamakaze};
                        }

                Speed = (Speed + MaxSpeed) / 2;
                Velocity.X *= 1.1;
                Velocity.Y *= 1.1;

                        return new AbilityResult() {Heat = _heat, AbilityType = AbilityType.None};
                    }
                },

            };
        }

        public void Update(IGameState gameState)
        {
            DoMovement(gameState);
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

        private static double GetRandomVDelta()
        {
            return _random.NextDouble() * .1 - .05;
        }

        private Vector GeneratePull(IEnumerable<IEntity> targets)
        {
            var pull = new Vector();
            foreach (var target in targets)
            {
                var xComponent = target.Center.X - Center.X;
                var yComponent = target.Center.Y - Center.Y;
                var distanceSquared = xComponent * xComponent + yComponent * yComponent;
                var angle = Math.Atan2(yComponent, xComponent);
                var magnitude = _gravityConstant / distanceSquared;
                if (target is IGravityEntity)
                {
                    magnitude *= ((GravityEntity)target).Strength * 1000 / Health;
                }
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

        private int GetFoesInRange(int range, double xTarget, double yTarget, IGameState gameState)
        {
            var foes = 0;
            foreach (var foe in gameState.Foes)
            {
                if (Math.Abs(foe.X - xTarget) < range && Math.Abs(foe.Y - yTarget) < range)
                {
                    foes++;
                }
            }
            return foes;
        }

        private void SetMovementTypes()
        {
            if (AbilityType == AbilityType.Healing)
            {
                DoMovement = (gameState) =>
                {
                    var range = 10 * (int)Math.Log(Health, 1.5);
                    int maxFoes = GetFoesInRange(range, _xTarget, _yTarget, gameState);
                    
                    for (int i = 0; i < 20; i++)
                    {
                        var y = _random.NextDouble() * gameState.Size.Height;
                        var x = _random.NextDouble() * gameState.Size.Width;
                        var f = GetFoesInRange(range, x, y, gameState);
                        if (f > maxFoes)
                        {
                            maxFoes = f;
                            _xTarget = x;
                            _yTarget = y;
                        }
                    }

                    var location = new Location(_xTarget, _yTarget);


                    Vector pull;

                    if (maxFoes == 1)
                    {
                        pull = GeneratePull(gameState.Goals.Select(x => (IEntity) x).Union(gameState.GravityEntities));
                    }
                    else
                    {
                        pull = GeneratePull(new List<IEntity>
                        {
                            new GravityEntity
                            {
                                Duration = 1,
                                Size = new Size(1, 1),
                                X = location.X,
                                Y = location.Y,
                                Strength = MaxHealth / 100.0
                            }
                        }.Union(gameState.GravityEntities));
                    }
                    DoActualMovement(pull, gameState);
                };
            }
            else
            {
                DoMovement = (gameState) =>
                {
                    var pull = GeneratePull(gameState.Goals.Select(x => (IEntity)x).Union(gameState.GravityEntities));
                    DoActualMovement(pull, gameState);
                };
            }
        }

        private void DoActualMovement(Vector pull, IGameState gameState)
        {
            if (Velocity.Total > 200)
            {
                Velocity.Total = 200;
            }

            var randomComponent = new Vector(GetRandomVDelta(), GetRandomVDelta());
            pull += randomComponent;
            Velocity += pull;
            var angle = Math.Atan2(Velocity.Y, Velocity.X);
            var speed = Math.Min(Speed, Math.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y));
            var xMovement = speed * Math.Cos(angle);
            var yMovement = speed * Math.Sin(angle);

            if (CanMove(X + xMovement, Y, gameState))
            {
                ((Location)Location).X += xMovement;
            }
            else
            {
                Velocity.X /= 2;
            }

            if (CanMove(X, Y + yMovement, gameState))
            {
                ((Location)Location).Y += yMovement;
            }
            else
            {
                Velocity.Y /= 2;
            }
        }

        protected void SetOnDeathAbilities()
        {

        }

        protected void SetOnHitAbilities()
        {
            if (AbilityType == AbilityType.Fast)
            {
                Speed = (Speed + MaxSpeed) / 2;
                Velocity.Total *= 1.1;
            }
            else if (AbilityType == AbilityType.Splitter)
            {
                OnHitAbility = (gameState, damageTaken) =>
                {
                    if (_heat <= 0 && gameState.Foes.Count < 100 && Health > 1 && Generation < 5)
                    {
                        var heatGain = 20;
                        _heat += heatGain;
                        Health /= 2;
                        Generation++;
                        var splitling = new Monster(Health, AbilityType.Splitter)
                        {
                            Location = new Location(X, Y),
                            Velocity = new Vector(Velocity.X + _random.NextDouble() - .5, Velocity.Y + _random.NextDouble() - .5),
                            Speed = Speed,
                            Generation = Generation
                        };
                        splitling._heat = heatGain;
                        gameState.Foes.Add(splitling);
                    }
                };
            }
        }

        protected void SetMonsterSize()
        {
            if (AbilityType == AbilityType.Kamakaze)
            {
                if (FoeType == FoeType.Monster)
                {
                    Size = new Size(36, 38);
                }
                else if (FoeType == FoeType.Boss)
                {
                    Size = new Size(67, 72);
                }
            }
            else if (AbilityType == AbilityType.Fast)
            {
                if (FoeType == FoeType.Monster)
                {
                    Size = new Size(23, 40);
                }
                else if (FoeType == FoeType.Boss)
                {
                    Size = new Size(41, 70);
                }
            }
            else if (AbilityType == AbilityType.Healing)
            {
                if (FoeType == FoeType.Monster)
                {
                    Size = new Size(32, 32);
                }
                else if (FoeType == FoeType.Boss)
                {
                    Size = new Size(64, 64);
                }
            }
        }
    }
}