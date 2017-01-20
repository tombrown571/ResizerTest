using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace ResizerTestConsole
{
    /// <summary>
    /// Note: SkiaSharp kills the runtime under some circumstances
    /// https://github.com/mono/SkiaSharp/
    /// </summary>
    public class SkiaSharpPackageTester : ImagePackageTesterBase
    {
        private int _quality = 90;
        public SkiaSharpPackageTester(string imagePath, string outputDir) : base(imagePath, outputDir)
        {
        }

        public override bool ProcessImage(string outputName, int maxDimension)
        {
            bool isValid = false;
            try
            {
                var outputPath = Path.Combine(_outputDir, outputName + ".jpg");
                using (var input = File.OpenRead(_imagePath))
                {
                    using (var inputStream = new SKManagedStream(input))
                    {
                        var original = SKBitmap.Decode(inputStream);
                        int width, height;
                        if (original.Width > original.Height)
                        {
                            width = maxDimension;
                            height = original.Height * maxDimension / original.Width;
                        }
                        else
                        {
                            width = original.Width * maxDimension / original.Height;
                            height = maxDimension;
                        }

                        var surface = SKSurface.Create(width, height, original.ColorType, original.AlphaType);
                        var canvas = surface.Canvas;
                        var scale = (float)width / original.Width;
                        canvas.Scale(scale);
                        var paint = new SKPaint();
                        paint.FilterQuality = SKFilterQuality.High;
                        canvas.DrawBitmap(original, 0, 0, paint);
                        canvas.Flush();

                        using (var output = File.OpenWrite(outputPath))
                        {
                            surface.Snapshot()
                                .Encode(SKImageEncodeFormat.Jpeg, _quality)
                                .SaveTo(output);
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
