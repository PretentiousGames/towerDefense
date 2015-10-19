using System;
using System.Linq;
using TowerDefense.Business.Models;
using TowerDefense.Interfaces;

namespace TestTower
{
    public class BaseTank : Tank
    {
        public override string Name { get { return "Mr. Base Tank"; } }
        private IGoal _goalToDefend;
        private IFoe _targetFoe;

        // Set starting x/y location
        public BaseTank() : base(400, 400) {}
        
        public override TankUpdate Update(IGameState gameState)
        {
            // Pick a random goal to defend
            if (_goalToDefend == null && gameState.Goals.Count > 0)
            {
                var shuffledGoals = gameState.Goals.OrderBy(a => Guid.NewGuid());
                _goalToDefend = shuffledGoals.First();
            }
            
            TankUpdate tankUpdate = new TankUpdate();

            if (gameState.Foes.Any() && gameState.Goals.Any())
            {
                if (_targetFoe == null || _targetFoe.Health <= 0)
                {
                    // Pick a random target
                    var shuffledFoes = gameState.Foes.OrderBy(a => Guid.NewGuid());
                    _targetFoe = shuffledFoes.First();
                }
                
                if (_targetFoe != null)
                {
                    // Target the foe's current location
                    tankUpdate.ShotTarget = _targetFoe.Center;
                }

                // Move toward tower
                tankUpdate.MovementTarget = LocationProvider.GetLocation(_goalToDefend.X, _goalToDefend.Y);
            }

            return tankUpdate;
        }

        public override IBullet GetBullet()
        {
            // Create a bullet
            return new Bullet { Damage = 200, Range = 500, Freeze = 0, SplashRange = 0 };
        }
    }
}
