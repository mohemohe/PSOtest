namespace PSOtest.Types
{
    class Particle
    {
        public Position CurrentPosition;
        public Position PersonalBestPosition;
        public Velocity Velocity;
        public double CurrentFittness;

        public Particle(Position position, Velocity velocity)
        {
            this.CurrentPosition = position;
            this.PersonalBestPosition = position;
            this.Velocity = velocity;
        }
    }
}
