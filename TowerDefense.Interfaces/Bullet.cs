namespace TowerDefense.Interfaces
{
    public class Bullet
    {
        public double Range { get; set; }
        public double Damage { get; set; }
        public double ReloadTime
        {
            get { return Range * Damage; }
        }
    }
}