using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResizerTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("**** Image Resizer multithread testing ****");

            // clear output
            DirectoryInfo di = new DirectoryInfo(@"..\..\TestImages\TestOutput");
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }


            string[] ListImages = Directory.GetFiles(@"..\..\TestImages\", "*.jpg");

            var processors = new List<ImageProcessor>();

            foreach (var item in ListImages)
            {
                processors.Add(new ImageProcessor(item));
            }

            List<int> imgDims = new List<int>() { 200, 320, 640, 768, 1024, 1080 };


            foreach (var processor in processors)
            {
                ThreadRunner threadRunner = new ThreadRunner(processor.ProcessImage);
                // run will all dimensions
                foreach (var dim in imgDims)
                {
                    threadRunner.RunThreads(10, dim);
                }
            }


            Console.WriteLine("*** Finished any key to quit *** ");
            Console.ReadKey();
        }
    }
}
