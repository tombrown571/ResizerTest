using System;
using System.Collections.Concurrent;
using System.IO;

namespace ResizerTestConsole
{
    public abstract class ImagePackageTesterBase : IImagePackageTester
    {
        protected readonly string _imagePath;
        protected readonly string _imagefileName;
        protected readonly string _outputDir;

        public ImagePackageTesterBase(string imagePath, string outputDir)
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

        public abstract bool ProcessImage(string outputName, int maxDimension);

        public ConcurrentBag<ErrorLogStruct> Exceptions { get; }
    }
}
