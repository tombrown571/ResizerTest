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
        private Delegate _fireMethod;
        public ThreadRunner(Delegate fireMethod)
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
                    Thread th = new Thread(InvokeWork);
                }
                runall = true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: {0}\n{1}", ex.Message, ex.StackTrace);
            }
            return runall;
        }

        private void InvokeWork()
        {
            _fireMethod.DynamicInvoke(null);
        }
    }
}
