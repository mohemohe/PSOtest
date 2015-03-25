using System;
using System.Linq;

// ReSharper disable InconsistentNaming
// ReSharper disable RedundantCast

namespace PSOtest
{
	static class TestFunctions
	{
		/// <summary>
		/// 要素が全て0のとき、最小値0を取る
		/// </summary>
		/// <param name="positions"></param>
		/// <returns></returns>
		public static double Test00(double[] positions)
		{
			var sum = 0.0;
			foreach (var position in positions)
			{
				sum += Math.Abs(position);
			}
			return sum;
		}

		/// <summary>
		/// 要素が全て0のとき、最小値0を取る
		/// </summary>
		/// <param name="positions"></param>
		/// <returns></returns>
		public static double Rastrigin(double[] positions)
		{
			double sum = 10.0 * (double)positions.Length;

			for (int i = 0; i < positions.Length; i++)
			{
				//double Val = (double)i * 0.1357 / (double)Num + Vals[i];

				double Val = positions[i];

				sum += (Val*Val - 10.0*Math.Cos(2.0*Math.PI*Val));
			}

			return sum;
		}

		/// <summary>
		/// 要素が全て1のとき、最小値を0を取る
		/// </summary>
		/// <param name="positions"></param>
		/// <returns></returns>
		public static double Rosenbrock(double[] positions)
		{
			double sum = 0.0;

			for (int i = 0; i < positions.Length - 1; i++)
			{
				double Val1 = (double)i * 0.1357 / (double)positions.Length + positions[i];
				double Val2 = (double)(i + 1) * 0.1357 / (double)positions.Length + positions[i + 1];

				sum += (100.0 * (Val2 - Val1 * Val1) * (Val2 - Val1 * Val1) + (1.0 - Val1) * (1.0 - Val1));
			}

			return sum;
		}

		/// <summary>
		/// 要素が全て0のとき、最小値0を取る
		/// </summary>
		/// <param name="positions"></param>
		/// <returns></returns>
		public static double Griewank(double[] positions)
		{
			double sum = 0.0;
			double add = 1.0;

			for (int i = 0; i < positions.Length; i++)
			{
			    sum += Math.Pow(positions[i], 2)/4000.0;
				add *= Math.Cos(positions[i]/Math.Sqrt(i + 1));
			}

			return sum - add + 1.0;
			//return ((sum*sum) - (val*val))/(sum + val);
		}
		//public static double Griewank(double[] positions)
		//{
		//    double sum = 1.0, val = 1.0;

		//    for (int i = 0; i < positions.Length; i++)
		//    {
		//        double Val = (double)i * 0.1357 / (double)positions.Length + positions[i];

		//        var Val = positions[i];

		//        sum += ((Val * Val) / 4000.0);
		//        val *= Math.Cos(Val / Math.Sqrt(i + 1));
		//    }

		//    return sum - val;
		//    return ((sum*sum) - (val*val))/(sum + val);
		//}

		/// <summary>
		/// 要素が全て0のとき、最小値0を取る
		/// </summary>
		/// <param name="positions"></param>
		/// <returns></returns>
		public static double Ridge(double[] positions)
		{
			double sum = 0.0, subsum = 0.0;

			for (int i = 0; i < positions.Length; i++)
			{
				//double Val = (double)i * 0.1357 / (double)Num + Vals[i];

				double Val = positions[i];

				subsum += Val;
				sum += ( subsum * subsum );
			}

			return sum;
		}

		/// <summary>
		/// [-512, 512]の範囲では、要素が全て420.9678...のとき、最小値-418.9829...を取る
		/// </summary>
		/// <param name="positions"></param>
		/// <returns></returns>
		public static double Schwefel(double[] positions)
		{
			double sum = 0.0;

			for (int i = 0; i < positions.Length; i++)
			{
				//double Val = (double)i * 0.1357 / (double)positions.Length + positions[i];

				double Val = positions[i];

				sum += (-Val*Math.Sin(Math.Sqrt(Math.Abs(Val))));
			}

			return sum;
		}
	}
}
