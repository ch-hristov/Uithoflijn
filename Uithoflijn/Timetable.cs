using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uithoflijn
{
    public class Timetable : List<int>
    {

        public Timetable(int frequency,
                         int offset,
                         int range = 54000)
        {
            Range = range;
            // initialize all to 0
            for (var i = 0; i < range; i++) Add(0);

            for (int i = offset; i < range; i += frequency)
                this[i] = 1;
        }

        public int Range { get; }

        public bool ShouldIssueAtTime(int triggerTime)
        {
            if (triggerTime < 0 || triggerTime >= Range) return false;
            return this[triggerTime] == 1;
        }

        public int? NextFromSchedule(int triggerTime)
        {
            if (triggerTime - 1 >= Range) return null;
            int j = triggerTime - 1;
            while (j < Range)
            {
                if (this[j] == 1) return j + 1;
                j++;
            }

            return null;
        }
    }
}
