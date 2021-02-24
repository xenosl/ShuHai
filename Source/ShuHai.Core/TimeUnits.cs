namespace ShuHai
{
    public static class TimeUnits
    {
        public const int NanosecondsPerMicrosecond = 1000;
        public const int NanosecondsPerMillisecond = NanosecondsPerMicrosecond * MicrosecondsPerMillisecond;
        public const int NanosecondsPerSecond = NanosecondsPerMillisecond * MillisecondsPerSecond;
        public const long NanosecondsPerMinute = (long)NanosecondsPerSecond * SecondsPerMinute;
        public const long NanosecondsPerHour = NanosecondsPerMinute * MinutesPerHour;
        public const long NanosecondsPerDay = NanosecondsPerHour * HoursPerDay;
        public const long NanosecondsPerWeek = NanosecondsPerDay * DaysPerWeek;

        public const int MicrosecondsPerMillisecond = 1000;
        public const int MicrosecondsPerSecond = MicrosecondsPerMillisecond * MillisecondsPerSecond;
        public const int MicrosecondsPerMinute = MicrosecondsPerSecond * SecondsPerMinute;
        public const long MicrosecondsPerHour = (long)MicrosecondsPerMinute * MinutesPerHour;
        public const long MicrosecondsPerDay = MicrosecondsPerHour * HoursPerDay;
        public const long MicrosecondsPerWeek = MicrosecondsPerDay * DaysPerWeek;

        public const int MillisecondsPerSecond = 1000;
        public const int MillisecondsPerMinute = MillisecondsPerSecond * SecondsPerMinute;
        public const int MillisecondsPerHour = MillisecondsPerMinute * MinutesPerHour;
        public const int MillisecondsPerDay = MillisecondsPerHour * HoursPerDay;
        public const int MillisecondsPerWeek = MillisecondsPerDay * DaysPerWeek;

        public const int SecondsPerMinute = 60;
        public const int SecondsPerHour = SecondsPerMinute * MinutesPerHour;
        public const int SecondsPerDay = SecondsPerHour * HoursPerDay;
        public const int SecondsPerWeek = SecondsPerDay * DaysPerWeek;

        public const int MinutesPerHour = 60;
        public const int MinutesPerDay = MinutesPerHour * HoursPerDay;
        public const int MinutesPerWeek = MinutesPerDay * DaysPerWeek;

        public const int HoursPerDay = 24;
        public const int HoursPerWeek = HoursPerDay * DaysPerWeek;

        public const int DaysPerWeek = 7;

        public const int NanosecondsPerTick = 100;
        public const int TicksPerMicrosecond = NanosecondsPerMicrosecond / NanosecondsPerTick;
        public const int TicksPerMillisecond = TicksPerMicrosecond * MicrosecondsPerMillisecond;
        public const int TicksPerSecond = TicksPerMillisecond * MillisecondsPerSecond;
        public const int TicksPerMinute = TicksPerSecond * SecondsPerMinute;
        public const long TicksPerHour = (long)TicksPerMinute * MinutesPerHour;
        public const long TicksPerDay = TicksPerHour * HoursPerDay;
        public const long TicksPerWeek = TicksPerDay * DaysPerWeek;
    }
}
