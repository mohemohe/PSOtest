using PSOtest.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable HeuristicUnreachableCode
// ReSharper disable RedundantExplicitArrayCreation
#pragma warning disable 162


namespace PSOtest
{
    internal static class Program
    {
        private const int Particles = 1000;
        private const int MaxLoop = Int32.MaxValue;
        private const double StageWidth = 32.0;
        private const double StageHeight = StageWidth;

        private static readonly List<Particle> ParticleList = new List<Particle>();
        private static Position GlobalBestPosition { get; set; }

        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            // start debug
            var test1 = new BigDecimal("-3.0000000105392843");
            Console.WriteLine(test1.ToString());
            var test2 = new BigDecimal("4.0000000105392843");
            Console.WriteLine(BigDecimal.Add(test1, test2).ToString());
            Console.ReadLine();
            // end debug

            // 初期化
            // リストの排他制御がそれなりに重いので粒子が少ない場合はシングルスレッドで初期化する
            Console.WriteLine("Initializing...");
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Particles <= 100000)
            {
                for (var i = 0; i < Particles; i++)
                {
                    ParticleList.Add(InitializeParticle());
                    UpdateFitness(ref ParticleList[i].CurrentFittness, ParticleList[i].CurrentPosition);
                }
            }
            else
            {
                Parallel.For(0, Particles, i =>
                {
                    lock (ParticleList) ParticleList.Add(InitializeParticle());
                });
            }

            Parallel.For(0, Particles, i =>
            {
                UpdateFitness(ref ParticleList[i].CurrentFittness, ParticleList[i].CurrentPosition);
            });

            // 初期化したランダムな粒子から現在の準最適な位置を保持する
            var minFitnessIndex = 0;
            for (var i = 0; i < ParticleList.Count; i++)
            {
                if (ParticleList[i].CurrentFittness < ParticleList[minFitnessIndex].CurrentFittness)
                {
                    minFitnessIndex = i;
                }
            }
            GlobalBestPosition = ParticleList[minFitnessIndex].CurrentPosition;

            var sw = new Stopwatch();
            sw.Start();

            // 関数最小化のループ
            for (var i = 0; i < MaxLoop; i++)
            {
                if (i%1== 0)
                {
                    Console.Clear();
                    OutputConsole(i, sw.Elapsed);
                }

                // == 0.0 は嫌な予感がする
                if (Math.Abs(CalcFitness(GlobalBestPosition)) < Double.Epsilon)
                {
                    sw.Stop();

                    Console.Clear();
                    OutputConsole(i, sw.Elapsed);

                    return;
                }

                PSO();
            }
        }

        /// <summary>
        /// 進捗表示用
        /// </summary>
        /// <param name="loop"></param>
        /// <param name="elapsedTime"></param>
        private static void OutputConsole(int loop, TimeSpan elapsedTime)
        {
            var str = new StringBuilder();
            str.AppendLine("現在の最適なX座標" + "\t\t" +
                           "現在の最適なY座標" + "\t\t" +
                           "評価関数の出力");
            str.AppendLine(GlobalBestPosition.X.ToString().PadRight(20) + "\t\t" +
                           GlobalBestPosition.Y.ToString().PadRight(20) + "\t\t" +
                           CalcFitness(GlobalBestPosition));
            str.AppendLine();
            str.AppendLine("粒子数\t\t" + Particles);
            str.AppendLine("ループ回数\t" + loop);
            str.AppendLine("経過時間\t" + elapsedTime);

            Console.WriteLine(str);
        }

        /// <summary>
        /// 粒子群最適化のキモ
        /// </summary>
        private static void PSO()
        {
            Parallel.For(0, ParticleList.Count, i =>
            {
                UpdatePosition(ref ParticleList[i].CurrentPosition, ParticleList[i].Velocity);
                UpdateFitness(ref ParticleList[i].CurrentFittness, ParticleList[i].CurrentPosition);

                if (ParticleList[(int)i].CurrentFittness < CalcFitness(ParticleList[i].PersonalBestPosition))
                {
                    UpdatePosition(ref ParticleList[i].PersonalBestPosition, ParticleList[i].CurrentPosition);
                }

            });

            var minFitnessIndex = 0;
            for (var i = 0; i < ParticleList.Count; i++)
            {
                if (ParticleList[i].CurrentFittness < ParticleList[minFitnessIndex].CurrentFittness)
                {
                    minFitnessIndex = i;
                }
            }
            if (ParticleList[minFitnessIndex].CurrentFittness < CalcFitness(GlobalBestPosition))
            {
                UpdatePosition(GlobalBestPosition, ParticleList[minFitnessIndex].CurrentPosition);
            }

            Parallel.For(0, ParticleList.Count, i =>
            {
                UpdateVelocity(ref ParticleList[i].Velocity,
                    ParticleList[i].CurrentPosition,
                    ParticleList[i].PersonalBestPosition,
                    GlobalBestPosition);
            });
        }

        /// <summary>
        /// 粒子をランダムに初期化する
        /// </summary>
        /// <returns></returns>
        private static Particle InitializeParticle()
        {
            var rand = ThreadSafeRandom.Random();
            var position = new Position(
                (rand.NextDouble() - 0.5)*2*StageWidth,
                (rand.NextDouble() - 0.5)*2*StageHeight
                );
            var velocity = new Velocity(
                rand.NextDouble(),
                rand.NextDouble()
                );

            return new Particle(position, velocity);
        }

        /// <summary>
        /// 適応度を計算する
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private static double CalcFitness(Position position)
        {
            const int type = 2;

            switch (type)
            {
                case 1:
                    return TestFunctions.Rastrigin(position.ToArray());

                case 2:
                    return TestFunctions.Rosenbrock(position.ToArray());

                case 3:
                    return TestFunctions.Griewank(position.ToArray());

                case 4:
                    return TestFunctions.Ridge(position.ToArray());

                case 5:
                    return TestFunctions.Schwefel(position.ToArray());

                default:
                    return TestFunctions.Test00(position.ToArray());
            }
        }

        /// <summary>
        /// 適応度を更新する
        /// </summary>
        /// <param name="currentFittness"></param>
        /// <param name="position"></param>
        // ReSharper disable once RedundantAssignment
        private static void UpdateFitness(ref double currentFittness, Position position)
        {
            currentFittness = CalcFitness(position);
        }

        /// <summary>
        /// 位置を更新する
        /// </summary>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        private static void UpdatePosition(ref Position position, Velocity velocity)
        {
            position.X += velocity.X;
            position.Y += velocity.Y;
        }

        /// <summary>
        /// 位置を更新する
        /// </summary>
        /// <param name="position"></param>
        /// <param name="newPosition"></param>
        private static void UpdatePosition(Position position, Position newPosition)
        {
            position.X = newPosition.X;
            position.Y = newPosition.Y;
        }

        /// <summary>
        /// 位置を更新する
        /// </summary>
        /// <param name="position"></param>
        /// <param name="newPosition"></param>
        private static void UpdatePosition(ref Position position, Position newPosition)
        {
            position.X = newPosition.X;
            position.Y = newPosition.Y;
        }

        /// <summary>
        /// 速さを更新する
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="currentPosition"></param>
        /// <param name="personalBestPosition"></param>
        /// <param name="globalBestPosition"></param>
        private static void UpdateVelocity(ref Velocity velocity, Position currentPosition,
            Position personalBestPosition, Position globalBestPosition)
        {
            var rand = ThreadSafeRandom.Random();
            var randX = rand.NextDouble();
            var randY = rand.NextDouble();

            velocity.X = 0.8*velocity.X +
                         2.0*randX*(personalBestPosition.X - currentPosition.X) +
                         2.0*randY*(globalBestPosition.X - currentPosition.X);

            velocity.Y = 0.8*velocity.Y +
                         2.0*randX*(personalBestPosition.Y - currentPosition.Y) +
                         2.0*randY*(globalBestPosition.Y - currentPosition.Y);
        }
    }
}