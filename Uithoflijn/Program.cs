using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Uithoflijn
{
    public class Program
    {
        /// <summary>
        /// Enable this to track the results
        /// </summary>
        public const bool DEBUG = false;

        public static void Main(string[] args)
        {

            //The values of the frequencies we're testing
            var tramFrequencies = Enumerable.Range(40, 3);

            //The values for the tram counts we're testing
            var tramNumbers = Enumerable.Range(1, 6);

            var output = new ConcurrentBag<string>();

            var optimalFrequency = 40;
            var optimalTramCount = 0;
            var optimalPassCount = 0;



            var testValues = new List<Tuple<int, int>>();

            foreach (var tramFrequency in tramFrequencies)
                foreach (var tramCount in tramNumbers)
                    testValues.Add(new Tuple<int, int>(tramFrequency, tramCount));

            Parallel.ForEach(testValues, new ParallelOptions() { MaxDegreeOfParallelism = DEBUG ? 1 : 8 }, tuple =>
              {
                  var tramFrequency = tuple.Item1;
                  var tramCount = tuple.Item2;
                  var sm = new StateManager();

                  Console.WriteLine($"Executing freq: {tramFrequency}; tram count: {tramCount}");
                  var statistics = sm.Start(tramFrequency, tramCount);
                  var data = $"{tramFrequency};{tramCount};{statistics.ToString()}";

                  output.Add(data);
                  Console.WriteLine(statistics.ToString());

                  if (statistics.TotalPassengersServiced > optimalPassCount)
                  {
                      optimalPassCount = statistics.TotalPassengersServiced;
                      optimalFrequency = tramFrequency;
                      optimalTramCount = tramCount;
                  }
              });

            var final = new List<string>(output);
            final.Insert(0, "freq;tramcnt;delay;punctuality;serviced");

            File.WriteAllLines("output.csv", final);
            Console.WriteLine(final.Aggregate((a, b) => a + Environment.NewLine + b));
        }

        public static double ComputeCycleLength()
        {
            var dt = new DateTime(01, 1, 1, 6, 30, 0);
            var dt2 = new DateTime(01, 1, 1, 21, 30, 0);
            return 0;
        }

        public void ComputeIntervalsInDay()
        {
            //TODO>>
            var dt1 = new DateTime(01, 1, 1, 1, 1, 1);
        }

    }
}
