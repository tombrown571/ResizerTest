using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResizerTestConsole
{
    public class ImageProcessor
    {
        private string _imagePath;
        public ImageProcessor(string imagePath)
        {
            _imagePath = imagePath;

        }


        public bool ProcessImage()
        {
            // ImageResizer.Resizing.RequestedAction.
            System.Threading.Thread.Sleep(500);

            return true;
        }
    }
}
