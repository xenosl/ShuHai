using System;
using System.Timers;

namespace ShuHai
{
    public class DateTimeTimer
    {
        public event Action<DateTimeTimer, DateTime> Elapsed;

        public bool Enabled { get => _timer.Enabled; set => _timer.Enabled = value; }

        public DateTime Time { get; }

        public DateTimeTimer(DateTime time)
        {
            Time = time;
            UpdateTimer();
        }

        private Timer _timer;

        private void UpdateTimer()
        {
            var span = Time > DateTime.Now ? Time - DateTime.Now : TimeSpan.Zero;
            _timer = new Timer(span.Milliseconds) { AutoReset = false };
            _timer.Elapsed += OnTimerElapsed;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e) { Elapsed?.Invoke(this, Time); }
    }
}