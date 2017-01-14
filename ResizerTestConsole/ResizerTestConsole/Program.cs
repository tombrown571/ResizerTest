using System;
using System.Collections.Generic;
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

            // 
            ImageProcessor imgProcessor = new ImageProcessor(@"..\..\TestImages\Earth_house.jpg");

            ThreadRunner threadRunner = new ThreadRunner(imgProcessor.ProcessImage);

            threadRunner.RunThreads(10);


            Console.WriteLine("*** Finished any key to quite *** ");
            Console.ReadKey();
        }
    }
}
