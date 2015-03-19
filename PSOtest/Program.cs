using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSOtest
{
    class Program
    {
        class Particle
        {
            Position position;
            Velocity velocity;

            public Particle(Position position, Velocity velocity)
            {
                this.position = position;
                this.velocity = velocity;
            }
        }
        
        const double _maxSpeed = 2.0;
        const double _minSpeed = -maxSpeed;
        const int _maxLoop = 100;
        const double _moment = 0.8;

        const double stageWidth = 10.0;
        const double stageHeight = 10.0;

        struct Position
        {
            public double x;
            public double y;

            public Position(double x, double y)
            {
                this.x = x;
                this.y = y;
            } 
        }

        struct Velocity
        {
            public double x;
            public double y;

            public Velocity(double x, double y)
            {
                this.x = x;
                this.y = y;
            } 
        }

        static void Main(string[] args)
        {
            // init
            var particleList = new List<Particle>();
            for (var i = 0; i < 100; i++)
            {
                particleList.Add(InitParticle());
            }


        }

        static void PSO()
        {
            
        }

        static Particle InitParticle()
        {
            var rand = new Random();
            Position position = Position(
                rand.Next(stageWidth * 100) / 100 * 2 - stageWidth / 2,
                rand.Next(stageHeight * 100) / 100 * 2 - stageHeight / 2
            );
            Velocity velocity = Velocity(
                rand.NextDouble() * 2,
                rand.NextDouble() * 2
            );

            return new Particle{position = position, velocity = velocity};
        }

        static void updatePosition(ref Position position, Velocity velocity)
        {
            pos.x += velocity.x;
            pos.y += velocity.y;
        }

        static void updateVelocity(ref Velocity velocity, Position currentPosition, Position localBestPosition, Position globalBestPosition)
        {
            var rand = new Random();
            velocity.x = _moment * velocity + 
                          2 * rand.NextDouble() * (localBestPosition.x - currentPosition.x) + 
                          2 * rand.NextDouble() * (globalBestPosition.x - currentPosition.x);
            velocity.y = _moment * velocity +
                          2 * rand.NextDouble() * (localBestPosition.y - currentPosition.y) +
                          2 * rand.NextDouble() * (globalBestPosition.y - currentPosition.y);
        }
    }
}
