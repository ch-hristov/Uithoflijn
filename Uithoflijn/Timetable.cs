using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uithoflijn
{
    /// <summary>
    /// This is used to model the frequency of different times of the day (morning, normal, evening)
    /// </summary>
    public class FrequencyInterval
    {
        public int Start { get; set; }

        public int End { get; set; }

        public int Frequency { get; set; }
    }

    public class Timetable : List<int>
    {

        public override string ToString()
        {
            return Schedule.Select(x => x.ToString()).Aggregate((a, b) => a + "|" + b);
        }

        public List<int> Schedule = new List<int>();

        public Timetable(IEnumerable<FrequencyInterval> intervals,
                         int offset,
                         int range = 54000)
        {
            Range = range;
            Offset = offset;
            FrequencyIntervals = intervals;

            // initialize all to 0
            for (var i = 0; i < range; i++) Add(0);

            foreach (var interval in FrequencyIntervals)
            {
                var start = interval.Start;
                var end = interval.End;
                var freq = interval.Frequency;

                for (int i = start + offset; i <= end; i += freq)
                {
                    this[i] = 1;
                    Schedule.Add(i);
                }
            }
        }


        public int Range { get; }

        public int Offset { get; }

        public IEnumerable<FrequencyInterval> FrequencyIntervals { get; }

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

        public int GetNextDeparture(Tram tram)
        {
            var departTime = tram.DepartureFromTerminal;

            for (int i = departTime; i < this.Count; i++)
            {
                if (this[i] == 1) return i + 1;
            }

            throw new Exception();
        }
    }
}
