using System;
using System.Diagnostics;

namespace ShuHai.Unity
{
    /// <summary>
    ///     A alternative class to <see cref="System.Timers.Timer" /> that executed in main thread for using in Unity.
    /// </summary>
    public class Timer
    {
        public event Action Elapsed;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (value == _enabled)
                    return;

                _enabled = value;

                if (_enabled)
                {
                    _Update += Update;
                    _stopwatch.Start();
                }
                else
                {
                    _stopwatch.Reset();
                    _Update -= Update;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the interval, expressed in milliseconds, at which to raise the <see cref="Elapsed" /> event.
        /// </summary>
        public double Interval
        {
            get => _interval;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Invalid value: " + value);
                _interval = value;
            }
        }

        public bool AutoReset = true;

        public Timer() : this(100) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Timer" /> class, and sets the <see cref="Interval" /> property
        ///     to the specified number of milliseconds.
        /// </summary>
        /// <param name="interval">The time, in milliseconds, between events.</param>
        public Timer(double interval) { Interval = interval; }

        public void Start() { Enabled = true; }

        public void Stop() { Enabled = false; }

        private bool _enabled;
        private double _interval;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private void Update()
        {
            var elapsedSeconds = _stopwatch.Elapsed.TotalMilliseconds;
            if (elapsedSeconds < Interval)
                return;

            Elapsed?.Invoke();

            if (AutoReset)
            {
                _stopwatch.Reset();
                _stopwatch.Start();
            }
            else
            {
                Enabled = false;
            }
        }

        private static event Action _Update
        {
            add
            {
#if UNITY_EDITOR
                Root.EditorUpdate += value;
#else
                Root.Update += value;
#endif // UNITY_EDITOR
            }
            remove
            {
#if UNITY_EDITOR
                Root.EditorUpdate -= value;
#else
                Root.Update -= value;
#endif // UNITY_EDITOR
            }
        }
    }
}