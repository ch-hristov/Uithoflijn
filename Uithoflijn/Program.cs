﻿using System;
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
        public static bool DEBUG = false;
        public static bool RUN_VALIDATION_TESTING = true;
        public static bool VISUALIZE = false;

        private const int AT_LEAST_COUNT_TRAMS = 1;
        public const int AVERAGING_RUNS_PER_FILE = 5;
        public const int TOTAL_TRAMS_TESTED = 20;

        public int LatenessThreshold { get; }

        public Program(int latenessThreshold = 60) => LatenessThreshold = latenessThreshold;

        public static void Main(string[] args)
        {
            //The values of the frequencies we're testing, issue at least every 40 seconds(otherwise we issue waaaay too fast)
            var testFrequencies = new List<int>() { };
            var turnAroundTimes = new List<int>() { };
            var tramNumbers = new List<int>();

            int max_freq = (int)TimeSpan.FromMinutes(12).TotalSeconds;
            int min_freq = (int)TimeSpan.FromMinutes(3).TotalSeconds;
            int max_turn = (int)TimeSpan.FromMinutes(5).TotalSeconds;
            int min_turn = (int)TimeSpan.FromMinutes(2).TotalSeconds;

            const int test_sec = 30;

            for (var seconds = max_freq; seconds >= min_freq; seconds -= test_sec) { testFrequencies.Add(seconds); }
            for (var seconds = max_turn; seconds >= min_turn; seconds -= test_sec) { turnAroundTimes.Add(seconds); }
            for (var tramCounts = TOTAL_TRAMS_TESTED; tramCounts >= AT_LEAST_COUNT_TRAMS; tramCounts--) { tramNumbers.Add(tramCounts); }

            //check if debugger is attached to guarantee nice debugging 
            if (Debugger.IsAttached) DEBUG = true;

            //The values for the tram counts we're testing

            var testValues = new List<(int a, int b, int c)>();

            foreach (var tramFrequency in testFrequencies)
                foreach (var tramCount in tramNumbers)
                    foreach (var turnAroundFreq in turnAroundTimes)
                        testValues.Add((tramFrequency, tramCount, turnAroundFreq));

            if (RUN_VALIDATION_TESTING)
            {
                // just change bus => validation to produce for validation datasets if needed ^_^   
                var validationData = ValidationFileReader.ReadValidationFolder("bus");

                for (int i = 0; i < AVERAGING_RUNS_PER_FILE; i++)
                    foreach (var validationSet in validationData)
                        new Program().Run(validationSet.Item1, testValues, validationSet.Item2, i.ToString());
            }

        }

        /// <summary>
        /// Start a simulation with the provided file name
        /// </summary>
        /// <param name="testValues">data to test for in format( frequency, tram count, turnaround freq(q))</param>
        /// <param name="stationFrequencies">Predefined frequencies for the stations to use(can be used with the validation set)</param>
        /// <returns>Results of the analysis for each result item in <paramref name="testValues"/> </returns>
        public IEnumerable<TramStatistics> Run(
                                               string fileName,
                                               List<(int frequency, int tramCount, int turnAroundTime)> testValues,
                                               IEnumerable<InputRow> stationFrequencies = null,
                                               string runIdentifier = "")
        {
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
                var concurrentAccessStationsFreq = new ConcurrentBag<InputRow>(stationFrequencies.Select(x => x.Clone()));
                var tramFrequency = tuple.frequency;
                var tramCount = tuple.tramCount;
                var turnAroundTime = tuple.turnAroundTime;
                var realName = Path.GetFileNameWithoutExtension(fileName);

                var debugFile = $"debug/{realName}_{tramFrequency}_{tramCount}_{turnAroundTime}.txt";
                var statisticsFile = $"stat/STAT_{realName}_{tramFrequency}_{tramCount}_{turnAroundTime}.txt";
                var visualizeFile = $"vis/vis_{realName}_{tramFrequency}_{tramCount}_{turnAroundTime}.txt";

                if (File.Exists(debugFile))
                {
                    File.Delete(visualizeFile);
                    File.Delete(debugFile);
                    File.Delete(statisticsFile);
                }

                // guarantee the debug files are deleted..
                Thread.Sleep(50);

                if (!Directory.Exists("stat")) Directory.CreateDirectory("stat");
                if (!Directory.Exists("debug")) Directory.CreateDirectory("debug");
                if (!Directory.Exists("vis")) Directory.CreateDirectory("vis");

                using (var file = File.OpenWrite(debugFile))
                using (var stat = File.OpenWrite(statisticsFile))
                using (var visualize = File.OpenWrite(visualizeFile))
                {
                    using (var statisticsWriter = new StreamWriter(stat))
                    using (var streamWriter = new StreamWriter(file))
                    using (var visWriter = new StreamWriter(visualize))
                    {
                        var debugStatisticsPerPerson = new List<string>();
                        var sm = new StateManager();

                        sm.DebugLine += (send, arg) =>
                        {
                            if (DEBUG)
                                if (!string.IsNullOrEmpty(arg.ToString().Trim()))
                                    streamWriter.WriteLine(arg.ToString());
                        };

                        sm.WriteState += (send, arg) =>
                        {
                            if (DEBUG && VISUALIZE)
                                visWriter.WriteLine(arg);
                        };

                        var statistics = sm.Start(turnAroundTime, tramFrequency,
                                                  tramCount, LatenessThreshold,
                                                  DEBUG, concurrentAccessStationsFreq);

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

                        var data = $"{realName},{turnAroundTime},{tramFrequency},{tramCount},{statistics.ToString()}";
                        output.Add(data);

                        statisticsWriter.WriteLine("Station,id,avg_wait_time,total_passengers_serviced");
                        statisticsWriter.WriteLine(string.Join("\n", debugStatisticsPerPerson));

                        if (string.IsNullOrEmpty(header)) header = statistics.GetHeader();
                    }
                }

                // delete temp stuff if debug mode is off X_X
                if (!DEBUG)
                {
                    if (File.Exists(debugFile))
                        File.Delete(debugFile);
                }

                progress++;
                Console.WriteLine($"Progress : {Math.Round(progress / total * 100, 1)}%");
            });

            var final = new List<string>(output);
            final.Insert(0, string.Concat("file,q,freq,tramcnt,", header));

            if (!Directory.Exists("answers")) Directory.CreateDirectory("answers");
            var finalName = $"answers/output_{Path.GetFileNameWithoutExtension(fileName)}_{runIdentifier}.csv";

            File.WriteAllLines(finalName, final);
            if (DEBUG) Console.WriteLine(final.Aggregate((a, b) => a + Environment.NewLine + b));

            return fileStatistics;
        }
    }
}
