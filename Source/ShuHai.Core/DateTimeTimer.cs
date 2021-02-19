using System;
using System.Timers;

namespace ShuHai
{
    public class DateTimeTimer
    {
        public event Action<DateTimeTimer, DateTime> Elapsed;

        public bool Enabled { get => _timer.Enabled; set => _timer.Enabled = value; }

        public DateTime Time { get; }

        public DateTimeTimer(DateTime time, bool enabled = true)
        {
            Time = time;
            UpdateTimer();
            Enabled = enabled;
        }

        private Timer _timer;

        private void UpdateTimer()
        {
            var span = Time - DateTime.Now;
            if (span <= TimeSpan.Zero)
                span = new TimeSpan(0, 0, 0, 0, 1);

            _timer = new Timer(span.TotalMilliseconds) { AutoReset = false };
            _timer.Elapsed += OnTimerElapsed;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e) { Elapsed?.Invoke(this, Time); }

        public override string ToString() { return $"{GetType()}({Time})"; }
    }
}
