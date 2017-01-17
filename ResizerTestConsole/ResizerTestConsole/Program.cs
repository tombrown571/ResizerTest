using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ResizerTestConsole
{
    class Program
    {

        private static int _runCount = 10;
        private static string _ProgName = "ResizerTestConsole";
        private static string _outputDir = @"..\..\..\TestImages\TestOutput";
        private static string _inputDir = @"..\..\..\TestImages\";
        private static string _logfile = @"..\..\..\TestImages\TestOutput\TestRun.log";

        private static string[] _testLibs = new[] { "ImageResizer", "ImageProcessor", "ImageSharp" };
        private static string[] _imageSharpAlgorithms = new[] { "Bicubic", "Triangle", "NearestNeighbor", "MitchellNetravali" };
        private static string _imageSharpAlgorithm = "NearestNeighbor";

        /// <summary>
        /// This Main() is for running the stress test
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            bool interactiveMode = false;
            string imageType = "";

            #region Command Line Args
            if (args.Length <= 0)
            {
                Usage();
                return;
            }
            if (!_testLibs.Contains(args[0]))
            {
                Console.WriteLine("{0} Not a valid Library to test", args[0]);
                Usage();
                return;
            }
            else
            {
                imageType = args[0];
            }
            if (args.Length > 1)
            {
                switch (args[1].ToLower())
                {
                    case "y":
                    case "true":
                    case "1":
                        interactiveMode = true;
                        break;
                    case "n":
                    case "false":
                    case "0":
                        interactiveMode = false;
                        break;
                    default:
                        Console.WriteLine("Unknown interactive mode '{0}' assuming false", args[1]);
                        break;
                }
                if (args.Length > 2)
                {
                    int argThreads;
                    if (!int.TryParse(args[2], out argThreads))
                    {
                        Console.WriteLine("{0} Not a valid number of threads", args[2]);
                        Usage();
                        return;
                    }
                    _runCount = argThreads;
                    if (args.Length > 3 && imageType == "ImageSharp")
                    {
                        if (!_imageSharpAlgorithms.Contains(args[3]))
                        {
                            Console.WriteLine("{0} Not a valid ImageSharp Algorithm");
                            Usage();
                            return;
                        }
                        _imageSharpAlgorithm = args[3];
                    }
                }
            }

            #endregion

            Console.WriteLine("**** ImagePackage Testing: '{0}' multithreaded testing ****", imageType);

            ClearOutputDirectory();

            string[] ListImages = Directory.GetFiles(_inputDir, "*.jpg");
            ConcurrentBag<ErrorLogStruct> threadErrors = new ConcurrentBag<ErrorLogStruct>();
            var processors = new List<IImagePackageTester>();
            foreach (var item in ListImages)
            {
                switch (imageType.ToLower())
                {
                    case "imageresizer":
                        processors.Add(new ImageResizerPackageTester(item, _outputDir));
                        break;
                    case "imageprocessor":
                        processors.Add(new ImageProcessorPackageTester(item, _outputDir));
                        break;
                    case "imagesharp":
                        var imageSharpPackageTester = new ImageSharpPackageTester(item, _outputDir);
                        if (!string.IsNullOrWhiteSpace(_imageSharpAlgorithm))
                        {
                            imageSharpPackageTester.ResamplerAlgorithm = _imageSharpAlgorithm;
                        }
                        processors.Add(imageSharpPackageTester);
                        break;
                }
            }
            var threadRunners = new List<ThreadRunner>();
            List<int> imgDims = new List<int>() { 200, 320, 640, 768, 1024, 1080 };

            var watch = System.Diagnostics.Stopwatch.StartNew();
            List<Thread> allThreads = new List<Thread>();
            // run will each dimension on each ImageResizerPackageTester object
            foreach (var dim in imgDims)
            {
                foreach (var p in processors.Select((value, index) => new { value, index }))
                {
                    Console.WriteLine("Call Process {0} threads {1} dimension {2}", p.index, _runCount, dim);
                    ThreadRunner threadRunner = new ThreadRunner(p.value.ProcessImage, threadErrors);
                    threadRunners.Add(threadRunner);
                    var threads = threadRunner.RunThreads(p.index, _runCount, dim);
                    allThreads.AddRange(threads);
                }
            }

            // wait for all threads to finish,
            foreach (var thread in allThreads)
            {
                if (thread.IsAlive)
                {
                    thread.Join();
                }
            }
            int successCount = 0;
            foreach (var threadRunner in threadRunners)
            {
                successCount += threadRunner.SuccessCount;
            }

            watch.Stop();
            var elapsed = watch.ElapsedMilliseconds;
            int memexCount = 0;
            StringBuilder exceptionLog = new StringBuilder("*** Image Processing Run Exception Log ***");
            Dictionary<string, int> exceptionTypes = new Dictionary<string, int>();
            foreach (IImagePackageTester imagePackageTester in processors)
            {
                foreach (var errLog in imagePackageTester.Exceptions)
                {
                    memexCount = MemexCount(errLog, exceptionLog, memexCount, exceptionTypes);
                }
            }
            foreach (var errorLogStruct in threadErrors)
            {
                memexCount = MemexCount(errorLogStruct, exceptionLog, memexCount, exceptionTypes);

            }
            var finishMessage = string.Format("*** Finished in {0} milliseconds: Success Count: {1} Exception Count {2} *** ", elapsed, successCount, memexCount);
            var finishMessage2 = string.Format("*** Run Details: Processor: {0}  Threads: {1} {2}  *** ", imageType, _runCount, (imageType == "ImageSharp" ? string.Format("Algorithm: {0}", _imageSharpAlgorithm) : ""));
            foreach (var exceptionType in exceptionTypes)
            {
                exceptionLog.AppendFormat("** {0} - Count = {1}{2}", exceptionType.Key, exceptionType.Value,
                    Environment.NewLine);
            }
            Console.WriteLine(finishMessage);
            Console.WriteLine(finishMessage2);
            exceptionLog.AppendLine(finishMessage);
            exceptionLog.AppendLine(finishMessage2);
            using (var fs = new StreamWriter(_logfile))
            {
                fs.WriteLine(exceptionLog.ToString());
            }
            if (interactiveMode)
            {
                Process.Start(_logfile);
                Console.WriteLine("** Any key to Exit ");
                Console.ReadKey();
            }
        }


        /// <summary>
        /// This Main() is for debugging a single package 
        /// </summary>
        /// <param name="args"></param>
        static void MainTest(string[] args)
        {
            ClearOutputDirectory();
            // IImagePackageTester pTest = new ImageProcessorPackageTester( Path.Combine(_inputDir, "House.jpg" ), _outputDir);
            IImagePackageTester pTest = new ImageSharpPackageTester(Path.Combine(_inputDir, "House.jpg"), _outputDir);
            var testOut = Path.Combine(_outputDir, "TestOut.jpg");
            var success = pTest.ProcessImage(testOut, 500);
            Console.WriteLine("Success = {0}", success);
            Console.ReadKey();
        }

        private static int MemexCount(ErrorLogStruct errLog, StringBuilder exceptionLog, int memexCount,
            Dictionary<string, int> exceptionTypes)
        {
            var exception = errLog.Ex;
            exceptionLog.AppendFormat("{1}LOGTXT: {0}{1}", errLog.LogText, Environment.NewLine);
            exceptionLog.AppendFormat("{2}** Exception: {0}{2}* StackTrace: {1}{2}", exception.Message,
                exception.StackTrace, Environment.NewLine);
            memexCount++;
            if (exceptionTypes.ContainsKey(exception.Message))
            {
                exceptionTypes[exception.Message] = exceptionTypes[exception.Message] + 1;
            }
            else
            {
                exceptionTypes.Add(exception.Message, 1);
            }
            return memexCount;
        }


        private static void Usage()
        {
            Console.WriteLine("USAGE: {0} <TestLib, ({1})>,  'Library to test'", _ProgName, string.Join("|", _testLibs));
            Console.WriteLine("     : {0} [<Interactive (Y|True|1|N|False|0)>,]  'optional interactive mode: default No'", "".PadLeft(_ProgName.Length));
            Console.WriteLine("     : {0} [<Threads (Number)>,]  'optional Threads per processor: default {1}'", "".PadLeft(_ProgName.Length), _runCount);
            Console.WriteLine("     : {0} [<Algorithm,  ({1})>]  'optional ImageSharp Resampler Algorithm: default {2}'", "".PadLeft(_ProgName.Length), string.Join("|", _imageSharpAlgorithms), _imageSharpAlgorithm);
        }

        private static void ClearOutputDirectory()
        {
            DirectoryInfo di = new DirectoryInfo(_outputDir);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }
    }
}
