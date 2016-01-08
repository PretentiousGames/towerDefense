using TowerDefense.Interfaces;

namespace TowerDefense.Business.Models
{
    public interface IGameTank
    {
        double Heat { get; set; }
        string Owner { get; set; }
        Tank Tank { get; set; }
        ILocation Location { get; set; }
        ILocation ShotTarget { get; set; }
        ILocation MovementTarget { get; set; }
        bool Shooting { get; set; }
        Bullet Bullet { get; set; }
        int BossesKilled { get; set; }
        int Killed { get; set; }
        int Damage { get; set; }
        int Freeze { get; set; }
        int Shots { get; set; }
        int MaxDamageDealt { get; set; }
        string TankColor { get; set; }
    }
}