using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImageResizer;
using System.IO;

namespace ResizerTestConsole
{

    /// <summary>
    /// Class to Test the ImageResizer product
    /// with multiple multi-threaded calls
    /// Saving any exceptions
    /// </summary>
    public class ImageProcessor
    {
        private readonly string _imagePath;
        private readonly string _imagefileName;
        private readonly string _outputDir;

        public ImageProcessor(string imagePath, string outputDir)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                throw new ArgumentNullException(nameof(imagePath));
            }
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException("missing image file", imagePath);
            }
            _imagePath = imagePath;
            if (!Directory.Exists(outputDir))
            {
                throw new ArgumentException(string.Format("Output Directory: {0} not found", _outputDir));
            }
            _outputDir = outputDir;
            FileInfo fi = new FileInfo(_imagePath);
            _imagefileName = fi.Name;
            Exceptions = new ConcurrentBag<ErrorLogStruct>();
        }

        public bool ProcessImage(string outputName, int maxDimension = 200)
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

        public ConcurrentBag<ErrorLogStruct> Exceptions { get; }
    }
}
