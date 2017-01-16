using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResizerTestConsole
{
    /// <summary>
    /// Placeholder for https://www.nuget.org/packages/ImageSharp/ when its ready
    /// </summary>
    public class ImageSharpPackageTester : ImagePackageTesterBase
    {
        public ImageSharpPackageTester(string imagePath, string outputDir) : base(imagePath, outputDir)
        {
        }

        public override bool ProcessImage(string outputName, int maxDimension)
        {


            throw new NotImplementedException();
        }
    }
}
