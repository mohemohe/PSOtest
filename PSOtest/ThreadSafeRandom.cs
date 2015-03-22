using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PSOtest
{
    static class ThreadSafeRandom
    {
        private static int _seed = Environment.TickCount;
        private static readonly ThreadLocal<Random> RandomWrapper = 
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));

        static public Random Random()
        {
            return RandomWrapper.Value;
        }
    }
}
