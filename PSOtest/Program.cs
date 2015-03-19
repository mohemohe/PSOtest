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
            public Position currentPosition;
            public Position personalBestPosition;
            public Velocity velocity;

            public Particle(Position position, Velocity velocity)
            {
                this.currentPosition = position;
                this.personalBestPosition = position;
                this.velocity = velocity;
            }
        }

        class Position
        {
            public double x;
            public double y;

            public Position(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
        }

        class Velocity
        {
            public double x;
            public double y;

            public Velocity(double x, double y)
            {
                this.x = x;
                this.y = y;
            }
        }

        const double _maxSpeed = 2.0;
        const double _minSpeed = -_maxSpeed;
        const int _maxLoop = 100;
        const double _moment = 0.8;

        const double _stageWidth = 10.0;
        const double _stageHeight = 10.0;

        static Position _globalBestPosition;

        static void Main(string[] args)
        {
            // 初期化
            var particleList = new List<Particle>();
            for (var i = 0; i < 100; i++)
            {
                particleList.Add(InitParticle());
            }

            // 最初の全体のベスポジを設定
            _globalBestPosition = particleList[0].currentPosition;
            foreach (var particle in particleList)
            {
                if (CalcFitness(particle.currentPosition) < CalcFitness(_globalBestPosition))
                {
                    _globalBestPosition = particle.currentPosition;
                }
            }


        }

        static void PSO()
        {
            
        }

        static Particle InitializeParticle()
        {
            var rand = new Random();
            Position position = new Position(
                rand.Next((int)_stageWidth * 100) / 100 * 2 - _stageWidth / 2,
                rand.Next((int)_stageHeight * 100) / 100 * 2 - _stageHeight / 2
            );
            Velocity velocity = new Velocity(
                rand.NextDouble() * 2,
                rand.NextDouble() * 2
            );

            return new Particle(position, velocity);
        }

        static double CalcFitness(Position position)
        {
            return Math.Pow(position.x + position.y, 2);
        }

        static void updatePosition(ref Position position, Velocity velocity)
        {
            position.x += velocity.x;
            position.y += velocity.y;
        }

        static void updateVelocity(ref Velocity velocity, Position currentPosition, Position personalBestPosition, Position globalBestPosition)
        {
            var rand = new Random();
            velocity.x = _moment * velocity.x + 
                          2 * rand.NextDouble() * (personalBestPosition.x - currentPosition.x) + 
                          2 * rand.NextDouble() * (globalBestPosition.x - currentPosition.x);
            velocity.y = _moment * velocity.y +
                          2 * rand.NextDouble() * (personalBestPosition.y - currentPosition.y) +
                          2 * rand.NextDouble() * (globalBestPosition.y - currentPosition.y);
        }
    }
}
