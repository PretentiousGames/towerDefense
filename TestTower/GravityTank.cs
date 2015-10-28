using System;
using System.Collections.Generic;
using System.Linq;
using TowerDefense.Business.Models;
using TowerDefense.Interfaces;

namespace TestTower
{
    public class GravityTank : Tank
    {
        private Random _rng = new Random();
        public Bullet Bullet { get; set; }
        public override string Name { get { return "Mr. Gravity"; } }
        private double _xTarget;
        private double _yTarget;

        public GravityTank()
            : base(200, 200)
        {
            _xTarget = 200;
            _yTarget = 200;
        }
        public override TankUpdate Update(IGameState gameState)
        {
            TankUpdate tankUpdate = new TankUpdate();

            if (gameState.Foes.Any() && gameState.Goals.Any())
            {
                double highestAverageDistance = 0;
                IGoal furthestGoal = null;

                gameState.Goals.ForEach(goal =>
                {
                    var average = gameState.Foes.Average(foe => GetDistanceToGoal(foe, new List<IGoal> {goal}));

                    if (average > highestAverageDistance)
                    {
                        highestAverageDistance = average;
                        furthestGoal = goal;
                    }
                });

                var target = furthestGoal;

                if (target != null)
                {
                    tankUpdate.ShotTarget = target.Center;
                    Bullet = new Bullet
                    {
                        Damage = 0,
                        Range = GetDistanceFromTank(target),
                        Freeze = 0,
                        SplashRange = 0,
                        Gravity = 2
                    };
                }
            }

            return tankUpdate;
        }

        public override IBullet GetBullet()
        {
            return Bullet;
        }
    }
}
