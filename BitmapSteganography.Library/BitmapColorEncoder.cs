using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace BitmapSteganography.Library
{
    /// <summary>
    ///     Encodes given byte sequence into given bitmap.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    public class BitmapColorEncoder
    {
        private byte[] data;
        private int lastReadBit;
        private int requiredPixels;
        private Bitmap result;

        /// <summary>
        ///     Settings to be used during encoding.
        /// </summary>
        public EncodingSettings Encoding { get; set; }

        /// <summary>
        ///     Original image. Not modyfied by the class.
        /// </summary>
        public Bitmap InputImage { get; set; }

        private int BitsOfData => data.Length * 8;

        private int AvailableBitsOfData => (InputImage.Width * InputImage.Height - 2) *
                                           Encoding.BitsPerPixel;

        private Color EncodingPixel => Color.FromArgb(Encoding.NoOfBitsFromR,
            Encoding.NoOfBitsFromG,
            Encoding.NoOfBitsFromB);

        /// <summary>
        ///     Encodes byte sequence using <see cref="InputImage" /> as an source.
        /// </summary>
        /// <param name="input">Byte sequence to encode in image.</param>
        /// <returns>New instance of <see cref="Bitmap" />.</returns>
        public Bitmap Encode(byte[] input)
        {
            data = input;

            CheckSize();

            result = new Bitmap(InputImage.Width, InputImage.Height);
            SetupHeader();

            CalculateRequiredPixels();

            FillPixels();

            return result;
        }

        private void CheckSize()
        {
            if (BitsOfData > AvailableBitsOfData)
            {
                throw new BitmapSteganographyException("Data overflow exception");
            }
        }

        private void SetupHeader()
        {
            var sizePixel = Color.FromArgb(data.Length);
            result.SetPixel(0, 0, EncodingPixel);
            result.SetPixel(1, 0, sizePixel);
        }

        private void CalculateRequiredPixels()
        {
            var bitsPerPixel = Encoding.BitsPerPixel;

            // division with rounding up 
            requiredPixels = (BitsOfData + bitsPerPixel - 1) / bitsPerPixel;
        }

        private void FillPixels()
        {
            lastReadBit = -1;
            for (var currentPixel = 0; currentPixel < requiredPixels; currentPixel++)
            {
                var xPosition = (currentPixel + 2) % InputImage.Width;
                var yPosition = (currentPixel + 2) / InputImage.Width;
                var sourcePixel = InputImage.GetPixel(xPosition,
                    yPosition);

                var rPayload = GetPayload(Encoding.NoOfBitsFromR);
                var gPayload = GetPayload(Encoding.NoOfBitsFromG);
                var bPayload = GetPayload(Encoding.NoOfBitsFromB);

                var newPixel = Color.FromArgb(
                    (sourcePixel.R & ~Encoding.MaskR) | rPayload,
                    (sourcePixel.G & ~Encoding.MaskG) | gPayload,
                    (sourcePixel.B & ~Encoding.MaskB) | bPayload);

                result.SetPixel(xPosition, yPosition, newPixel);
            }

            var size = InputImage.Width * InputImage.Height;
            for (var currentPixel = requiredPixels + 2; currentPixel < size; currentPixel++)
            {
                var xPosition = currentPixel % InputImage.Width;
                var yPosition = currentPixel / InputImage.Width;

                result.SetPixel(xPosition,
                    yPosition,
                    InputImage.GetPixel(xPosition, yPosition));
            }
        }

        private byte GetPayload(int bitsNo)
        {
            byte payload = 0;
            for (var bit = 0; bit < bitsNo; bit++)
            {
                ++lastReadBit;
                if (GetBit(lastReadBit))
                {
                    payload = (byte) ((payload << 1) + 1);
                }
                else
                {
                    payload <<= 1;
                }
            }

            return payload;
        }

        private bool GetBit(int i)
        {
            var byteNo = i / 8;
            if (byteNo == data.Length)
            {
                return false;
            }

            var @byte = data[byteNo];
            var bit = i % 8;

            return (@byte >> (7 - bit)) % 2 == 1;
        }
    }
}