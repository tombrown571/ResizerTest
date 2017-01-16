using System;
using ImageResizer;
using System.IO;

namespace ResizerTestConsole
{

    /// <summary>
    /// Class to Test the ImageResizer product
    /// https://www.nuget.org/packages/ImageResizer/
    /// </summary>
    public class ImageResizerPackageTester : ImagePackageTesterBase
    {

        public ImageResizerPackageTester(string imagePath, string outputDir) : base (imagePath, outputDir)
        {
        }

        public override bool ProcessImage(string outputName, int maxDimension = 200)
        {
            bool isValid = false;
            try
            {
                var outputPath = Path.Combine(_outputDir, outputName + ".jpg");

                string resizeSetting = string.Format("maxwidth={0}&maxheight={0}", maxDimension);

                ImageBuilder.Current.Build(_imagePath, outputPath, new ResizeSettings(resizeSetting));

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
