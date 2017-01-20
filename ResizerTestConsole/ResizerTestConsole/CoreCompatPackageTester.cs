using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace ResizerTestConsole
{
    /// <summary>
    /// https://www.nuget.org/packages/CoreCompat.System.Drawing/
    /// </summary>
    public class CoreCompatPackageTester : ImagePackageTesterBase
    {
        private int _quality = 90;
        private ImageCodecInfo _codec;
        public CoreCompatPackageTester(string imagePath, string outputDir) : base(imagePath, outputDir)
        {
        }

        public override bool ProcessImage(string outputName, int maxDimension)
        {
            bool isValid = false;
            try
            {
                var outputPath = Path.Combine(_outputDir, outputName + ".jpg");
                using (var image = new Bitmap(System.Drawing.Image.FromFile(_imagePath)))
                {

                    int width, height;
                    if (image.Width > image.Height)
                    {
                        width = maxDimension;
                        height = Convert.ToInt32(image.Height * maxDimension / (double)image.Width);
                    }
                    else
                    {
                        width = Convert.ToInt32(image.Width * maxDimension / (double)image.Height);
                        height = maxDimension;
                    }
                    var resized = new Bitmap(width, height);
                    using (var graphics = Graphics.FromImage(resized))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighSpeed;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.CompositingMode = CompositingMode.SourceCopy;
                        graphics.DrawImage(image, 0, 0, width, height);
                        using (var output = File.Open(outputPath, FileMode.Create))
                        {
                            var qualityParamId = Encoder.Quality;
                            var encoderParameters = new EncoderParameters(1);
                            encoderParameters.Param[0] = new EncoderParameter(qualityParamId, _quality);
                            _codec = ImageCodecInfo.GetImageDecoders()
                                .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
                            resized.Save(output, _codec, encoderParameters);

                        }

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
