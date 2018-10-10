using System;
using System.Collections.Generic;
using System.Text;

namespace Uithoflijn
{
    public static class Utils
    {
        /// <summary>
        /// Retrieve current time of day.
        /// </summary>
        /// <param name="currTime"></param>
        /// <returns>Returns 1/1/01 and the time of the day 6:00 AM, etc...</returns>
        public static DateTime SecondsToDateTime(int currTime)
        {
            TimeSpan mytime = TimeSpan.FromSeconds(6 * 60 * 60 + currTime);
            DateTime dt = new DateTime();
            DateTime secondsToDateTime = dt.Add(mytime);

            return secondsToDateTime;
        }

        public static int TimeToSeconds(DateTime time)
        {
            return (int)(time - new DateTime(time.Year, time.Day, time.Day, 6, 30, 0)).TotalSeconds;
        }

        public static DateTime GetForMinuteHour(int hour, int minute)
        {
            return new DateTime(01, 1, 1, hour, minute, 0);
        }

        public static DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            var modTicks = dt.Ticks % d.Ticks;
            var delta = modTicks != 0 ? d.Ticks - modTicks : 0;
            return new DateTime(dt.Ticks + delta, dt.Kind);
        }

        public static DateTime RoundDown(DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            return new DateTime(dt.Ticks - delta, dt.Kind);
        }

        public static DateTime RoundToNearest(DateTime dt, TimeSpan d)
        {
            var delta = dt.Ticks % d.Ticks;
            bool roundUp = delta > d.Ticks / 2;
            var offset = roundUp ? d.Ticks : 0;

            return new DateTime(dt.Ticks + offset - delta, dt.Kind);
        }
    }
}
