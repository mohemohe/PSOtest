using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        const int _maxLoop = 100000;

        const double _stageWidth = 10.0;
        const double _stageHeight = 10.0;

        static Position _globalBestPosition;
        static List<Particle> _particleList;

        static void Main(string[] args)
        {
            // 初期化
            _particleList = new List<Particle>();
            for (var i = 0; i < 100; i++)
            {
                _particleList.Add(InitializeParticle());
            }

            // 最初の全体のベスポジを設定
            _globalBestPosition = _particleList[0].currentPosition;
            foreach (var particle in _particleList)
            {
                if (CalcFitness(particle.currentPosition) < CalcFitness(_globalBestPosition))
                {
                    _globalBestPosition = particle.currentPosition;
                }
            }

            var firstBestPosition = _globalBestPosition;
            
            for (var i = 0; i < _maxLoop; i++)
            {
                if (i % 100 == 0)
                {
                    //Console.Clear();
                    //Console.WriteLine(firstBestPosition.x + ", " + firstBestPosition.y);
                    //Console.WriteLine(i + " / " + _maxLoop);
                    Console.WriteLine(_globalBestPosition.x + ", " + _globalBestPosition.y + ", " + CalcFitness(_globalBestPosition));
                    //Thread.Sleep(500);
                }
                PSO();
            }

            //Console.Clear();
            //Console.WriteLine(firstBestPosition.x + ", " + firstBestPosition.y);
            Console.WriteLine(_globalBestPosition.x + ", " + _globalBestPosition.y);
        }

        static void PSO()
        {
            for (var i = 0; i < _particleList.ToArray().Length; i++)
            {
                UpdatePosition(ref _particleList[i].currentPosition, _particleList[i].velocity);
            }

            for (var i = 0; i < _particleList.ToArray().Length; i++)
            {
                if (CalcFitness(_particleList[i].currentPosition) < CalcFitness(_particleList[i].personalBestPosition))
                {
                    _particleList[i].personalBestPosition = _particleList[i].currentPosition;
                }
            }

            foreach (var particle in _particleList)
            {
                if (CalcFitness(particle.currentPosition) < CalcFitness(_globalBestPosition))
                {
                    _globalBestPosition = particle.currentPosition;
                }
            }

            for (var i = 0; i < _particleList.ToArray().Length; i++)
            {
                UpdateVelocity(ref _particleList[i].velocity, 
                               _particleList[i].currentPosition, 
                               _particleList[i].personalBestPosition, 
                               _globalBestPosition);
            }
        }

        static Random rand = new Random();
        static Particle InitializeParticle()
        {
            
            Position position = new Position(
                rand.NextDouble() * _stageWidth,
                rand.NextDouble() * _stageHeight
            );
            Velocity velocity = new Velocity(
                rand.NextDouble(),
                rand.NextDouble()
            );

            return new Particle(position, velocity);
        }

        static double CalcFitness(Position position)
        {
            return Math.Abs(position.x) + Math.Abs(position.y);
        }

        static void UpdatePosition(ref Position position, Velocity velocity)
        {
            position.x += velocity.x;
            position.y += velocity.y;
        }

        static void UpdateVelocity(ref Velocity velocity, Position currentPosition, Position personalBestPosition, Position globalBestPosition)
        {
            var randX = rand.NextDouble();
            var randY = rand.NextDouble();
            velocity.x = 0.8 * velocity.x +
                          2.0 * randX * (personalBestPosition.x - currentPosition.x) +
                          2.0 * randY * (globalBestPosition.x - currentPosition.x);
            
            velocity.y = 0.8 * velocity.y +
                          2.0 * randX * (personalBestPosition.y - currentPosition.y) +
                          2.0 * randY * (globalBestPosition.y - currentPosition.y);

        }
    }
}
