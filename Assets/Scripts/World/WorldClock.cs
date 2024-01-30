using System;
using UnityEngine;

namespace World
{
    public class WorldClock
    {
        public event Action HourElapsed;
        public event Action DayElapsed;

        private int _hour;
        private int _day = 1;
        private float _currentSeconds;
        private const float TimeScale = 15f;

        public void UpdateClock()
        {
            _currentSeconds += Time.deltaTime * TimeScale;

            if (_currentSeconds < 3600f) return;
            _currentSeconds = 0f;
            _hour++;
            HourElapsed?.Invoke();
            
            if (_hour < 24) return;
            _hour = 0;
            _day++;
            DayElapsed?.Invoke();
        }
    }
}