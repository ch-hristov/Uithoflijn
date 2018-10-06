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
    }
}
