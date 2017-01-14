using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ResizerTestConsole
{
    public class ThreadRunner
    {
        private Func<string, bool> _fireMethod;
        public ThreadRunner(Func<string, bool> fireMethod)
        {
            _fireMethod = fireMethod;
        }

        public bool RunThreads(int threadCount)
        {
            bool runall = false;
            try
            {
                for (int i = 0; i < threadCount; i++)
                {
                    Console.WriteLine("Invoking thread {0}", i);

                    string newImg = Guid.NewGuid().ToString();

                    Thread th = new Thread(() => InvokeWork(newImg));
                    th.Start();
                }
                runall = true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: {0}\n{1}", ex.Message, ex.StackTrace);
            }
            return runall;
        }

        private void InvokeWork(string newPath)
        {
            var result = _fireMethod(newPath);
        }
    }
}
