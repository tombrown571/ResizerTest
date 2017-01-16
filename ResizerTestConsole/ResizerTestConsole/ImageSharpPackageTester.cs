using System;
using System.IO;
using ImageSharp;

namespace ResizerTestConsole
{
    /// <summary>
    /// ImageSharp (currently in alpha)
    /// Placeholder for https://www.nuget.org/packages/ImageSharp/ when its ready
    /// </summary>
    public class ImageSharpPackageTester : ImagePackageTesterBase
    {
        public ImageSharpPackageTester(string imagePath, string outputDir) : base(imagePath, outputDir)
        {
        }

        public override bool ProcessImage(string outputName, int maxDimension)
        {
            bool isValid = false;
            try
            {
                var outputPath = Path.Combine(_outputDir, outputName + ".jpg");

                var size = new System.Drawing.Size(maxDimension, maxDimension);

                using (FileStream inStream = File.OpenRead(_imagePath))
                {
                    using (var outStream = new MemoryStream())
                    {
                        Image imageSharp = new Image(inStream);
                        imageSharp.MaxHeight = maxDimension;
                        imageSharp.MaxWidth = maxDimension;
                        imageSharp.Save(outStream);
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
