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
        public Vector V { get; set; }
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
            V = new Vector(GetRandomVDelta() * 20, GetRandomVDelta() * 20);
            Size = new Size(Width, Height);
            Id = _id++;
            Health = MaxHealth = monsterMaxHealth;
            Speed = MaxSpeed = 1;
            Generation = 1;
            CreateAbilities();
            AbilityType = abilityType;
            Ability = AbilitiesDictionary[AbilityType];
            SetOnDeathAbilities();
            SetOnHitAbilities();
            SetMovementTypes();
            SetMonsterSize();
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
                    gameState => {
                        var range = 10*(int) Math.Log(Health, 1.5);
                        if (FoeType == FoeType.Boss)
                        {
                            range = 10;
                            Health += 10;
                            return new AbilityResult() {Heat = 10, AbilityType = AbilityType.Healing, Range=range};
                        }
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
                }
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
                var xComponent = target.X + target.Size.Width / 2 - X;
                var yComponent = target.Y + target.Size.Height / 2 - Y;
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

                    var pull = GeneratePull(new List<IEntity>
                    {
                        new GravityEntity
                        {
                            Duration = 1,
                            Size = new Size(1, 1),
                            X = location.X,
                            Y = location.Y,
                            Strength = MaxHealth
                        }
                    }.Union(gameState.GravityEntities));
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
            V.X = Math.Min(Math.Max(V.X, -100), 100);
            V.Y = Math.Min(Math.Max(V.Y, -100), 100);

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
        }

        protected void SetOnDeathAbilities()
        {

        }

        protected void SetOnHitAbilities()
        {
            if (AbilityType == AbilityType.Splitter)
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
                            V = new Vector(V.X + _random.NextDouble() - .5, V.Y + _random.NextDouble() - .5),
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
            if (AbilityType == AbilityType.Healing)
            {
                //if (FoeType == FoeType.Monster)
                //{
                //    Size = new Size(36, 36);
                //}
                //else if (FoeType == FoeType.Boss)
                //{
                //    Size = new Size(72, 72);
                //}
            }
        }
    }
}