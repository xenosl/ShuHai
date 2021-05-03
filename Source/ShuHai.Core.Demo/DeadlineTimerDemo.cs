using System;
using System.Threading;

namespace ShuHai.Core.Demo
{
    public class DeadlineTimerDemo : Demo
    {
        private AutoResetEvent _runWaitHandle;

        public override void Run()
        {
            using (_runWaitHandle = new AutoResetEvent(false))
            {
                using (new DeadlineTimer(DateTime.Now, OnElapsed, "Now"))
                    _runWaitHandle.WaitOne();
                using (new DeadlineTimer(DateTime.Now + new TimeSpan(0, 0, 3), OnElapsed, "3 sec"))
                    _runWaitHandle.WaitOne();
                using (new DeadlineTimer(DateTime.Now + new TimeSpan(0, 0, 3), OnElapsed, "6 sec"))
                    _runWaitHandle.WaitOne();
            }
        }

        private void OnElapsed(DeadlineTimer timer, DateTime deadline, object context)
        {
            var msg = (string)context;
            Console.WriteLine(msg);

            _runWaitHandle.Set();
        }
    }
}
