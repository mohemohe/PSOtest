namespace PSOtest.Types
{
    class TwoDimensions
    {
        public double X { get; set; }
        public double Y { get; set; }

        public int Count
        {
            get { return 2; }
        }

        public TwoDimensions(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
