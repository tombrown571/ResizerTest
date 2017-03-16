using ResizerServiceTest.Common;
using ResizerServiceTest.ServiceTest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ResizerServiceTest
{
    class Program
    {

        private static int _runCount = 1;
        private static string _ProgName = "ResizerServiceTest";
        private static string[] _testLibs = new[]
        {"ImageResizer", "ImageProcessor"};

        private static string[] _presets = new[]
        //{"full" };
        {"full", "large", "medium", "small", "thumb"};

    private static string _inputDir = @"..\..\TestImages\";
        private static string _outputDir = Path.Combine(_inputDir, "TestOutput");
        private static string _logfile = Path.Combine(_outputDir, "TestRun.log");


        static void Main(string[] args)
        {
            bool interactiveMode = false;
            string imageType = "";

            #region Command Line Args

            if (args.Length <= 0)
            {
                UsageExit();
            }
            if (!_testLibs.Contains(args[0]))
            {
                Console.WriteLine("{0} Not a valid Library to test", args[0]);
                UsageExit();
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
                        UsageExit();
                    }
                    _runCount = argThreads;
                }
            }

            Setup();
            string[] ListImages = GetTestImageNames();

            #endregion

            Console.WriteLine("**** ImagePackage Testing: '{0}' multithreaded testing ****", imageType);


            ConcurrentBag<ErrorLogStruct> threadErrors = new ConcurrentBag<ErrorLogStruct>();
            var processors = new List<IImageServiceTester>();
            foreach (var currentFile in ListImages)
            {
                foreach (var preset in _presets)
                {
                    switch (imageType.ToLower())
                    {
                        case "imageprocessor":
                            var imageProcessorServiceTester = new ImageProcessorServiceTester(preset, currentFile);
                            processors.Add(imageProcessorServiceTester);
                            break;
                    }
                }
            }
            var threadRunners = new List<ThreadRunner>();

            var watch = System.Diagnostics.Stopwatch.StartNew();
            List<Thread> allThreads = new List<Thread>();
            foreach (var p in processors.Select((value, index) => new { value, index }))
            {
                Console.WriteLine("Call Process {0} threads {1} ", p.index, _runCount);
                ThreadRunner threadRunner = new ThreadRunner(p.value.GetRequest, threadErrors);
                threadRunners.Add(threadRunner);
                var threads = threadRunner.RunThreads(p.index, _runCount);
                allThreads.AddRange(threads);
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
            foreach (IImageServiceTester imagePackageTester in processors)
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
            var finishMessage =
                string.Format("*** Finished in {0} milliseconds: Success Count: {1} Exception Count {2} *** ", elapsed,
                    successCount, memexCount);
            var finishMessage2 = string.Format("*** Run Details: Processor: {0}  Threads: {1} *** ", imageType,
                _runCount);
            foreach (var exceptionType in exceptionTypes)
            {
                exceptionLog.AppendFormat("** {0} - Count = {1}{2}", exceptionType.Key, exceptionType.Value,
                    Environment.NewLine);
            }
            Console.WriteLine(finishMessage);
            Console.WriteLine(finishMessage2);
            exceptionLog.AppendLine(finishMessage);
            exceptionLog.AppendLine(finishMessage2);
            using (var fs = new StreamWriter(new FileStream(_logfile, FileMode.CreateNew)))
            {
                fs.WriteLine(exceptionLog.ToString());
            }
            if (interactiveMode)
            {
                Process.Start("notepad.exe", _logfile);
                Console.WriteLine("** Any key to Exit ");
                Console.ReadKey();
            }
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
            Console.WriteLine(
                "     : {0} [<Interactive (Y|True|1|N|False|0)>,]  'optional interactive mode: default No'",
                "".PadLeft(_ProgName.Length));
            Console.WriteLine("     : {0} [<Threads (Number)>,]  'optional Threads per processor: default {1}'",
                "".PadLeft(_ProgName.Length), _runCount);
        }
        private static void UsageExit()
        {
            Usage();
            ExitMessage("");
        }

        private static void ExitMessage(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("** Any key to Exit ");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static void Setup()
        {
            DirectoryInfo di = new DirectoryInfo(_inputDir);
            if (!Directory.Exists(_inputDir))
            {
                ExitMessage($"Test Image directory '{ di.FullName}' Not Found");
            }
            if (!Directory.Exists(_outputDir))
            {
                Directory.CreateDirectory(_outputDir);
            }
            else
            {
                DirectoryInfo dout = new DirectoryInfo(_outputDir);
                foreach (FileInfo file in dout.GetFiles())
                {
                    file.Delete();
                }
            }
        }


        private static string[] GetTestImageNames()
        {
            // for expediency - taken direct from the blob site 
            string[] blogImages = new string[]
            {
             "0005156d-b585-431d-92cd-e76f964bfd13.jpg",
             "0071ead2-2a60-4e95-915e-2f97d2c21ad5.jpg",
             "00cf0473-78cd-4d7f-9c64-77bb2962967a.jpg",
             "011e025b-d72f-45b7-9f88-33249e66a8c4.jpg",
             "018f578c-ea32-4624-93fe-cd5e13d88a41.jpg",
             "01cb4d40-4b74-4e33-a56e-3e93cfcf99af.jpg",
             "035d74e3-5b65-4133-bb21-db9c7768b69d.jpg",
             "0216a238-5629-480b-b802-1ff23879b321.jpg",
             "02ed5bb6-615a-45e4-966b-f2116ca40d08.jpg",
             "015f2239-7d40-408a-b347-32c6ccce5172.jpg",
             "037b5dd9-cd5a-490d-aafe-7640de8eba6b.jpg"
            };
            return blogImages;
        }
    }
}