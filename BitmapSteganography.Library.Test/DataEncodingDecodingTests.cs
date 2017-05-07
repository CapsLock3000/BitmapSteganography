using System.Drawing;
using System.Reflection;
using NUnit.Framework;

namespace BitmapSteganography.Library.Test
{
    [TestFixture]
    internal class DataEncodingDecodingTests
    {
        private Bitmap testBitmap;

        private static readonly object[] Sequences =
        {
            new object[] {new byte[] { }},
            new object[] {new byte[] {200}},
            new byte[] {200, 120},
            new byte[] {200, 120, 123},
            ByteSequenceGenerator.GetSequence(50, 7),
            ByteSequenceGenerator.GetSequence(100, 8),
            ByteSequenceGenerator.GetSequence(300, 9),
            ByteSequenceGenerator.GetSequence(10000, 10)
        };

        private int PixelsForData => testBitmap.Width * testBitmap.Height - 2;

        private byte[] EncodeDecodeSequence(byte[] sequence, EncodingSettings encoding)
        {
            var bitmapColorEncoder = new BitmapColorEncoder
            {
                Encoding = encoding,
                InputImage = testBitmap
            };

            var encodedBitmap = bitmapColorEncoder.Encode(sequence);

            var bitmapColorDecoder = new BitmapColorDecoder
            {
                InputImage = encodedBitmap
            };

            return bitmapColorDecoder.Decode();
        }

        [OneTimeSetUp]
        public void LoadResources()
        {
            var assembly = Assembly.GetAssembly(typeof(DataEncodingDecodingTests));

            using (var manifestResourceStream =
                assembly.GetManifestResourceStream("BitmapSteganography.Library.Test.Test.bmp"))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                testBitmap = new Bitmap(manifestResourceStream);
            }
        }

        [OneTimeTearDown]
        public void DisposeResources()
        {
            testBitmap?.Dispose();
            testBitmap = null;
        }

        [Test]
        [TestCaseSource(nameof(Sequences))]
        public void CheckEncodingDecodingRoundSeriesATest(byte[] sequence)
        {
            var encoding = new EncodingSettings {NoOfBitsFromR = 3, NoOfBitsFromG = 4, NoOfBitsFromB = 2};

            var decodedSequence = EncodeDecodeSequence(sequence, encoding);

            Assert.That(decodedSequence, Is.EqualTo(sequence));
        }

        [Test]
        [TestCaseSource(nameof(Sequences))]
        public void CheckEncodingDecodingRoundSeriesBTest(byte[] sequence)
        {
            var encoding = new EncodingSettings {NoOfBitsFromR = 2, NoOfBitsFromG = 2, NoOfBitsFromB = 1};

            var decodedSequence = EncodeDecodeSequence(sequence, encoding);

            Assert.That(decodedSequence, Is.EqualTo(sequence));
        }

        [Test]
        public void DataOverflowTest()
        {
            var encoding = new EncodingSettings {NoOfBitsFromR = 1, NoOfBitsFromG = 1, NoOfBitsFromB = 1};
            var testSequence = ByteSequenceGenerator.GetSequence(PixelsForData);

            var bitmapColorEncoder = new BitmapColorEncoder
            {
                Encoding = encoding,
                InputImage = testBitmap
            };

            Assert.That(() => bitmapColorEncoder.Encode(testSequence),
                Throws.InstanceOf<BitmapSteganographyException>()
                    .And.Message.Contains("data overflow").IgnoreCase);
        }

        [Test]
        public void EncodingWrongInputTest()
        {
            var bitmapColorDecoder = new BitmapColorDecoder
            {
                InputImage = testBitmap
            };

            Assert.That(() => bitmapColorDecoder.Decode(),
                Throws.InstanceOf<BitmapSteganographyException>()
                    .And.Message.Contains("input"));
        }
    }
}