using ImageProcessor.Imaging.Formats;
using ImageSharp;
using ImageSharp.Processing;
using System;
using System.IO;
using ImageSharp.Formats;

namespace ResizerTestConsole
{
    public class ImageSharpProcessor
    {

        private string _imgName;
        private readonly int _newWidth;
        private readonly int _newHeight;
        private readonly double _ratio;
        private readonly int _quality;
        private readonly IResampler _resampler;

        public ImageSharpProcessor(string imageName, Preset preset)
        {
            if (string.IsNullOrWhiteSpace(imageName))
            {
                throw new ArgumentNullException(nameof(imageName));
            }
            _imgName = imageName;
            if (preset == null)
            {
                throw new ArgumentNullException(nameof(preset));
            }
            _newWidth = preset.Width;
            _newHeight = preset.Height;
            _quality = preset.Quality;
            _ratio = (double)_newWidth / (double)_newHeight;
            _resampler = new TriangleResampler();
            IImageFormat format = new ImageSharp.Formats.JpegFormat();
            Configuration.Default.AddImageFormat(format);
        }

        public MemoryStream ResizeImage(Stream inputStream)
        {
            if (inputStream == null || inputStream.Length == 0)
            {
                throw new ArgumentNullException(nameof(inputStream));
            }
            int resizeWidth = 0;
            int resizeHeight = 0;
            ImageSharp.Image imageSharp = new ImageSharp.Image(inputStream);
            // set either width or height is zero in resize operation to maintain aspect ratio.
            double existingRatio = (double)imageSharp.Width / (double)imageSharp.Height;
            if (existingRatio > _ratio)
            {
                resizeWidth = _newWidth;
            }
            else
            {
                resizeHeight = _newHeight;
            }
            imageSharp.Quality = _quality;
            imageSharp.ExifProfile = null;
            MemoryStream outputStream = new MemoryStream();
            imageSharp.Resize(resizeWidth, resizeHeight, _resampler, true)
                .Save(outputStream);
            return outputStream;
        }
    }

    public class Preset
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Quality { get; set; } = 90;
    }
}
