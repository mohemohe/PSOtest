using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PSOtest.Types;



namespace PSOtest
{
	class Program
	{
		private const int Particles = 100000; 
		private const int MaxLoop = Int32.MaxValue;
		private const double StageWidth = 256.0;
		private const double StageHeight = StageWidth;

		private static readonly Random _random = new Random();
		private static readonly List<Particle> _particleList = new List<Particle>();
		private static Position _globalBestPosition { get; set; }

		static void Main(string[] args)
		{
			//// start debug
			//var test = 0.00000001053928425;
			//Console.WriteLine(CalcFitness(new Position(test, test)));
			//Console.ReadLine();
			//// end debug
			


			// 初期化 
			// リストの排他制御がそれなりに重いので粒子が少ない場合はシングルスレッドで初期化する
			if (Particles <= 100000)
			{
				for (var i = 0; i < Particles; i++)
				{
					_particleList.Add(InitializeParticle());
					UpdateFitness(ref _particleList[i].CurrentFittness, _particleList[i].CurrentPosition);
				}
			}
			else
			{
				Parallel.For(0, Particles, i =>
				{
					lock (_particleList) _particleList.Add(InitializeParticle());
				});
				Parallel.For(0, Particles, i =>
				{
					UpdateFitness(ref _particleList[i].CurrentFittness, _particleList[i].CurrentPosition);
				});
			}

			var minFitnessIndex = 0;
			for (var i = 0; i < _particleList.Count; i++)
			{
				if (_particleList[i].CurrentFittness < _particleList[minFitnessIndex].CurrentFittness)
				{
					minFitnessIndex = i;
				}
			}
			_globalBestPosition = _particleList[minFitnessIndex].CurrentPosition;

			var sw = new Stopwatch();
			sw.Start();

			for (var i = 0; i < MaxLoop; i++)
			{
				if (i % 100 == 0)
				{
					Console.Clear();
					OutputConsole(i, sw.Elapsed);
				}

				if (CalcFitness(_globalBestPosition) < Double.Epsilon)
				{
					sw.Stop();

					Console.Clear();
					OutputConsole(i, sw.Elapsed);

					return;
				}

				PSO();
			}
		}

		static void OutputConsole(int loop, TimeSpan elapsedTime)
		{
			var str = new StringBuilder();
			str.AppendLine("現在の最適なX座標" + "\t\t" + 
						   "現在の最適なY座標" + "\t\t" +
						   "評価関数の出力");
			str.AppendLine(_globalBestPosition.X.ToString().PadRight(20) + "\t\t" +
						   _globalBestPosition.Y.ToString().PadRight(20) + "\t\t" +
						   CalcFitness(_globalBestPosition).ToString().PadRight(20));
			str.AppendLine();
			str.AppendLine("粒子数\t\t" + Particles);
			str.AppendLine("ループ回数\t" + loop);
			str.AppendLine("経過時間\t" + elapsedTime);

			Console.WriteLine(str);
		}

		static void PSO()
		{
			Parallel.For(0, _particleList.Count, i =>
			{
				UpdatePosition(ref _particleList[i].CurrentPosition, _particleList[i].Velocity);
				UpdateFitness(ref _particleList[i].CurrentFittness, _particleList[i].CurrentPosition);
			});

			var minFitnessIndex = 0;
			for (var i = 0; i < _particleList.Count; i++)
			{
				if (_particleList[i].CurrentFittness < _particleList[minFitnessIndex].CurrentFittness)
				{
					minFitnessIndex = i;
				}
			}

			if (_particleList[minFitnessIndex].CurrentFittness < CalcFitness(_globalBestPosition))
			{
				_globalBestPosition = _particleList[minFitnessIndex].CurrentPosition;
			}

			Parallel.For(0, _particleList.Count, i =>
			{
				UpdateVelocityMT(ref _particleList[i].Velocity,
									 _particleList[i].CurrentPosition,
									 _particleList[i].PersonalBestPosition,
									 _globalBestPosition);
			});
		}

		
		static Particle InitializeParticle()
		{
			var position = new Position(
				(_random.NextDouble() - 0.5) * 2 * StageWidth,
				(_random.NextDouble() - 0.5) * 2 * StageHeight
			);
			var velocity = new Velocity(
				_random.NextDouble(),
				_random.NextDouble()
			);

			return new Particle(position, velocity);
		}

		static double CalcFitness(Position position)
		{
			const int type = 0;

			switch (type)
			{
				case 1:
					return TestFunctions.Griewank(new double[] { position.X, position.Y });
				case 2:
					return TestFunctions.Rosenbrock(new double[] { position.X, position.Y });
				default:
					return TestFunctions.Test00(new double[] {position.X, position.Y});
			}
		}

		static void UpdateFitness(ref double currentFittness, Position position)
		{
			currentFittness = CalcFitness(position);
		}

		static void UpdatePosition(ref Position position, Velocity velocity)
		{
			position.X += velocity.X;
			position.Y += velocity.Y;
		}

		static void UpdateVelocity(ref Velocity velocity, Position currentPosition, Position personalBestPosition, Position globalBestPosition)
		{
			var randX = _random.NextDouble();
			var randY = _random.NextDouble();
			velocity.X = 0.8 * velocity.X +
						 2.0 * randX * (personalBestPosition.X - currentPosition.X) +
						 2.0 * randY * (globalBestPosition.X - currentPosition.X);
			
			velocity.Y = 0.8 * velocity.Y +
						 2.0 * randX * (personalBestPosition.Y - currentPosition.Y) +
						 2.0 * randY * (globalBestPosition.Y - currentPosition.Y);

		}

		static void UpdateVelocityMT(ref Velocity velocity, Position currentPosition, Position personalBestPosition, Position globalBestPosition)
		{
			var rand = ThreadSafeRandom.Random();
			var randX = rand.NextDouble();
			var randY = rand.NextDouble();

			velocity.X = 0.8 * velocity.X +
						 2.0 * randX * (personalBestPosition.X - currentPosition.X) +
						 2.0 * randY * (globalBestPosition.X - currentPosition.X);

			velocity.Y = 0.8 * velocity.Y +
						 2.0 * randX * (personalBestPosition.Y - currentPosition.Y) +
						 2.0 * randY * (globalBestPosition.Y - currentPosition.Y);
		}
	}
}
