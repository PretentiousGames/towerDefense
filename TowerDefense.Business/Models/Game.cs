using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TestTower;
using TowerDefense.Interfaces;
using Size = TowerDefense.Interfaces.Size;

namespace TowerDefense.Business.Models
{
    public class Game
    {
        private Thread _thread;
        private GameThread _gameThread;

        public int Killed { get; set; }
        public GameState GameState { get; private set; }
        public string Name { get; set; }
        public List<Player> Players { get; set; }
        public Size DefaultSize = new Size(800, 800);
        public Size Size { get; set; }
        public IGameBroadcaster GameBroadcaster { get; set; }
        public int FoeCount { get; set; }
        public int FoesToSpawn { get; set; }
        public int MonsterStartHealth { get; set; }

        public Game()
        {
            Players = new List<Player>();
            Size = DefaultSize;
            Tank.SetLocationProvider(new LocationProvider());
        }

        private void Setup()
        {
            if (Players.Count == 0)
            {
                Players.Add(new Player
                {
                    Name = "demo",
                    Tanks = new List<Tank> { new GravityTank(), new FreezeTank(), new TestTank() } //new BaseTank(), new TestTank(), new FreezeTank(), new BoomTank() }
                });
            }

            GameState = GenerateGameState(DefaultSize.Height, DefaultSize.Width, this);

            MonsterStartHealth = 10;
            FoeCount = 0;
            NewWave();
        }

        public void StartNewGame(IGameBroadcaster gameBroadcaster)
        {
            Setup();

            if (_thread != null && _thread.IsAlive)
            {
                _thread.Abort(gameBroadcaster);
            }

            GameBroadcaster = gameBroadcaster;

            _gameThread = new GameThread(this);
            _thread = new Thread(_gameThread.Run);
            _thread.Start(this);
        }

        public List<Monster> GetFoesInRange(double x, double y, double radius)
        {
            List<Monster> foesInRange = new List<Monster>();

            foreach (var foe in GameState.Foes)
            {
                var distance = GetDistance(foe, x, y);
                if (distance <= radius + (foe.Size.Height + foe.Size.Width) / 8.0)
                {
                    foesInRange.Add((Monster)foe);
                }
            }

            return foesInRange;
        }
        protected static double GetDistance(IEntity entity1, double x, double y)
        {
            var xDistance = entity1.Center.X - x;
            var yDistance = entity1.Center.Y - y;
            return Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));
        }

        public bool IsTankInBounds(ITank tank, double newX, double newY, IGameState gameState)
        {
            return newX + tank.Size.Width < gameState.Size.Width && newX > 0 &&
                   newY + tank.Size.Height < gameState.Size.Height && newY > 0;
        }

        public bool CanReach(IEntity shooter, Bullet bullet, ILocation target)
        {
            var xDistance = shooter.X + (shooter.Size.Width / 2) - (target.X);
            var yDistance = shooter.Y + (shooter.Size.Height / 2) - (target.Y);
            var shooterSize = (shooter.Size.Width + shooter.Size.Height) / 2;
            var distance = bullet.Range + shooterSize - Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));
            return target != null && (distance > 0);
        }

        private static GameState GenerateGameState(double height, double width, Game game)
        {
            return new GameState
            {
                Size = new Size { Height = height, Width = width },
                Foes = new List<IFoe>(),
                Goals = new List<IGoal>
                {
                    new Goal {Location = new Location(0,0)},
                    new Goal {Location = new Location(width - Goal.Width, 0)},
                    new Goal {Location = new Location(0, height - Goal.Height)},
                    new Goal {Location = new Location(width - Goal.Width, height - Goal.Height)}
                },
                GameTanks = game.Players.SelectMany(player => player.Tanks.Select(tank => (IGameTank)new GameTank(tank, player.Name))).ToList(),
                GravityEntities = new List<IGravityEntity>(),
                Wave = 0,
                Lost = false
            };
        }

        public void ClearGameOut(IGameBroadcaster gameBroadcaster)
        {
            Players.Clear();
            _thread.Abort(gameBroadcaster);
        }

        public void NewWave()
        {
            GameState.Wave++;
            FoeCount++;
            FoesToSpawn = FoeCount;
        }
    }
}