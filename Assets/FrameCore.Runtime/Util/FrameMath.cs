using System;

namespace FrameCore.Runtime
{
    public class FrameMath
    {
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        public static int Next(int a, int b)
        {
            return Random.Next(a, b);
        }
    }
}