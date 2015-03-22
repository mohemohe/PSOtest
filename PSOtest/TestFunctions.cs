using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSOtest
{
    static class TestFunctions
    {
        public static double Test00(double[] positions)
        {
            var sum = 0.0;
            foreach (var position in positions)
            {
                sum += Math.Abs(position);
            }
            return sum;
        }

        public static double Griewank(double[] positions)
        {
            double sum = 1.0, val = 1.0;

            for (int i = 0; i < positions.Length; i++)
            {
                //double Val = (double)i * 0.1357 / (double)positions.Length + positions[i];

                double Val = positions[i];

                sum += (Val * Val / 4000.0);
                val *= Math.Cos(Val / (i + 1.0));
            }

            return sum - val;
        }

        public static double Rosenbrock(double[] Vals)
        {
            double Num = Vals.Length;
            double sum = 0.0;

            for (int i = 0; i < Num - 1; i++)
            {
                double Val1 = (double)i * 0.1357 / (double)Num + Vals[i];
                double Val2 = (double)(i + 1) * 0.1357 / (double)Num + Vals[i + 1];

                sum += (100.0 * (Val2 - Val1 * Val1) * (Val2 - Val1 * Val1) + (1.0 - Val1) * (1.0 - Val1));
            }

            return sum;
        }
    }
}
