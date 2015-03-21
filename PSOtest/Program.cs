using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
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
			public double currentFittness;

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

		const int _maxLoop = Int32.MaxValue;

		const double _stageWidth = 100.0;
		const double _stageHeight = 100.0;

		private static Position _globalBestPosition;
		private static List<Particle> _particleList;

		static void Main(string[] args)
		{
			//// start debug
			//var test = 0.00000001053928425;
			//Console.WriteLine(CalcFitness(new Position(test, test)));
			//Console.ReadLine();
			//// end debug
			


			// 初期化
			_particleList = new List<Particle>();
			for (var i = 0; i < 10000; i++)
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

			var sw = new Stopwatch();

			sw.Start();

			for (var i = 0; i < _maxLoop; i++)
			{
				if (i % 1000 == 0)
				{
					Console.Clear();
					//Console.WriteLine(firstBestPosition.x + ", " + firstBestPosition.y);
					OutputConsole(i, sw.Elapsed);
					//Thread.Sleep(500);
				}

				if (Double.Equals(CalcFitness(_globalBestPosition), 0.0))
				//if (CalcFitness(_globalBestPosition) <= Double.Epsilon)
				{
					sw.Stop();
					Console.Clear();
					OutputConsole(i, sw.Elapsed);
					break;
				}

				PSO();
			}

			//Console.Clear();
			//Console.WriteLine(_globalBestPosition.x + "\t\t" + _globalBestPosition.y + "\t\t" + CalcFitness(_globalBestPosition));
		}

		static void OutputConsole(int loop, TimeSpan elapsedTime)
		{
			Console.WriteLine("現在の最適なX座標" + "\t\t" + "現在の最適なY座標" + "\t\t" + "(0,0)との誤差");
			Console.WriteLine(_globalBestPosition.x + "\t\t" + _globalBestPosition.y + "\t\t" + CalcFitness(_globalBestPosition));
			Console.WriteLine();
			Console.WriteLine("ループ回数\t" + loop);
			Console.WriteLine("経過時間\t" + elapsedTime);
		}

		static void PSO()
		{

			Parallel.For(0, _particleList.Count, i =>
			{
				UpdatePosition(ref _particleList[i].currentPosition, _particleList[i].velocity);
				UpdateFitness(ref _particleList[i].currentFittness, ref _particleList[i].currentPosition);
			});

			var minFitnessIndex = 0;
			for (var i = 0; i < _particleList.Count; i++)
			{
				if (_particleList[i].currentFittness < _particleList[minFitnessIndex].currentFittness)
				{
					minFitnessIndex = i;
				}
			}

			if (_particleList[minFitnessIndex].currentFittness <= CalcFitness(_globalBestPosition))
			{
				_globalBestPosition = _particleList[minFitnessIndex].currentPosition;
			}

			// マルチスレッド化したら計算がおかしくなった
			//for (var i = 0; i < _particleList.Count; i++)
			//{
			//    UpdateVelocity(ref _particleList[i].velocity,
			//                   _particleList[i].currentPosition,
			//                   _particleList[i].personalBestPosition,
			//                   _globalBestPosition);
			//}

			// Rand がスレッドセーフじゃなかっただけだった
			Parallel.For(0, _particleList.Count, i =>
			{
				UpdateVelocityMT(ref _particleList[i].velocity,
								 _particleList[i].currentPosition,
								 _particleList[i].personalBestPosition,
								 _globalBestPosition);
			});
		}

		static Random rand = new Random();
		static Particle InitializeParticle()
		{
			
			Position position = new Position(
				(rand.NextDouble() - 0.5) * 2 * _stageWidth,
				(rand.NextDouble() - 0.5) * 2 * _stageHeight
			);
			Velocity velocity = new Velocity(
				rand.NextDouble(),
				rand.NextDouble()
			);

			return new Particle(position, velocity);
		}

		static void UpdateFitness(ref double currentFittness, ref Position position)
		{
			currentFittness = CalcFitness(position);
		}

		static double CalcFitness(Position position)
		{
			return Math.Abs(position.x) + Math.Abs(position.y);

			//return Math.Pow(position.x, 2) + Math.Pow(position.y, 2) - 1;

			// Griewank
			//return Griewank(new double[] { position.x, position.y });
		}

		static double Griewank(double[] positions)
		{
			double sum = 1.0, val = 1.0;

			for (int i = 0; i < positions.Length; i++)
			{
				//double Val = (double)i * 0.1357 / (double)positions.Length + positions[i];

				double Val = positions[i];

				sum += ((Val * Val) / (double)4000.0);
				val *= Math.Cos(Val / (double)(i + 1));
			}

			return sum - val;
		}

		static double Rosenbrock(double[] Vals)
		{
			double Num = Vals.Length;
			double sum = 0.0;

			for(int i=0; i<Num-1; i++) 
			{
				double Val1 = (double)i     * 0.1357 / (double)Num + Vals[i];
				double Val2 = (double)(i+1) * 0.1357 / (double)Num + Vals[i+1];

				sum += (  100.0 * (Val2 - Val1*Val1) * (Val2 - Val1*Val1)  +  (1.0 - Val1) * (1.0 - Val1)  );
			}

			return sum;
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

		private static int seed = Environment.TickCount;
		private static ThreadLocal<Random> randomWrapper = new ThreadLocal<Random>(() =>
			new Random(Interlocked.Increment(ref seed)));

		static void UpdateVelocityMT(ref Velocity velocity, Position currentPosition, Position personalBestPosition, Position globalBestPosition)
		{
			using (var rng = new RNGCryptoServiceProvider())
			{
				var rand = randomWrapper.Value;
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
}
