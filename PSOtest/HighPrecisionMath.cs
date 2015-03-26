using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PSOtest
{
    static class HighPrecisionMath
    {
        public static BigInteger Abs(BigInteger a)
        {
            if (a.Sign < 0)
            {
                return -a;
            }
            else
            {
                return a;
            }
        }
    }
}
