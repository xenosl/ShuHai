using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShuHai.Unity.Demo
{
    public class Timers : MonoBehaviour
    {
        public KeyCode FireKey = KeyCode.Return;

        public List<int> ElapseSeconds;

        private void ResetTimers()
        {
            Debug.Log($"ResetTimers");

            foreach (var s in ElapseSeconds)
            {
                var time = DateTime.Now + new TimeSpan(0, 0, 0, s);
                var timer = new DateTimeTimer(time);
                timer.Elapsed += OnTimerElapsed;
            }
        }

        private void OnTimerElapsed(DateTimeTimer timer, DateTime time)
        {
            timer.Elapsed -= OnTimerElapsed;

            Debug.Log($"{timer} Fired");
        }

        private void Update()
        {
            if (Input.GetKeyDown(FireKey))
                ResetTimers();
        }
    }
}
