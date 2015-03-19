using System;
using System.Threading;
using System.Collections.Generic;

namespace PSOtest
{
	internal class Program
	{
		private class Particle
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

		private class Position
		{
			public double x;
			public double y;

			public Position(double x, double y)
			{
				this.x = x;
				this.y = y;
			}
		}

		private class Velocity
		{
			public double x;
			public double y;

			public Velocity(double x, double y)
			{
				this.x = x;
				this.y = y;
			}
		}

		private const int _maxLoop = 200000;

		private const double _stageWidth = 10.0;
		private const double _stageHeight = 10.0;

		private static double _maxFitness = Double.MinValue;

		private static Position _globalBestPosition;
		private static List<Particle> _particleList;
        private static List<double> _fitnessList;

		private static void Main(string[] args)
		{
			// 初期化
			_particleList = new List<Particle>();
            _fitnessList = new List<double>();
			for (var i = 0; i < 100; i++)
			{
				_particleList.Add(InitializeParticle());
                _fitnessList.Add(Double.MinValue);
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
				if (i % 1 == 0)
				{
                    Console.Clear();
					//Console.WriteLine(firstBestPosition.x + ", " + firstBestPosition.y);
                    Console.WriteLine(i + " / " + _maxLoop);
					Console.WriteLine(_globalBestPosition.x + ",\t" + _globalBestPosition.y + ",\t" + CalcFitness(_globalBestPosition) + ",\t" + _maxFitness);
                    Thread.Sleep(500);
                }

				PSO();

			}

			//Console.Clear();
			//Console.WriteLine(firstBestPosition.x + ", " + firstBestPosition.y);
			Console.WriteLine(_globalBestPosition.x + ",\t" + _globalBestPosition.y + ",\t" + CalcFitness(_globalBestPosition));
		}

		private static void PSO()
		{
			for (var i = 0; i < _particleList.ToArray().Length; i++)
			{
				UpdatePosition(ref _particleList[i].currentPosition, _particleList[i].velocity);
			}

			for (var i = 0; i < _particleList.ToArray().Length; i++)
			{
				if (Griewank(_particleList[i]) > _maxFitness)
				{
					_particleList[i].personalBestPosition = _particleList[i].currentPosition;
                    _globalBestPosition = _particleList[i].currentPosition;
                    _maxFitness = Griewank(_particleList[i]);
				}
			}

            //foreach (var particle in _particleList)
            //{
            //    if (CalcFitness(particle.currentPosition) < CalcFitness(_globalBestPosition))
            //    {
            //        _globalBestPosition = particle.currentPosition;
            //    }
            //}

			for (var i = 0; i < _particleList.ToArray().Length; i++)
			{
				UpdateVelocity(ref _particleList[i].velocity,
							   _particleList[i].currentPosition,
							   _particleList[i].personalBestPosition,
							   _globalBestPosition);
			}
		}

		static double Griewank(Particle particle)
		{
			var vals = new double[] { particle.currentPosition.x, particle.currentPosition.y };
			var num = vals.Length;

			return -_Griewank(vals, num);
		}

		static double _Griewank(double[] Vals, int Num)
		{
			double sum = 1.0, val = 1.0;

			for (int i = 0; i < Num; i++)
			{
				//double Val = (double)i * 0.1357 / (double)Num + Vals[i];

                double Val = Vals[i];

				sum += (Val * Val / 4000.0);
				val *= Math.Cos(Val / (double)(i + 1));
			}

			return sum - val;
		}

		private static Random rand = new Random();

		private static Particle InitializeParticle()
		{
			Position position = new Position(
				rand.NextDouble() * _stageWidth * 2 - _stageWidth,
				rand.NextDouble() * _stageHeight * 2 - _stageHeight
			);
			Velocity velocity = new Velocity(
				rand.NextDouble(),
				rand.NextDouble()
			);

			return new Particle(position, velocity);
		}

		private static double CalcFitness(Position position)
		{
            return Math.Abs(position.x) + Math.Abs(position.y);
		}

		private static void UpdatePosition(ref Position position, Velocity velocity)
		{
			position.x += velocity.x;
			position.y += velocity.y;
		}

		private static void UpdateVelocity(ref Velocity velocity, Position currentPosition, Position personalBestPosition, Position globalBestPosition)
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