using System;
//using System.Drawing;
//using System.Drawing.Imaging;
using System.IO;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;

namespace ResizerTestConsole
{

    /// <summary>
    /// Test package:  https://www.nuget.org/packages/ImageProcessor/
    /// </summary>
    public class ImageProcessorPackageTester : ImagePackageTesterBase
    {
        public ImageProcessorPackageTester(string imagePath, string outputDir) : base(imagePath, outputDir)
        {
        }

        public override bool ProcessImage(string outputName, int maxDimension)
        {
            bool isValid = false;
            try
            {
                var outputPath = Path.Combine(_outputDir, outputName + ".jpg");
               
                byte[] picBytes = File.ReadAllBytes(_imagePath);
                ISupportedImageFormat format = new JpegFormat();
                var size = new System.Drawing.Size(maxDimension, maxDimension);
                using (var inStream = new MemoryStream(picBytes, writable: false))
                {
                    using (var outStream = new MemoryStream())
                    {
                        using (var imageFactory = new ImageFactory(preserveExifData: true))
                        {
                            imageFactory.Load(inStream)
                                .Resize(size)
                                .Format(format)
                                .Save(outStream);
                        }
                        outStream.WriteTo(new FileStream(outputPath, FileMode.CreateNew));
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
