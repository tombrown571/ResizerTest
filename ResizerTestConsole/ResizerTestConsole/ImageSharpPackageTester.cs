using System;
using System.IO;
using ImageSharp;
using ImageSharp.Processing;

namespace ResizerTestConsole
{
    /// <summary>
    /// ImageSharp (currently in alpha)
    /// Placeholder for https://www.nuget.org/packages/ImageSharp/ when its ready
    /// </summary>
    public class ImageSharpPackageTester : ImagePackageTesterBase
    {

        private IResampler _resampler;
        public ImageSharpPackageTester(string imagePath, string outputDir) : base(imagePath, outputDir)
        {
            _resampler = new NearestNeighborResampler(); // default
        }

        public string ResamplerAlgorithm
        {
            set
            {
                switch (value.ToLower())
                {
                    case "bicubic":
                        _resampler = new BicubicResampler();
                        break;
                    case "triangle":
                        _resampler = new TriangleResampler();
                        break;
                    case "nearestneighbor":
                        _resampler = new NearestNeighborResampler();
                        break;
                    case "mitchellnetravali":
                        _resampler = new MitchellNetravaliResampler();
                        break;
                    default:
                        throw new ArgumentException("Invalid ResamplerAlgorithm");
                }
            }
        }

        public override bool ProcessImage(string outputName, int maxDimension)
        {
            bool isValid = false;
            try
            {
                var outputPath = Path.Combine(_outputDir, outputName + ".jpg");

                var size = new Size(maxDimension, maxDimension);

                using (FileStream inStream = File.OpenRead(_imagePath))
                {
                    ImageSharp.Image imageSharp = new ImageSharp.Image(inStream);
                    // if either width or height is zero in resize operation, it should maintain aspect ratio.
                    var newWidth = imageSharp.Width > imageSharp.Height ? maxDimension : 0;
                    var newHeight = imageSharp.Height >= imageSharp.Width ? maxDimension : 0;
                    using (FileStream outStream = File.OpenWrite(outputPath))
                    {
                        imageSharp.Resize(newWidth, newHeight, _resampler, true)
                            .Save(outStream);
                        //var curformat = imageSharp.CurrentImageFormat;
                        // imageSharp.MaxHeight = maxDimension;
                        // imageSharp.MaxWidth = maxDimension;
                        // imageSharp.Save(outStream);

                    }
                }
                isValid = true;

            }
            catch (Exception ex)
            {
                Exceptions.Add(new ErrorLogStruct()
                {
                    Ex = ex,
                    LogText = string.Format("File: {0}, Dimension: {1}", _imagefileName, maxDimension),
                });
            }
            return isValid;
        }
    }
}
