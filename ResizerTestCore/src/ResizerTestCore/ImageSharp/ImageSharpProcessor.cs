using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageSharp;
using ImageSharp.Drawing;
using ImageSharp.Drawing.Brushes;
using ImageSharp.Drawing.Processors;
using ImageSharp.Formats;
using ImageSharp.Processing;
using ResizerTestCore.Processor;

namespace ResizerTestCore.ImageSharp
{

    public class ImageSharpProcessor : BaseImageProcessor
    {

        private readonly IResampler _resampler;

        public ImageSharpProcessor()
        {
            _resampler = new TriangleResampler();
            Configuration.Default.AddImageFormat(new JpegFormat());
        }

        protected override MemoryStream CallLibraryResizer(Stream inputStream)
        {
            int resizeWidth = 0;
            int resizeHeight = 0;
            Image imageSharp = new Image(inputStream);
            int padTop = 0;
            int padLeft = 0;
            // set either width or height is zero in resize operation to maintain aspect ratio.
            double existingRatio = (double) imageSharp.Width/(double) imageSharp.Height;
            if (_ratio > existingRatio)
            {
                if (_newHeight < imageSharp.Height)
                {
                    resizeHeight = _newHeight;
                    resizeWidth =
                        Convert.ToInt32((double) imageSharp.Width/((double) imageSharp.Height/(double) _newHeight));
                }
                else
                {
                    resizeHeight = imageSharp.Height;
                    resizeWidth = imageSharp.Width;
                    padTop = Math.Abs(_newHeight - resizeHeight)/2;
                }
                padLeft = Math.Abs(_newWidth - resizeWidth)/2;
                if (resizeHeight >= imageSharp.Height)
                {
                    if (_mode != "pad" || (_mode == "pad" && padLeft == 0))
                    {
                        return inputStream as MemoryStream;
                    }
                }
            }
            else
            {
                if (_newWidth < imageSharp.Width)
                {
                    resizeWidth = _newWidth;
                    resizeHeight =
                        Convert.ToInt32((double) imageSharp.Height/((double) imageSharp.Width/(double) _newWidth));
                }
                else
                {
                    resizeWidth = imageSharp.Width;
                    resizeHeight = imageSharp.Height;
                    padLeft = Math.Abs(_newWidth - resizeWidth)/2;
                }
                padTop = Math.Abs(_newHeight - resizeHeight)/2;
                if (resizeWidth >= imageSharp.Width)
                {
                    if (_mode != "pad" || (_mode == "pad" && padTop == 0))
                    {
                        return inputStream as MemoryStream;
                    }
                }
            }

            imageSharp.MetaData.Quality = _quality;
            imageSharp.MetaData.ExifProfile = null;
            MemoryStream outputStream = new MemoryStream();

            if (_mode == "pad" && (padTop > 0 || padLeft > 0))
            {
                var currentFormat = imageSharp.CurrentImageFormat;
                var config = new Configuration();
                config.AddImageFormat(currentFormat);
                Image frameImage = new Image(width: _newWidth, height: _newHeight);

                var rect = new Rectangle(0, 0, _newWidth, _newHeight);
                var shape = new Rregion(rect);
                var brush = Brushes.Solid(Color.AntiqueWhite);
                var options = GraphicsOptions.Default;
                FillRegionProcessor<Color> processor = new FillRegionProcessor<Color>(brush, shape, options);
                var overlayRect = new Rectangle(0, 0, resizeWidth, resizeHeight);
                DrawImageProcessor<Color> overlay =
                    new DrawImageProcessor<Color>(imageSharp.Resize(resizeWidth, resizeHeight, _resampler, true),
                        new Size(resizeWidth, resizeHeight), new Point(padLeft, padTop), 100);
                frameImage.ApplyProcessor(processor, rect);
                //        .ApplyProcessor(overlay, overlayRect);   // not fluent
                // blend image
                frameImage.ApplyProcessor(overlay, overlayRect);
                frameImage.Save(outputStream);

            }
            else
            {
                imageSharp.Resize(resizeWidth, resizeHeight, _resampler, true)
                    .Save(outputStream);
            }
            return outputStream;
        }
    }
}



