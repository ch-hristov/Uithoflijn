using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Uithoflijn
{
    public class Program
    {
        /// <summary>
        /// Enable this to track the results
        /// </summary>
        private const int TEST_COUNTS = 10;
        public static bool DEBUG = false;

        public const int TOTAL_TESTED_FREQUENCIES = TEST_COUNTS;
        public const int TOTAL_TRAMSCOUNT_TO_TEST = TEST_COUNTS;
        public const int TURNAROUND_TIME_TESTS = TEST_COUNTS;

        public const int FREQ_TEST_FREQ = TURNAROUND_TIME_TEST_FREQ * 3;

        public static int MIN_FREQ = (int)TimeSpan.FromMinutes(1).TotalSeconds;

        public const int AT_LEAST_COUNT_TRAMS = 3;

        public const int TURNAROUND_TIME_MIN = 300;
        public const int TURNAROUND_TIME_TEST_FREQ = 15;

        public static void Main(string[] args)
        {
            //TODO: do stuff with this??
            var validationData = ValidationFileReader.ReadValidationFolder();
            //every item in validationData contains the information for a file.
            //1-st item for example has n items which correspond to the rows in the file
            //2-nd same etc,, ffor another file.. you can decide what to do with them but 
            //make sure u dont break anything

            //The values of the frequencies we're testing, issue at least every 40 seconds(otherwise we issue waaaay too fast)
            var testFrequencies = new List<int>();
            var turnAroundTimes = new List<int>();

            for (var i = 0; i < TOTAL_TESTED_FREQUENCIES; i++) { testFrequencies.Add(MIN_FREQ + (i * FREQ_TEST_FREQ)); }
            for (var i = 0; i < TURNAROUND_TIME_TESTS; i++) { turnAroundTimes.Add(TURNAROUND_TIME_MIN + (i * TURNAROUND_TIME_TEST_FREQ)); }

            //check if debugger is attached to guarantee nice debugging
            if (System.Diagnostics.Debugger.IsAttached)
                DEBUG = true;

            //The values for the tram counts we're testing
            var tramNumbers = Enumerable.Range(AT_LEAST_COUNT_TRAMS, TOTAL_TRAMSCOUNT_TO_TEST);
            var output = new ConcurrentBag<string>();

            var optimalFrequency = 40;
            var optimalTramCount = 0;
            var optimalPassCount = 0;

            var testValues = new List<Tuple<int, int, int>>();

            foreach (var tramFrequency in testFrequencies)
                foreach (var tramCount in tramNumbers)
                    foreach (var turnAroundFreq in turnAroundTimes)
                        testValues.Add(new Tuple<int, int, int>(tramFrequency, tramCount, turnAroundFreq));

            var total = testValues.Count;
            var progress = 0.0;
            var header = "";

            Parallel.ForEach(testValues, new ParallelOptions()
            {
                MaxDegreeOfParallelism = DEBUG ? 1 : 8
            }, tuple =>
              {
                  var tramFrequency = tuple.Item1;
                  var tramCount = tuple.Item2;
                  var turnAroundTime = tuple.Item3;
                  var debugFile = $"DEBUG_{tramFrequency}_{tramCount}_{turnAroundTime}.txt";

                  if (DEBUG)
                  {
                      if (File.Exists(debugFile))
                      {
                          File.Delete(debugFile);
                      }
                      //guarantee the file is deleted..
                      Thread.Sleep(50);
                  }

                  using (var file = File.OpenWrite(debugFile))
                  {
                      using (var streamWriter = new StreamWriter(file))
                      {
                          var sm = new StateManager();

                          sm.DebugLine += (send, arg) =>
                          {
                              if (DEBUG)
                                  if (!string.IsNullOrEmpty(arg.ToString().Trim()))
                                      streamWriter.WriteLine(arg.ToString());
                          };

                          var statistics = sm.Start(turnAroundTime, tramFrequency, tramCount, DEBUG);

                          if (string.IsNullOrEmpty(header)) header = statistics.GetHeader();

                          var data = $"{turnAroundTime};{tramFrequency};{tramCount};{statistics.ToString()}";

                          output.Add(data);

                          if (statistics.TotalPassengersServiced > optimalPassCount)
                          {
                              optimalPassCount = statistics.TotalPassengersServiced;
                              optimalFrequency = tramFrequency;
                              optimalTramCount = tramCount;
                          }
                      }
                  }
                  // delete temp stuff if debug mode is off X_X
                  if (!DEBUG) if (File.Exists(debugFile)) File.Delete(debugFile);

                  progress++;
                  Console.WriteLine($"Progress : {Math.Round(progress / total * 100, 1)}%");
              });

            var final = new List<string>(output);
            final.Insert(0, string.Concat("q;freq;tramcnt;", header));
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
            var dt1 = new DateTime(01, 1, 1, 1, 1, 1);
        }
    }
}
