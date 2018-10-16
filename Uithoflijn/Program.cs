using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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

        public const int AT_LEAST_COUNT_TRAMS = 1;

        public const int TURNAROUND_TIME_MIN = 300;
        public const int TURNAROUND_TIME_TEST_FREQ = 15;

        public int LatenessThreshold { get; }

        public Program(int latenessThreshold = 60)
        {
            LatenessThreshold = latenessThreshold;
        }

        public static void Main(string[] args)
        {
            //The values of the frequencies we're testing, issue at least every 40 seconds(otherwise we issue waaaay too fast)
            var testFrequencies = new List<int>();
            var turnAroundTimes = new List<int>();
            var tramNumbers = new List<int>();

            const int sec = 30;

            for (var seconds = 15 * 60; seconds > 1 * 60; seconds -= sec) { testFrequencies.Add(seconds); }
            for (var seconds = 5 * 60; seconds > 2 * 60; seconds -= sec) { turnAroundTimes.Add(seconds); }
            for (var tramCounts = 20; tramCounts >= AT_LEAST_COUNT_TRAMS; tramCounts--) { tramNumbers.Add(tramCounts); }

            //check if debugger is attached to guarantee nice debugging
            if (Debugger.IsAttached) DEBUG = true;

            //The values for the tram counts we're testing

            var testValues = new List<(int a, int b, int c)>();

            foreach (var tramFrequency in testFrequencies)
                foreach (var tramCount in tramNumbers)
                    foreach (var turnAroundFreq in turnAroundTimes)
                        testValues.Add((tramFrequency, tramCount, turnAroundFreq));

            new Program().Run(testValues);
        }

        public IEnumerable<TramStatistics> Run(List<(int frequency, int tramCount, int turnAroundTime)> testValues)
        {
            //TODO: do stuff with this??
            var validationData = ValidationFileReader.ReadValidationFolder();
            //every item in validationData contains the information for a file.
            //1-st item for example has n items which correspond to the rows in the file
            //2-nd same etc for another file.. you can decide what to do with them but 
            //make sure u dont break anything
            var output = new ConcurrentBag<string>();

            var fileStatistics = new ConcurrentBag<TramStatistics>();

            var total = testValues.Count;
            var progress = 0.0;
            var header = "";

            Parallel.ForEach(testValues, new ParallelOptions()
            {
                MaxDegreeOfParallelism = DEBUG ? 1 : 8
            }, tuple =>
            {
                var tramFrequency = tuple.frequency;
                var tramCount = tuple.tramCount;
                var turnAroundTime = tuple.turnAroundTime;

                var debugFile = $"debug/{tramFrequency}_{tramCount}_{turnAroundTime}.txt";
                var statisticsFile = $"stat/STAT_{tramFrequency}_{tramCount}_{turnAroundTime}.txt";

                if (File.Exists(debugFile))
                {
                    File.Delete(debugFile);
                    File.Delete(statisticsFile);
                }

                //guarantee the debug files are deleted..
                Thread.Sleep(50);

                if (!Directory.Exists("stat")) Directory.CreateDirectory("stat");
                if (!Directory.Exists("debug")) Directory.CreateDirectory("debug");

                using (var file = File.OpenWrite(debugFile))
                using (var stat = File.OpenWrite(statisticsFile))
                {
                    using (var statisticsWriter = new StreamWriter(stat))
                    using (var streamWriter = new StreamWriter(file))
                    {
                        var debugStatisticsPerPerson = new List<string>();
                        var sm = new StateManager();

                        sm.DebugLine += (send, arg) =>
                        {
                            if (DEBUG)
                            {
                                if (!string.IsNullOrEmpty(arg.ToString().Trim()))
                                {
                                    streamWriter.WriteLine(arg.ToString());
                                }
                            }
                        };

                        var statistics = sm.Start(turnAroundTime, tramFrequency, tramCount, LatenessThreshold, DEBUG);

                        fileStatistics.Add(statistics);

                        if (DEBUG)
                        {
                            var terrain = sm.Track.Vertices;
                            foreach (var st in terrain)
                            {
                                var avg_waiting_time = (double)st.TotalWaitingTime / st.TotalPassengersServiced;
                                var toWrite = st.Name + "," + st.Id + "," + avg_waiting_time + "," + st.TotalPassengersServiced;
                                debugStatisticsPerPerson.Add(toWrite);
                            }
                        }

                        if (string.IsNullOrEmpty(header)) { header = statistics.GetHeader(); }

                        var data = $"{turnAroundTime};{tramFrequency};{tramCount};{statistics.ToString()}";
                        output.Add(data);

                        statisticsWriter.WriteLine("Station,id,avg_wait_time,total_passengers_serviced");
                        statisticsWriter.WriteLine(string.Join("\n", debugStatisticsPerPerson));

                        if (string.IsNullOrEmpty(header)) header = statistics.GetHeader();
                    }
                }

                // delete temp stuff if debug mode is off X_X
                if (!DEBUG)
                    if (File.Exists(debugFile))
                        File.Delete(debugFile);

                progress++;
                Console.WriteLine($"Progress : {Math.Round(progress / total * 100, 1)}%");
            });


            var final = new List<string>(output);

            final.Insert(0, string.Concat("q;freq;tramcnt;", header));
            File.WriteAllLines("output.csv", final);

            if (DEBUG) Console.WriteLine(final.Aggregate((a, b) => a + Environment.NewLine + b));

            return fileStatistics;
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
