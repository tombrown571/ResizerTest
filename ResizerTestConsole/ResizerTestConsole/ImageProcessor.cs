using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImageResizer;
using System.IO;

namespace ResizerTestConsole
{
    public class ImageProcessor
    {
        private string _imagePath;

        private string _outputDir;

        public ImageProcessor(string imagePath)
        {
            // verify that imagePath exists
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                throw new ArgumentNullException(nameof(imagePath));
            }
            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException("missing image file", imagePath);
            }
            _imagePath = imagePath;
            // our image processor is useless without somewhere to create processed images
            FileInfo fi = new FileInfo(_imagePath);
            _outputDir = Path.Combine(fi.Directory.FullName, @"TestOutput");
            if (!Directory.Exists(_outputDir))
            {
                throw new ArgumentException(string.Format("Output Directory: {0} not found", _outputDir));
            }

        }


        public bool ProcessImage(string outputName)
        {
            // ImageResizer.Resizing.RequestedAction.
            //System.Threading.Thread.Sleep(500);

            var outputPath = Path.Combine(_outputDir, outputName + ".jpg");

            ImageBuilder.Current.Build(_imagePath, outputPath, new ResizeSettings("maxwidth=200&maxheight=200"));


            return true;
        }
    }
}
