using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Uithoflijn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            bool debug = false;
            //The values of the frequencies we're testing
            var tramFrequencies = Enumerable.Range(40, 90);

            //The values for the tram counts we're testing
            var tramNumbers = Enumerable.Range(1, 6);

            var output = new List<string>();

            var optimalFrequency = 40;
            var optimalTramCount = 0;
            var optimalPassCount = 0;

            output.Add("freq;tramcnt;delay;punctuality;serviced");

            List<Tuple<int, int>> testValues = new List<Tuple<int, int>>();

            foreach (var tramFrequency in tramFrequencies)
            {
                foreach (var tramCount in tramNumbers)
                {
                    testValues.Add(new Tuple<int, int>(tramFrequency, tramCount));
                }
            }


            Parallel.ForEach(testValues, new ParallelOptions() { MaxDegreeOfParallelism = debug ? 1 : 8 }, tuple =>
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


            File.WriteAllLines("output.csv", output);
            Console.WriteLine(output.Aggregate((a, b) => a + Environment.NewLine + b));
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
