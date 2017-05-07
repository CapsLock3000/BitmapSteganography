using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using BitmapSteganography.Library;
using Microsoft.Win32;

namespace BitmapSteganography.Client
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DefaultExt = ".bmp";
        private const string Filter = "PNG Files (*.png)|*.png|BMP Files (*.bmp)|*.bmp|All files (*.*)|*.*";
        private readonly BitmapColorEncoder encoder;
        private Bitmap inputBitmap;
        private Bitmap outputPicture;

        public MainWindow()
        {
            InitializeComponent();
            encoder = new BitmapColorEncoder
            {
                Encoding = new EncodingSettings
                {
                    NoOfBitsFromR = RBits.SelectedIndex,
                    NoOfBitsFromG = GBits.SelectedIndex,
                    NoOfBitsFromB = BBits.SelectedIndex
                }
            };
        }

        private int BytesAvailbleToSave => encoder.Encoding.BitsPerPixel * BitmapSize / 8;

        private int BitmapSize => inputBitmap.Width * inputBitmap.Height;

        private void SelectFileButtonClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            InputFilePath.Text = openFileDialog.FileName;
        }

        private void LoadPictureButtonClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                DefaultExt = DefaultExt,
                Filter =
                    Filter
            };
            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                inputBitmap = new Bitmap(openFileDialog.OpenFile());
                var imageWpf = new BitmapImage();
                imageWpf.BeginInit();
                imageWpf.StreamSource = openFileDialog.OpenFile();
                imageWpf.EndInit();


                InputImage.Source = imageWpf;
            }
            catch (Exception exception)
            {
                inputBitmap = null;
                InputImage.Source = null;
                LogError("Load", exception);
            }
        }

        private void LogError(string msg, Exception exception = null)
        {
            MessageBox.Show(this, $"{msg} error: {exception}");
        }

        private void EncodeButtonClick(object sender, RoutedEventArgs e)
        {
            BitmapSteganographyException decodeException;
            TryDecode(out decodeException);

            if (decodeException == null)
            {
                if (
                    MessageBox.Show(this, "It appears that picture has already some information. Continue?",
                        "Detected information", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            encoder.InputImage = inputBitmap;
            byte[] input = null;
            try
            {
                input = File.ReadAllBytes(InputFilePath.Text);
            }
            catch (Exception exception)
            {
                LogError("IO load", exception);
            }
            if (encoder.Encoding.BitsPerPixel == 0)
            {
                LogError("Set different pixel settings!");
                return;
            }

            try
            {
                outputPicture = encoder.Encode(input);
                OutputImage.Source = outputPicture.ConvertToWpf();
            }
            catch (Exception ex)
            {
                OutputImage.Source = null;
                outputPicture = null;
                LogError("Encode", ex);
            }

            try
            {
                StatsLabel.Content +=
                    $" Encoded data: {input?.Length}B. Size left: {BytesAvailbleToSave - input?.Length}B";
            }
            catch (NullReferenceException)
            {
                StatsLabel.Content += " Retry encoding.";
            }
        }

        private void DecodeButtonClick(object sender, RoutedEventArgs e)
        {
            BitmapSteganographyException decodeException;
            var data = TryDecode(out decodeException);

            if (data == null)
            {
                LogError("Decode", decodeException);
            }
            else
            {
                var saveWindow = new SaveFileDialog();
                if (saveWindow.ShowDialog() == true)
                {
                    File.WriteAllBytes(saveWindow.FileName, data);
                }
            }
        }

        private byte[] TryDecode(out BitmapSteganographyException exception)
        {
            if (inputBitmap == null)
            {
                exception = null;
                return null;
            }

            exception = null;
            try
            {
                var decoder = new BitmapColorDecoder
                {
                    InputImage = inputBitmap
                };
                var bytes = decoder.Decode();
                return bytes;
            }
            catch (BitmapSteganographyException ex)
            {
                exception = ex;
                return null;
            }
        }

        private void SavePictureButtonClick(object sender, RoutedEventArgs e)
        {
            var saveWindow = new SaveFileDialog
            {
                DefaultExt = DefaultExt,
                Filter = Filter
            };

            if (saveWindow.ShowDialog() != true)
            {
                return;
            }

            outputPicture.Save(saveWindow.FileName);
        }

        private void BitsSettingsChanged(object sender, SelectionChangedEventArgs e)
        {
            if (encoder == null)
            {
                return;
            }

            encoder.Encoding = new EncodingSettings
            {
                NoOfBitsFromR = RBits.SelectedIndex,
                NoOfBitsFromG = GBits.SelectedIndex,
                NoOfBitsFromB = BBits.SelectedIndex
            };

            if (inputBitmap != null)
            {
                StatsLabel.Content = $"Bytes avaible to save: {BytesAvailbleToSave}B";
            }
            else
            {
                StatsLabel.Content = "Select input file.";
            }
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}