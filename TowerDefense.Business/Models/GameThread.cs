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

                if (gameState.Foes.Count < _game.FoeCount)
                {
                    Monster m = new Monster { Location = new Location(400 - 8, 400 - 8) };
                    gameState.Foes.Add(m);
                }

                for (int j = 0; j < gameState.Foes.Count; j++)
                {
                    var monster = (Monster)gameState.Foes[j];
                    monster.Update(gameState);

                    //Thaw out monsters
                    monster.Speed = (monster.Speed * 9999 + monster.MaxSpeed) / 10000.0;

                    if (MonsterAtGoal(monster, gameState))
                    {
                        j--;
                    }
                }

                foreach (var gameTank in gameState.GameTanks)
                {
                    var tank = gameTank.Tank;
                    var tankUpdate = tank.Update(gameState);

                    MoveTank(tankUpdate, tank, gameState);
                    DoTankAttack(gameTank, tankUpdate, tank, gameState);
                }
                Thread.Sleep(10);
            }
        }

        private void DoTankAttack(IGameTank gameTank, TankUpdate tankUpdate, Tank tank, GameState gameState)
        {
            gameTank.ShotTarget = tankUpdate.ShotTarget;
            gameTank.Shooting = false;

            if (gameTank.Heat <= 0 && gameTank.ShotTarget != null)
            {
                var bullet = (Bullet)tank.GetBullet();
                if (_game.CanReach(tank, bullet, gameTank.ShotTarget))
                {
                    gameTank.Shooting = true;
                    gameTank.Heat += bullet.ReloadTime;

                    var splash = bullet.Splash ?? new SplashBullet
                    {
                        Range = 1
                    };

                    List<Monster> foesInRange = _game.GetFoesInRange(tankUpdate.ShotTarget.X, tankUpdate.ShotTarget.Y, splash.Range);

                    ApplyDamage(gameTank, bullet, foesInRange, gameState);
                    ApplyFreeze(gameTank, bullet, foesInRange);
                }
            }
            else
            {
                gameTank.Heat = Math.Max(gameTank.Heat - 1, 0);
            }
        }

        private void ApplyDamage(IGameTank gameTank, Bullet bullet, List<Monster> foesInRange, GameState gameState)
        {
            foreach (var monster in foesInRange)
            {
                monster.Health -= bullet.Damage;

                if (monster.Health <= 0)
                {
                    gameState.Foes.Remove(monster);
                    if (!gameState.Lost)
                    {
                        gameTank.Killed++;
                        _game.Killed++;
                        if (_game.Killed == _game.FoeCount)
                        {
                            _game.Killed = 0;
                            gameState.Wave++;
                            _game.FoeCount++;
                            Monster.MonsterMaxHealth = (int)(Monster.MonsterMaxHealth * 1.1);
                        }
                    }
                }
            }
        }

        private static void ApplyFreeze(IGameTank gameTank, Bullet bullet, List<Monster> foesInRange)
        {
            foreach (var monster in foesInRange)
            {
                monster.Speed *= monster.MaxHealth / (double)(bullet.Freeze + monster.MaxHealth);
            }
        }

        private void MoveTank(TankUpdate tankUpdate, Tank tank, GameState gameState)
        {
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

        private bool MonsterAtGoal(IMonster monster, GameState gameState)
        {
            var goal = _game.IsMonsterAtGoal(monster, gameState.Goals);
            if (goal != null)
            {
                gameState.Foes.Remove(monster);
                ((Goal)goal).Health -= 1;
                if (goal.Health <= 0)
                {
                    gameState.Goals.Remove(goal);
                    Monster.MonsterMaxHealth = (int)(Monster.MonsterMaxHealth * 1.25);

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