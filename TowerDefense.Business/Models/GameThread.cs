using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using TowerDefense.Interfaces;
using Size = TowerDefense.Interfaces.Size;

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
                bool gameOver = gameState.Lost;

                _game.GameBroadcaster.BroadcastGameState(gameState);
                gameState.Foes.RemoveAll(foe => foe.Health == 0);
                SpawnFoes(gameState);
                UpdateGravityEntities(gameState);
                UpdateAllMonsters(gameState);
                UpdateAllTanks(gameState);
                KillDeadGoals(gameState);
                Thread.Sleep(1);

                if (gameOver)
                {
                    break;
                }
            }
        }

        private void KillDeadGoals(GameState gameState)
        {
            var deadGoals = gameState.Goals.Where(goal => goal.Health <= 0);

            gameState.Goals.RemoveAll(goal => deadGoals.Contains(goal));
            _game.MonsterStartHealth = (int)(_game.MonsterStartHealth * Math.Pow(1.25, deadGoals.Count()));

            if (!gameState.Goals.Any())
            {
                gameState.Lost = true;
            }
        }

        private void UpdateAllTanks(GameState gameState)
        {
            foreach (var gameTank in gameState.GameTanks)
            {
                var tank = gameTank.Tank;
                var tankUpdate = tank.Update(gameState);

                gameTank.TankColor = tankUpdate.TankColor;
                MoveTank(gameTank, tankUpdate, tank);
                DoTankAttack(gameTank, tankUpdate, tank);
            }
            var top = gameState.GameTanks[0];
            gameState.GameTanks.RemoveAt(0);
            gameState.GameTanks.Add(top);
        }

        private void UpdateAllMonsters(GameState gameState)
        {
            foreach (var foe in gameState.Foes)
            {
                var monster = (Monster)foe;
                monster.Update(gameState);

                //Thaw out monsters
                monster.Speed = (monster.Speed * 9999 + monster.MaxSpeed) / 10000.0;
            }
        }

        private void UpdateGravityEntities(GameState gameState)
        {
            // Update existing gravity bullets
            foreach (GravityEntity gravityEntity in gameState.GravityEntities)
            {
                gravityEntity.Duration -= .01;
            }

            gameState.GravityEntities.RemoveAll(x => x.Duration <= 0);

            // Add new gravity bullets
            gameState.GameTanks.ForEach((gameTank) =>
            {
                if (gameTank.Shooting && gameTank.Bullet.GravityDuration > 0)
                {
                    gameState.GravityEntities.Add(new GravityEntity
                    {
                        Duration = gameTank.Bullet.GravityDuration,
                        Size = new Size(1, 1),
                        X = gameTank.ShotTarget.X,
                        Y = gameTank.ShotTarget.Y,
                        Strength = gameTank.Bullet.GravityStrength
                    });
                }
            });
        }

        private void SpawnFoes(GameState gameState)
        {
            CheckForNewWave();

            int foesToSpawnLog = _game.FoesToSpawn;
            while (foesToSpawnLog > 0)
            {
                if (_game.GameState.Wave % 5 == 0)
                {
                    _game.FoesToSpawn = 0;

                    AbilityType type;
                    int rand = new Random().Next(1, 20);

                    if (rand == 1)
                    {
                        type = AbilityType.Healing;
                    }
                    else if (rand == 2)
                    {
                        type = AbilityType.Splitter;
                    }
                    else if (rand == 3)
                    {
                        type = AbilityType.RangedHeat;
                    }
                    else
                    {
                        type = AbilityType.Kamakaze;
                    }

                    BossMonster m =
                        new BossMonster(foesToSpawnLog *
                                        (int)(_game.MonsterStartHealth * Math.Pow(1.1, gameState.Wave) + 1), type);
                    m.Location = new Location(gameState.Size.Width / 2 - m.Size.Width / 2.0,
                                gameState.Size.Height / 2 - m.Size.Height / 2.0);

                    foesToSpawnLog = 0;
                    gameState.Foes.Add(m);
                }
                else
                {
                    _game.FoesToSpawn--;
                    foesToSpawnLog /= 10;

                    AbilityType type;
                    int rand = new Random().Next(1, 20);

                    if (rand == 1)
                    {
                        type = AbilityType.Healing;
                    }
                    else if (rand == 2)
                    {
                        type = AbilityType.Splitter;
                    }
                    else if (rand == 3)
                    {
                        type = AbilityType.RangedHeat;
                    }
                    else
                    {
                        type = AbilityType.Kamakaze;
                    }

                    Monster m = new Monster((int)(_game.MonsterStartHealth * Math.Pow(1.1, gameState.Wave) + 1), type)
                    {
                        Location = new Location(gameState.Size.Width / 2 - Monster.Width / 2.0,
                            gameState.Size.Height / 2 - Monster.Height / 2.0)
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
                try
                {
                    var bullet = (Bullet) tankUpdate.Bullet;
                    
                    gameTank.Shooting = true;
                    gameTank.Shots++;
                    gameTank.Bullet = bullet;
                    gameTank.Heat += bullet.GetReloadTime(Game.GetDistance(tank.Center.X, tank.Center.Y, tankUpdate.ShotTarget.X, tankUpdate.ShotTarget.Y));
                    
                    List<Monster> foesInRange = _game.GetFoesInRange(tankUpdate.ShotTarget.X,
                        tankUpdate.ShotTarget.Y, bullet.SplashRange);

                    ApplyDamage(gameTank, bullet, foesInRange);
                    ApplyFreeze(gameTank, bullet, foesInRange);
                }
                catch
                {
                    //They made a bad bullet, bad tank!
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
                monster.OnHitAbility?.Invoke(_game.GameState, bullet.Damage);
                gameTank.Damage += bullet.Damage;
                gameTank.MaxDamageDealt = Math.Max(bullet.Damage, gameTank.MaxDamageDealt);

                if (monster.Health <= 0)
                {
                    KilledMonster(gameTank, monster);
                }
            }
        }

        private void KilledMonster(IGameTank gameTank, Monster monster)
        {
            var gameState = _game.GameState;

            monster.OnDeathAbility?.Invoke(gameState);

            gameState.Foes.Remove(monster);
            if (!gameState.Lost)
            {
                gameTank.Killed++;
                if (monster is BossMonster)
                {
                    gameTank.BossesKilled++;
                }
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

                gameTank.Freeze += bullet.Freeze;
            }
        }

        private void MoveTank(IGameTank gameTank, TankUpdate tankUpdate, Tank tank)
        {
            var gameState = _game.GameState;
            if (tankUpdate.MovementTarget != null)
            {
                gameTank.MovementTarget = tankUpdate.MovementTarget;

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
    }
}