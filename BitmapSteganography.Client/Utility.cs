using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace BitmapSteganography.Client
{
    internal static class Utility
    {
        public static BitmapImage ConvertToWpf(this Bitmap input)
        {
            /*
                Based on:
                http://stackoverflow.com/questions/1118496/using-image-control-in-wpf-to-display-system-drawing-bitmap
             */
            var ms = new MemoryStream();
            input.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            return bi;
        }
    }
}