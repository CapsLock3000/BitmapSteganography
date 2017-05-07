# BitmapSteganography

Library with client to hide data in uncompressed bitmap.

![ScreenShot](screenshot.png)

## Contents

`BitmapSteganography.Library` contains library realizing the
[steganography](https://en.wikipedia.org/wiki/Steganography).
_Encoder_ as an input takes byte sequence and input image.
As an output gives similar image, but with hidden input data.
_Decoder_ as an input takes image with encoded data and returns
original byte sequence.

Encoder can be configured to control image quality.
Decoder reads configuration from image.

`BitmapSteganography.Library.Test` contains tests of the library.
Performs encode-decode cycles.

`BitmapSteganography.Client` contains GUI client to perform
encoding and decoding from `BitmapSteganography.Library`.

The image `Test.bmp` is from Wikipedia.

## Disclaimer

The software is intended to be used only for educational purposes.