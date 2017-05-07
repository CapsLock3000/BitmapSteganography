using System;
using System.Drawing;

namespace BitmapSteganography.Library
{
    /// <summary>
    ///     Decodes byte sequence from given bitmap.
    /// </summary>
    public class BitmapColorDecoder
    {
        private bool[] bits;
        private byte[] data;
        private int dataLength;
        private EncodingSettings encoding;
        private int lastBitSet;
        private int requiredPixels;

        /// <summary>
        ///     Image containing encoded byte sequence. Not modyfied by the class.
        /// </summary>
        public Bitmap InputImage { get; set; }

        /// <summary>
        ///     Decodes byte sequence from <see cref="InputImage" />.
        /// </summary>
        /// <returns>Decoded byte sequence.</returns>
        public byte[] Decode()
        {
            ReadHeader();
            CalculateRequiredPixels();
            CalculateBits();
            CalculateBytes();
            bits = null;
            return data;
        }

        private void ReadHeader()
        {
            try
            {
                ReadEncoding();
            }
            catch (ArgumentException e)
            {
                throw new BitmapSteganographyException("Wrong input: wrong formatted header.", e);
            }

            ReadLength();
        }

        private void CalculateRequiredPixels()
        {
            var bitsPerPixel = encoding.BitsPerPixel;
            // division with rounding up 
            requiredPixels = (dataLength * 8 + bitsPerPixel - 1) / bitsPerPixel;
        }

        private void CalculateBits()
        {
            bits = new bool[dataLength * 8 + encoding.BitsPerPixel];
            lastBitSet = -1;
            for (var i = 0; i < requiredPixels; i++)
            {
                var width = InputImage.Width;
                var pixel = InputImage.GetPixel((i + 2) % width,
                    (i + 2) / width);

                PackYoungestBits(pixel.R, encoding.NoOfBitsFromR);
                PackYoungestBits(pixel.G, encoding.NoOfBitsFromG);
                PackYoungestBits(pixel.B, encoding.NoOfBitsFromB);
            }
        }

        private void CalculateBytes()
        {
            data = new byte[dataLength];
            for (var i = 0; i < dataLength; i++)
            {
                var startBit = 8 * i;
                data[i] = ReadByteFromBits(startBit);
            }
        }

        private void ReadLength()
        {
            var secondPixel = InputImage.GetPixel(1, 0);
            dataLength = secondPixel.ToArgb();
        }

        private void ReadEncoding()
        {
            var firstPixel = InputImage.GetPixel(0, 0);
            encoding = new EncodingSettings
            {
                NoOfBitsFromR = firstPixel.R,
                NoOfBitsFromG = firstPixel.G,
                NoOfBitsFromB = firstPixel.B
            };
        }

        private void PackYoungestBits(byte inputByte, int bitsToRead)
        {
            for (var j = 0; j < bitsToRead; j++)
            {
                bits[lastBitSet + bitsToRead - j] = inputByte % 2 == 1;
                inputByte >>= 1;
            }

            lastBitSet += bitsToRead;
        }

        private byte ReadByteFromBits(int startBit)
        {
            byte cell = 0;

            for (var bit = 0; bit < 8; bit++)
            {
                if (bits[startBit + bit])
                {
                    cell = (byte) ((cell << 1) + 1);
                }
                else
                {
                    cell <<= 1;
                }
            }

            return cell;
        }
    }
}