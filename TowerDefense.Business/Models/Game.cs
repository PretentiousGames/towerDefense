using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class Game
    {
        private Thread Thread;

        public Game()
        {
            Players = new List<Player>();
            Size = DefaultSize;
        }

        public static Size DefaultSize = new Size(800, 800);
        private int _foeCount;

        public string Name { get; set; }
        public List<Player> Players { get; set; }
        public Size Size { get; set; }
        public IGameBroadcaster GameBroadcaster { get; set; }

        public void StartNewGame(IGameBroadcaster gameBroadcaster)
        {
            if (Thread != null && Thread.IsAlive)
            {
                Thread.Abort(gameBroadcaster);
            }
            Thread = new Thread(ThreadStart);
            _foeCount = 10;
            GameBroadcaster = gameBroadcaster;
            Thread.Start(this);
        }

        private static void ThreadStart(object gameObj)
        {
            var game = (Game)gameObj;
            //Random r = new Random();

            var height = DefaultSize.Height;
            var width = DefaultSize.Width;
            var gameState = new GameState
            {
                Size = new Size { Height = height, Width = width },
                Foes = new List<IFoe>(),
                Goals = new List<IGoal>
                {
                    new Goal {X = 0, Y = 0},
                    new Goal {X = width - Goal.Width, Y = 0},
                    new Goal {X = 0, Y = height - Goal.Height},
                    new Goal {X = width - Goal.Width, Y = height - Goal.Height}
                },
                
            };

            while (true)
            {
                if (gameState.Foes.Count < game._foeCount)
                {
                    Monster m = new Monster { X = 400 - 8, Y = 400 - 8 };
                    gameState.Foes.Add(m);
                }

                game.GameBroadcaster.BroadcastGameState(gameState);
                for (int j = 0; j < gameState.Foes.Count; j++)
                {
                    var monster = (IMonster)gameState.Foes[j];
                    monster.Update(gameState);
                    var goal = IsMonsterAtGoal(monster, gameState.Goals);
                    if (goal != null)
                    {
                        gameState.Foes.Remove(monster);
                        ((Goal)goal).Health -= 1;
                        if (goal.Health <= 0)
                        {
                            gameState.Goals.Remove(goal);
                        }
                        j--;
                    }
                }
                Thread.Sleep(10);
            }
        }
        
        public static IGoal IsMonsterAtGoal(IFoe monster, List<IGoal> goals)
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
    }
}