using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public class GameThread
    {
        private readonly Game _game;

        public GameThread(Game game)
        {
            _game = game;
        }

        public void Run(object caller)
        {
            var gameState = _game.GameState;

            while (true)
            {
                _game.GameBroadcaster.BroadcastGameState(gameState);

                SpawnFoes(gameState);

                for (int j = 0; j < gameState.Foes.Count; j++)
                {
                    var monster = (Monster)gameState.Foes[j];
                    monster.Update(gameState);

                    //Thaw out monsters
                    monster.Speed = (monster.Speed * 9999 + monster.MaxSpeed) / 10000.0;

                    if (MonsterAtGoal(monster))
                    {
                        j--;
                    }
                }

                foreach (var gameTank in gameState.GameTanks)
                {
                    var tank = gameTank.Tank;
                    var tankUpdate = tank.Update(gameState);

                    MoveTank(tankUpdate, tank);
                    DoTankAttack(gameTank, tankUpdate, tank);
                }
                Thread.Sleep(10);
            }
        }

        private void SpawnFoes(GameState gameState)
        {
            CheckForNewWave();

            int foesToSpawnLog = _game.FoesToSpawn;
            while (foesToSpawnLog > 0)
            {
                if (_game.GameState.Wave % 2 == 0)
                {
                    _game.FoesToSpawn = 0;
                    BossMonster m = new BossMonster(foesToSpawnLog * (int)(_game.MonsterStartHealth * Math.Pow(1.1, gameState.Wave) + 1))
                    {
                        Location =
                            new Location(gameState.Size.Width / 2 - BossMonster.Width / 2.0,
                                gameState.Size.Height / 2 - BossMonster.Height / 2.0),
                    };
                    foesToSpawnLog = 0;
                    gameState.Foes.Add(m);
                }
                else
                {
                    _game.FoesToSpawn--;
                    foesToSpawnLog /= 10;
                    Monster m = new Monster((int)(_game.MonsterStartHealth * Math.Pow(1.1, gameState.Wave) + 1))
                    {
                        Location =
                            new Location(gameState.Size.Width / 2 - Monster.Width / 2.0,
                                gameState.Size.Height / 2 - Monster.Height / 2.0),
                    };
                    gameState.Foes.Add(m);
                }
            }
        }

        private void DoTankAttack(IGameTank gameTank, TankUpdate tankUpdate, Tank tank)
        {
            gameTank.ShotTarget = tankUpdate.ShotTarget;
            gameTank.Shooting = false;

            if (gameTank.Heat <= 0 && gameTank.ShotTarget != null)
            {
                var bullet = (Bullet)tank.GetBullet();
                if (_game.CanReach(tank, bullet, gameTank.ShotTarget))
                {
                    gameTank.Shooting = true;
                    gameTank.Bullet = bullet;
                    gameTank.Heat += bullet.ReloadTime;

                    List<Monster> foesInRange = _game.GetFoesInRange(tankUpdate.ShotTarget.X, tankUpdate.ShotTarget.Y, bullet.SplashRange);

                    ApplyDamage(gameTank, bullet, foesInRange);
                    ApplyFreeze(gameTank, bullet, foesInRange);
                }
            }
            else
            {
                gameTank.Heat = Math.Max(gameTank.Heat - 1, 0);
            }
        }

        private void ApplyDamage(IGameTank gameTank, Bullet bullet, List<Monster> foesInRange)
        {
            foreach (var monster in foesInRange)
            {
                monster.Health -= bullet.Damage;

                if (monster.Health <= 0)
                {
                    KilledMonster(gameTank, monster);
                }
            }
        }

        private void KilledMonster(IGameTank gameTank, Monster monster)
        {
            var gameState = _game.GameState;
            gameState.Foes.Remove(monster);
            if (!gameState.Lost)
            {
                gameTank.Killed++;
            }
        }

        private void CheckForNewWave()
        {
            var gameState = _game.GameState;
            if (!gameState.Foes.Any())
            {
                _game.NewWave();
            }
        }

        private static void ApplyFreeze(IGameTank gameTank, Bullet bullet, List<Monster> foesInRange)
        {
            foreach (var monster in foesInRange)
            {
                monster.Speed *= monster.Health / (double)(bullet.Freeze * 3 + monster.Health);
            }
        }

        private void MoveTank(TankUpdate tankUpdate, Tank tank)
        {
            var gameState = _game.GameState;
            if (tankUpdate.MovementTarget != null)
            {
                var V = new Vector(tankUpdate.MovementTarget.X - tank.X, tankUpdate.MovementTarget.Y - tank.Y);
                var angle = Math.Atan2(V.Y, V.X);
                var speed = Math.Min(tank.Speed, Math.Sqrt(V.X * V.X + V.Y * V.Y));
                var xMovement = speed * Math.Cos(angle);
                var yMovement = speed * Math.Sin(angle);

                if (_game.IsTankInBounds(tank, tank.X + xMovement, tank.Y, gameState))
                {
                    ((Location)tank.Location).X += xMovement;
                }
                else
                {
                    V.X /= 2;
                }

                if (_game.IsTankInBounds(tank, tank.X, tank.Y + yMovement, gameState))
                {
                    ((Location)tank.Location).Y += yMovement;
                }
                else
                {
                    V.Y /= 2;
                }
            }
        }

        private bool MonsterAtGoal(IMonster monster)
        {
            var gameState = _game.GameState;
            var goal = _game.IsMonsterAtGoal(monster, gameState.Goals);
            if (goal != null)
            {
                if (monster is BossMonster)
                {
                    ((Monster)monster).Health /= 2;
                }
                else
                {
                    ((Monster)monster).Health = 0;
                    gameState.Foes.Remove(monster);
                }
                ((Goal)goal).Health -= 1;
                if (goal.Health <= 0)
                {
                    gameState.Goals.Remove(goal);
                    _game.MonsterStartHealth = (int)(_game.MonsterStartHealth * 1.25);

                    if (!gameState.Goals.Any())
                    {
                        gameState.Lost = true;
                    }
                }

                return true;
            }

            return false;
        }
    }
}