using System;

namespace BitmapSteganography.Library
{
    /// <summary>
    ///     Encoding and decoding configuration.
    ///     Describes, how many bits from color channels are consumed for byte sequence encoding.
    /// </summary>
    public class EncodingSettings
    {
        private int noofbitsfromb;
        private int noofbitsfromg;
        private int noofbitsfromr;

        public int NoOfBitsFromR
        {
            get => noofbitsfromr;
            set
            {
                CheckRange(value);
                noofbitsfromr = value;
            }
        }

        public int NoOfBitsFromG
        {
            get => noofbitsfromg;
            set
            {
                CheckRange(value);
                noofbitsfromg = value;
            }
        }

        public int NoOfBitsFromB
        {
            get => noofbitsfromb;
            set
            {
                CheckRange(value);
                noofbitsfromb = value;
            }
        }

        public int BitsPerPixel => NoOfBitsFromR + NoOfBitsFromG + NoOfBitsFromB;

        public byte MaskR => GetMask(NoOfBitsFromR);
        public byte MaskG => GetMask(NoOfBitsFromG);
        public byte MaskB => GetMask(NoOfBitsFromB);

        private static byte GetMask(int bits)
        {
            switch (bits)
            {
                case 0:
                    return 0x00;
                case 1:
                    return 0x01;
                case 2:
                    return 0x03;
                case 3:
                    return 0x07;
                case 4:
                    return 0x0f;
                case 5:
                    return 0x1f;
                case 6:
                    return 0x3f;
                case 7:
                    return 0x7f;
                default:
                    return 0xff;
            }
        }

        private static void CheckRange(int value)
        {
            if (value < 0 || value > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
    }
}