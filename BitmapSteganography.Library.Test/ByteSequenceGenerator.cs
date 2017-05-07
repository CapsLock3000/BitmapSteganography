using System;

namespace BitmapSteganography.Library.Test
{
    internal static class ByteSequenceGenerator
    {
        internal static byte[] GetSequence(int length, int seed = 1)
        {
            var bytes = new byte[length];
            var random = new Random(seed);
            random.NextBytes(bytes);
            return bytes;
        }
    }
}