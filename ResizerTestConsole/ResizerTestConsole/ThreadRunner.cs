using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ResizerTestConsole
{
    public class ThreadRunner
    {
        private int _successCount;

        public int SuccessCount
        {
            get { return _successCount; }
        }

        private ConcurrentBag<ErrorLogStruct> _errorLog;

        private Func<string, int, bool> _fireMethod;
        public ThreadRunner(Func<string, int, bool> fireMethod,          ConcurrentBag<ErrorLogStruct> errorLog)
        {
            _fireMethod = fireMethod;
            _errorLog = errorLog;
            _successCount = 0;
        }

        public List<Thread> RunThreads(int iteration, int threadCount, int dimension)
        {
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < threadCount; i++)
            {

                string newImg = Guid.NewGuid().ToString();
                string threadName = string.Format("ThreadId: {0} Dim: {1}  Image:{2}", i, dimension, newImg);
                try
                {
                    Thread th = new Thread(() => InvokeWork(newImg, dimension));
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

        private void InvokeWork(string newPath, int dimension)
        {
            var result = _fireMethod(newPath, dimension);
            if (result)
            {
                System.Threading.Interlocked.Increment(ref _successCount);
            }
        }



        public async Task<bool> RunTasks(int iteration, int threadCount, int dimension)
        {
            bool runall = false;
            try
            {
                var tasks = new Task[threadCount];
                for (int i = 0; i < threadCount; i++)
                {
                    Console.WriteLine("Invoking interation {0} thread {1} dimension {2}", iteration, i, dimension);
                    string newImg = Guid.NewGuid().ToString();
                    tasks[i] = Task.Factory.StartNew(await InvokeTask(newImg, dimension));
                }
                runall = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
            return runall;
        }

        private async Task<Action> InvokeTask(string newpath, int dimension)
        {
            try
            {
                var result = _fireMethod(newpath, dimension);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return null;
        }

    }
}
