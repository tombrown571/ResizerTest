﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ResizerServiceTest.Common
{
    /// <summary>
    /// Run the same method on multiple threads
    /// this should expose any threading issues with the method
    /// If it runs OK, increase the threadcount until the errors appear
    /// (or it takes too long to complete.)
    /// </summary>
    public class ThreadRunnerBool
    {
        private int _successCount;

        public int SuccessCount
        {
            get { return _successCount; }
        }

        private ConcurrentBag<ErrorLogStruct> _errorLog;

        private Func<bool> _fireMethod;
        public ThreadRunnerBool(Func<bool> fireMethod, ConcurrentBag<ErrorLogStruct> errorLog)
        {
            _fireMethod = fireMethod;
            _errorLog = errorLog;
            _successCount = 0;
        }

        public List<Thread> RunThreads(int iteration, int threadCount)
        {
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < threadCount; i++)
            {
                string threadName = string.Format("ThreadId: {0}", i);
                try
                {
                    Thread th = new Thread(() => InvokeWork());
                    threads.Add(th);
                    th.Name = threadName;
                    th.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Thread Creation Exception Name: {2}:  {0}\n{1}", ex.Message, ex.StackTrace, threadName);
                    _errorLog.Add(new ErrorLogStruct()
                    {
                        Ex = ex,
                        LogText = threadName,
                    });
                }

            }
            return threads;
        }

        private void InvokeWork()
        {
            var result =  _fireMethod();
            if (result)
            {
                System.Threading.Interlocked.Increment(ref _successCount);
            }
        }



        //public async Task<bool> RunTasks(int iteration, int threadCount)
        //{
        //    bool runall = false;
        //    try
        //    {
        //        var tasks = new Task[threadCount];
        //        for (int i = 0; i < threadCount; i++)
        //        {
        //            Console.WriteLine("Invoking interation {0} thread {1} dimension {2}", iteration, i);
        //            tasks[i] = Task.Factory.StartNew(await InvokeTask());
        //        }
        //        runall = true;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);

        //    }
        //    return runall;
        //}

        //private async Task<Action> InvokeTask()
        //{
        //    try
        //    {
        //         _fireMethod();
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        throw;
        //    }
        //    return null;
        //}

    }

}
