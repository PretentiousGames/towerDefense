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
                    Tanks = new List<Tank> { new TestTank(), new FreezeTank(), new BoomTank() }
                });
            }

            GameState = GenerateGameState(DefaultSize.Height, DefaultSize.Width, this);

            NewWave();
            MonsterStartHealth = 10;
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
                var totalWidth = radius + foe.Size.Width;
                var totalHeight = radius + foe.Size.Height;
                Rectangle rect = new Rectangle(x - totalWidth / 2, y - totalHeight / 2, totalWidth, totalHeight);

                if (rect.Contains(foe.Center.X, foe.Center.Y))
                {
                    foesInRange.Add((Monster)foe);
                }
            }

            return foesInRange;
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
                Wave = 0,
                Lost = false
            };
        }

        public IGoal IsMonsterAtGoal(IFoe monster, List<IGoal> goals)
        {
            foreach (var goal in goals)
            {
                if (((monster.X - monster.Size.Width / 2) > goal.X) && (monster.X + monster.Size.Width / 2 < (goal.X + goal.Size.Width)) &&
                    ((monster.Y - monster.Size.Height / 2) > goal.Y) && (monster.Y + monster.Size.Height / 2 < (goal.Y + goal.Size.Height)))
                {
                    return goal;
                }
            }

            return null;
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