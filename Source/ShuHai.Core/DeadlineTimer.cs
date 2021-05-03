using System;
using System.Threading;

namespace ShuHai
{
    public class DeadlineTimer : IDisposable
    {
        public event Action<DeadlineTimer, DateTime, object> Elapsed;

        public DateTime Deadline { get; }

        public DeadlineTimer(DateTime deadline, Action<DeadlineTimer, DateTime, object> elapsed, object context = null)
        {
            Deadline = deadline;

            var duration = Deadline - DateTime.Now;
            if (duration <= TimeSpan.Zero)
                duration = new TimeSpan(0, 0, 0, 0, 1);
            _timer = new Timer(OnTimerCallback, context, duration, Timeout.InfiniteTimeSpan);

            Elapsed += elapsed;
        }

        public void Dispose()
        {
            _timer.Dispose();
            _timer = null;
        }

        private Timer _timer;

        private void OnTimerCallback(object context)
        {
            if (Elapsed == null)
                return;

            Elapsed.Invoke(this, Deadline, context);
            Elapsed = null;
        }

        public override string ToString() { return $"{GetType()}({Deadline})"; }
    }
}
