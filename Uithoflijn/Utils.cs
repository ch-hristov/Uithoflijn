using System;
using System.Collections.Generic;
using System.Text;

namespace Uithoflijn
{
    public static class Utils
    {
        /// <summary>
        /// TODO: Retrieve current time of day.
        /// </summary>
        /// <param name="currTime"></param>
        /// <returns></returns>
        public static TimeFrame GetTimeFrameFromSeconds(int currTime)
        {
            return TimeFrame.General;
        }

        public static int TimeToSeconds(DateTime time)
        {
            return (int)(time - new DateTime(time.Year, time.Day, time.Day, 6, 30, 0)).TotalSeconds;
        }

        public static DateTime GetForMinuteHour(int hour, int minute)
        {
            return new DateTime(2000, 1, 1, hour, minute, 0);
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
