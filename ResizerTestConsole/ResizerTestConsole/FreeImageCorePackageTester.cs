using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResizerTestConsole
{

    /// <summary>
    /// https://www.nuget.org/packages/FreeImage-dotnet-core
    /// Install-Package FreeImage-dotnet-core
    /// </summary>
    public class FreeImageCorePackageTester : ImagePackageTesterBase
    {
        public FreeImageCorePackageTester(string imagePath, string outputDir) : base(imagePath, outputDir)
        {
        }

        public override bool ProcessImage(string outputName, int maxDimension)
        {
            bool isValid = false;
            try
            {
                var outputPath = Path.Combine(_outputDir, outputName + ".jpg");

                //FreeImageAPI.FreeImage input = new FreeImageAPI.FreeImageBitmap();

                // TODO finish FreeImageCore resize 

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
