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

            
            string[] ListImages = Directory.GetFiles(@"..\..\TestImages\", "*.jpg");

            var processors = new List<ImageProcessor>();

            foreach (var item in ListImages)
            {
                processors.Add(new ImageProcessor(item));
            }

            //ImageProcessor imgProcessor = new ImageProcessor(@"..\..\TestImages\Earth_house.jpg");
            //ThreadRunner threadRunner = new ThreadRunner(imgProcessor.ProcessImage);
            //threadRunner.RunThreads(10);

            foreach (var processor in processors)
            {
                ThreadRunner threadRunner = new ThreadRunner(processor.ProcessImage);
                threadRunner.RunThreads(10);
            }


            Console.WriteLine("*** Finished any key to quit *** ");
            Console.ReadKey();
        }
    }
}
