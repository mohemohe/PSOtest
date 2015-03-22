using System;
using System.Threading;

namespace PSOtest
{
    static class ThreadSafeRandom
    {
        private static int _seed = Environment.TickCount;
        private static readonly ThreadLocal<Random> ThreadLocalRandom = 
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));

        static public Random Random()
        {
            return ThreadLocalRandom.Value;
        }
    }
}
