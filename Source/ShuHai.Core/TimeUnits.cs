﻿namespace ShuHai
{
    public static class TimeUnits
    {
        public const int NanosecondsPerMicrosecond = 1000;
        public const int NanosecondsPerMillisecond = NanosecondsPerMicrosecond * MicrosecondsPerMillisecond;
        public const int NanosecondsPerSecond = NanosecondsPerMillisecond * MillisecondsPerSecond;
        public const int NanosecondsPerTimeSpanTick = 100;

        public const int MicrosecondsPerMillisecond = 1000;
        public const int MicrosecondsPerSecond = MicrosecondsPerMillisecond * MillisecondsPerSecond;

        public const int MillisecondsPerSecond = 1000;
        public const int MillisecondsPerMinute = SecondsPerMinute * MillisecondsPerSecond;
        public const int MillisecondsPerHour = SecondsPerHour * MillisecondsPerSecond;
        public const int MillisecondsPerDay = SecondsPerDay * MillisecondsPerSecond;

        public const int SecondsPerMinute = 60;
        public const int SecondsPerHour = SecondsPerMinute * MinutesPerHour;
        public const int SecondsPerDay = SecondsPerHour * HoursPerDay;

        public const int MinutesPerHour = 60;
        public const int MinutesPerDay = MinutesPerHour * HoursPerDay;

        public const int HoursPerDay = 24;

        public const int TimeSpanTicksPerMicrosecond = NanosecondsPerMicrosecond / NanosecondsPerTimeSpanTick;
        public const int TimeSpanTicksPerMillisecond = TimeSpanTicksPerMicrosecond * MicrosecondsPerMillisecond;
        public const int TimeSpanTicksPerSecond = TimeSpanTicksPerMillisecond * MillisecondsPerSecond;


        public static int MicrosecondsToTimeSpanTicks(int microseconds)
        {
            return microseconds * TimeSpanTicksPerMicrosecond;
        }

        public static int MillisecondsToTimeSpanTicks(int milliseconds)
        {
            return milliseconds * TimeSpanTicksPerMillisecond;
        }

        public static int SecondsToTimeSpanTicks(int seconds) { return seconds * TimeSpanTicksPerSecond; }

        public static (int Days, int Hours, int Minutes, int Seconds, int Milliseconds)
            MillisecondsToDayHourMinuteSecondMillisecond(long milliseconds)
        {
            int days = (int)(milliseconds / MillisecondsPerDay);
            int hours = (int)(milliseconds % MillisecondsPerDay) / MillisecondsPerHour;
            int minutes = (int)(milliseconds % MillisecondsPerHour) / MillisecondsPerMinute;
            int seconds = (int)(milliseconds % MillisecondsPerMinute) / MillisecondsPerSecond;
            int ms = (int)(milliseconds % MillisecondsPerSecond);
            return (days, hours, minutes, seconds, ms);
        }
    }
}