using System;
using System.Runtime.Serialization;

namespace BitmapSteganography.Library
{
    /// <summary>
    ///     Exception for encoding or decoding exceptions.
    /// </summary>
    [Serializable]
    public class BitmapSteganographyException : Exception
    {
        public BitmapSteganographyException()
        {
        }

        public BitmapSteganographyException(string message)
            : base(message)
        {
        }

        public BitmapSteganographyException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected BitmapSteganographyException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}