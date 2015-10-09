namespace TowerDefense.Interfaces
{
    public abstract class Tank : ITank
    {
        private static int _id = 0;

        public Tank(double x, double y)
        {
            Id = _id++;
            Size = new Size(32);
            X = x;
            Y = y;
        }
        public Size Size { get; private set; }
        public int Id { get; private set; }
        public double X { get; set; }
        public double Y { get; set; }

        public abstract string Name { get; }
        public abstract TankUpdate Update(IGameState gameState);
        public abstract IBullet GetBullet();
    }
}