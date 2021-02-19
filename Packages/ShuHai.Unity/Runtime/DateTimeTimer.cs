using System;
using System.Collections.Generic;
using UnityEngine.LowLevel;
using UnityEngine;

namespace ShuHai.Unity
{
    public class DateTimeTimer
    {
        public event Action<DateTimeTimer, DateTime> Elapsed;

        public bool Enabled { get => _timer.Enabled; set => _timer.Enabled = value; }

        public DateTime Time => _timer.Time;

        public DateTimeTimer(DateTime time, bool enabled = true)
        {
            _timer = new ShuHai.DateTimeTimer(time);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Enabled = enabled;
        }

        private readonly ShuHai.DateTimeTimer _timer;

        private void FireElapsed() { Elapsed?.Invoke(this, Time); }

        private void OnTimerElapsed(ShuHai.DateTimeTimer timer, DateTime time)
        {
            Debug.Assert(timer == _timer);

            lock (_elapsedInstancesLock)
                _elapsedInstances.Add(this);
        }

        public override string ToString() { return $"{GetType()}({Time})"; }

        #region Update

        private static readonly object _elapsedInstancesLock = new object();
        private static readonly List<DateTimeTimer> _elapsedInstances = new List<DateTimeTimer>();

        private static readonly PlayerLoopSystem _loopSystem = new PlayerLoopSystem
        {
            type = typeof(DateTimeTimer),
            updateDelegate = Update
        };

        private static void Update()
        {
            lock (_elapsedInstancesLock)
            {
                foreach (var inst in _elapsedInstances)
                    inst.FireElapsed();
                _elapsedInstances.Clear();
            }
        }

        private static void SetupUpdate()
        {
            var loop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopSystemUtil.AddSubSystem(ref loop, _loopSystem);
            PlayerLoop.SetPlayerLoop(loop);
        }

        #endregion Update

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        private static void Setup() { SetupUpdate(); }
    }
}
