using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using MathNet.Numerics.Distributions;

namespace Uithoflijn
{
    public class ArrivalClass
    {
        /// <summary>
        /// From time
        /// </summary>
        public int From { get; set; }

        /// <summary>
        /// To time
        /// </summary>
        public int To { get; set; }

    }

    public class Station
    {
        public List<InputRow> GoingDistrubutions { get; set; }

        public List<InputRow> ComingDistrubutions { get; set; }

        public Station(int minSwitchDelay)
        {
            TramsQueue = new Queue<Tram>();
            ArrivalClasses = new Dictionary<ArrivalClass, int>();
            SwitchUsedLast = -1;
            _switchDelay = minSwitchDelay;
            GoingDistrubutions = new List<InputRow>();
            ComingDistrubutions = new List<InputRow>();
        }

        public Dictionary<ArrivalClass, int> ArrivalClasses { get; set; }

        public void SetTimetable(Timetable t)
        {
            if (Timetable != null || !IsTerminal) throw new Exception("Cant set timetable for this station");
            Timetable = t;
        }

        public int SwitchDelay(int time)
        {
            if (!IsTerminal) return 0;

            if (SwitchUsedLast == -1)
            {
                SwitchUsedLast = time;
                return 0;
            }
            else
            {
                if (time - SwitchUsedLast < 0) throw new Exception("Invalid state..something is really bad");

                if (time - SwitchUsedLast < _switchDelay)
                {
                    //wait for some time :/
                    return _switchDelay - (time - SwitchUsedLast);
                }
                else
                {
                    return 0;
                }
            }
        }

        public int SwitchUsedLast { get; set; }

        private int _switchDelay;

        public Timetable Timetable { get; private set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsTerminal { get; internal set; }

        public int LeftBehind { get; set; }

        public int TotalPassengersServiced { get; set; }

        public int TotalWaitingTime { get; set; }

        public IEnumerable<UEdge> OutEdges { get; internal set; }

        public IEnumerable<UEdge> InEdges { get; internal set; }

        public int? TimeOfLastTram { get; set; }

        public Queue<Tram> TramsQueue { get; private set; }

        public Tram CurrentTram { get; set; }

        private Dictionary<DateTime, int> IndexFinderDict = new Dictionary<DateTime, int>()
        {
            {new DateTime(01, 1, 1, 6, 0, 0), 0},
            {new DateTime(01, 1, 1, 6, 15, 0), 1},
            {new DateTime(01, 1, 1, 6, 30, 0), 2},
            {new DateTime(01, 1, 1, 6, 45, 0), 3},
            {new DateTime(01, 1, 1, 7, 0, 0), 4},
            {new DateTime(01, 1, 1, 7, 15, 0), 5},
            {new DateTime(01, 1, 1, 7, 30, 0), 6},
            {new DateTime(01, 1, 1, 7, 45, 0), 7},
            {new DateTime(01, 1, 1, 8, 0, 0), 8},
            {new DateTime(01, 1, 1, 8, 15, 0), 9},
            {new DateTime(01, 1, 1, 8, 30, 0), 10},
            {new DateTime(01, 1, 1, 8, 45, 0), 11},
            {new DateTime(01, 1, 1, 9, 0, 0), 12},
            {new DateTime(01, 1, 1, 9, 15, 0), 13},
            {new DateTime(01, 1, 1, 9, 30, 0), 14},
            {new DateTime(01, 1, 1, 9, 45, 0), 15},
            {new DateTime(01, 1, 1, 10, 0, 0), 16},
            {new DateTime(01, 1, 1, 10, 15, 0), 17},
            {new DateTime(01, 1, 1, 10, 30, 0), 18},
            {new DateTime(01, 1, 1, 10, 45, 0), 19},
            {new DateTime(01, 1, 1, 11, 0, 0), 20},
            {new DateTime(01, 1, 1, 11, 15, 0), 21},
            {new DateTime(01, 1, 1, 11, 30, 0), 22},
            {new DateTime(01, 1, 1, 11, 45, 0), 23},
            {new DateTime(01, 1, 1, 12, 0, 0), 24},
            {new DateTime(01, 1, 1, 12, 15, 0), 25},
            {new DateTime(01, 1, 1, 12, 30, 0), 26},
            {new DateTime(01, 1, 1, 12, 45, 0), 27},
            {new DateTime(01, 1, 1, 13, 0, 0), 28},
            {new DateTime(01, 1, 1, 13, 15, 0), 29},
            {new DateTime(01, 1, 1, 13, 30, 0), 30},
            {new DateTime(01, 1, 1, 13, 45, 0), 31},
            {new DateTime(01, 1, 1, 14, 0, 0), 32},
            {new DateTime(01, 1, 1, 14, 15, 0), 33},
            {new DateTime(01, 1, 1, 14, 30, 0), 34},
            {new DateTime(01, 1, 1, 14, 45, 0), 35},
            {new DateTime(01, 1, 1, 15, 0, 0), 36},
            {new DateTime(01, 1, 1, 15, 15, 0), 37},
            {new DateTime(01, 1, 1, 15, 30, 0), 38},
            {new DateTime(01, 1, 1, 15, 45, 0), 39},
            {new DateTime(01, 1, 1, 16, 0, 0), 40},
            {new DateTime(01, 1, 1, 16, 15, 0), 41},
            {new DateTime(01, 1, 1, 16, 30, 0), 42},
            {new DateTime(01, 1, 1, 16, 45, 0), 43},
            {new DateTime(01, 1, 1, 17, 0, 0), 44},
            {new DateTime(01, 1, 1, 17, 15, 0), 45},
            {new DateTime(01, 1, 1, 17, 30, 0), 46},
            {new DateTime(01, 1, 1, 17, 45, 0), 47},
            {new DateTime(01, 1, 1, 18, 0, 0), 48},
            {new DateTime(01, 1, 1, 18, 15, 0), 49},
            {new DateTime(01, 1, 1, 18, 30, 0), 50},
            {new DateTime(01, 1, 1, 18, 45, 0), 51},
            {new DateTime(01, 1, 1, 19, 0, 0), 52},
            {new DateTime(01, 1, 1, 19, 15, 0), 53},
            {new DateTime(01, 1, 1, 19, 30, 0), 54},
            {new DateTime(01, 1, 1, 19, 45, 0), 55},
            {new DateTime(01, 1, 1, 20, 0, 0), 56},
            {new DateTime(01, 1, 1, 20, 15, 0), 57},
            {new DateTime(01, 1, 1, 20, 30, 0), 58},
            {new DateTime(01, 1, 1, 20, 45, 0), 59},
            {new DateTime(01, 1, 1, 21, 0, 0), 60},
            {new DateTime(01, 1, 1, 21, 15, 0), 61},
            {new DateTime(01, 1, 1, 21, 30, 0), 62}
        };

        private double[,] RouteCStoPR = new double[,] {
            {147.4,2.33,0,0.286,0,0,14.048,3.143,0},
            {99.8,4.465,0.75,0.468,0.025,0,21.024,4.8465,0},
            {68.2,6.6,1.5,0.65,0.05,0,28,6.55,0},
            {47.7,1.14,1.57,0.524,0,0,13.238,3.048,0},
            {75,5.28,2.15,0.295,0.1,0,23.35,5.54,0},
            {85.7,11.1,2.48,0.143,0.143,0.048,29,6.809,0},
            {240.8,30.71,4.33,0.336,0.193,0.048,66.714,15.386,0},
            {213.7,31.75,3.59,1.283,0.1,0.05,48.05,11.067,0},
            {630.8,24.62,8.95,2.19,0.239,0.238,138.287,29.857,0},
            {720.6,13.95,8.24,1.001,0.381,0.428,171.19,36.143,0},
            {1010.5,23.53,10.37,2.289,0.949,0.671,244.817,51.886,0},
            {376.2,9.06,5.34,1.226,0.543,0.19,61.288,13.812,0},
            {399.8,7.67,5.14,0.595,0.254,0.301,65.286,14.683,0},
            {304.3,5.55,3.75,0.6,0.2,0.05,54.45,12.3,0},
            {266.3,3.94,4.36,0.825,0.05,0.15,35,8.588,0},
            {270.1,5.76,3.48,0.97,0.542,0.109,37.161,8.938,0},
            {332.1,5.2,6.62,1.344,0.526,0.059,39.865,9.432,0},
            {404.8,8.77,6.04,1.341,0.58,0.39,52.16,11.902,0},
            {371.4,8.52,8.69,1.262,0.738,0.159,43.154,9.999,0},
            {241.1,7.13,6.55,4.518,0.4,0.5,20.335,5.789,0},
            {133.3,5.71,3.85,1.142,0.523,0.191,11.381,3.524,0},
            {139.4,3.37,3.42,0.488,0.148,0.145,8.057,2.847,0},
            {138.2,3.1,4.09,1.095,0.333,0,9.857,3.047,0},
            {118.2,2.77,3.03,0.864,0.143,0.048,8.631,2.91,0},
            {166.3,3.32,3.9,1.064,0.578,0.195,9.622,3.238,0},
            {202.5,4.14,4.23,1.239,0.428,0.238,20.858,5.428,0},
            {379.4,7.43,8.04,3.477,1.524,0.239,31.476,8.286,0},
            {231.4,5.24,6.05,5.953,0.905,0.096,19.095,5.143,0},
            {154.4,3.93,5.86,2.264,0.393,0.048,12.759,3.81,0},
            {116.4,2.67,3,0.714,0.523,0.334,13.096,4.001,0},
            {95.4,2.71,3.04,0.381,0.286,0.095,11.286,3.571,0},
            {75.2,1.62,2.54,0.629,0.777,0.101,10.13,3.238,0},
            {76.5,2.17,3.62,0.581,0.383,0.098,10.397,3.146,0},
            {138.8,3.11,3.77,0.801,0.333,0.143,13.985,4.192,0},
            {142.4,3.86,5.39,1.713,0.619,0.048,21.191,5.619,0},
            {95.5,4.21,4.18,1.377,0.806,0.206,13.744,4.183,0},
            {69.9,3.72,3.23,4.529,0.917,0.191,12.136,3.907,0},
            {55.6,2.07,2.03,1.314,0.334,0.098,6.746,2.365,0},
            {44.4,2.62,2.09,1.143,0.285,0,6.524,2.571,0},
            {70.9,2.93,3.13,1.655,0.636,0.1,13.673,4.38,0},
            {38.1,2.41,2.21,1.15,0.492,0.053,10.614,3.253,0},
            {72.5,2.68,3.69,1.708,0.441,0.095,14.025,4.392,0},
            {67,3.57,4.21,1.84,0.576,0.146,9.153,3.005,0},
            {65.4,3.37,3.65,7.91,2.1,0.241,9.865,3.551,0},
            {38.2,1.99,2.63,4.756,0.911,0,6.333,2.567,0},
            {87,3,4.14,1.274,0.552,0.096,15.248,4.543,0},
            {36.5,2.11,1.67,0.376,0.15,0,3.645,1.574,0},
            {50.4,1.85,2.34,0.506,0.048,0.053,5.903,2.163,0},
            {39.1,1.32,3.25,0.984,0.056,0.048,2.96,1.429,0},
            {23.4,0.9,1,0.095,0,0.048,1.048,0.619,0},
            {29,1.14,1.33,0.143,0,0,2.667,0.952,0},
            {22.4,0.86,0.52,0,0.048,0,0.714,0.476,0},
            {32,0.76,0.86,0.143,0,0,1.19,0.571,0},
            {20.5,0.57,0.38,0.048,0,0.095,0.714,0.381,0},
            {16.1,0.38,0.57,0,0,0,0.857,0.429,0},
            {13.1,0.4,0.5,0.05,0,0,0.25,0.15,0},
            {14.9,0.57,0.76,0.048,0,0,0.286,0.19,0},
            {9,0.38,0.48,0,0,0,0.143,0.095,0},
            {10.9,0.67,0.71,0,0,0,0.238,0.238,0},
            {9,0.38,0.43,0.095,0,0,0.048,0.048,0},
            {9.5,0.38,0.33,0.048,0.048,0,0.095,0.095,0},
            {0.48,4,5.19,0.19,0.86,1.1,0.667,0.38,0}
        };

        private double[,] RoutePRtoCS = new double[,] {
            {0,0,0,0.143,0,0,0.024,2.26,73.7},
            {0,0,0.025,0.325,0.575,0.55,1.225,4.775,34.1},
            {0,0,0,0.524,1,1.905,0.619,1.38,47.7},
            {0,0,0.05,0.1475,0.515,1.093,0.2415,4.53,37.5},
            {0,0.024,0.0715,0.0715,1.5,0.643,0.4045,10.21,42.85},
            {0,0.012,0.04825,0.084,0.8625,0.938,0.50475,14.31,60.2},
            {0,0.016666667,0.033333333,0.427666667,0.996666667,0.97,0.459666667,20.27666667,71.23333333},
            {0,0.0595,0.05975,0.5475,1.1075,2.9405,1.15475,10.6425,157.7},
            {0,0.142666667,0.127,0.333666667,3.016666667,2.063333333,1.492333333,7.35,240.2},
            {0,0.16775,0.23725,0.57225,2.37,2.3635,2.13325,9.23,252.625},
            {0,0.063333333,0.181,0.408666667,1.453333333,1.667333333,1.323666667,4.386666667,125.4},
            {0,0.07525,0.0635,0.14875,1.1825,0.94325,1.091,2.28,99.95},
            {0,0.016666667,0.066666667,0.2,0.9,1.133333333,0.616666667,2.766666667,101.4333333},
            {0,0.05,0.016666667,0.275,0.93,1.491666667,0.904,1.46,88.76666667},
            {0,0.036333333,0.180666667,0.323333333,0.87,1.077,0.612,2.743333333,90.03333333},
            {0,0.019666667,0.175333333,0.448,0.83,3.194,0.969666667,1.953333333,110.7},
            {0,0.13,0.193333333,0.447,2.246666667,1.246333333,1.081,4.24,134.9333333},
            {0,0.053,0.246,0.420666667,3.15,2.120333333,1.655333333,3.536666667,123.8},
            {0,0.125,0.1,1.1295,0.9275,1.98575,0.80575,2.37,60.275},
            {0,0.063666667,0.174333333,0.380666667,1.366666667,0.761666667,0.778,2.7,44.43333333},
            {0,0.048333333,0.049333333,0.162666667,1.19,0.707333333,0.4,1.543333333,46.46666667},
            {0,0,0.111,0.365,1.333333333,0.968333333,0.333333333,1.253333333,46.06666667},
            {0,0.016,0.047666667,0.288,1.13,0.534,0.338666667,1.196666667,39.4},
            {0,0.065,0.192666667,0.354666667,1.2,1.046666667,0.498666667,1.286666667,55.43333333},
            {0,0.079333333,0.142666667,0.413,1.556666667,0.984,0.381,1.97,67.5},
            {0,0.05975,0.381,0.86925,2.1075,1.488,1.1785,2,94.85},
            {0,0.032,0.301666667,1.984333333,2.016666667,1.587333333,1.365333333,1.65,77.13333333},
            {0,0.016,0.131,0.754666667,2.29,1.149,1.085,1.176666667,51.46666667},
            {0,0.111333333,0.174333333,0.238,0.983333333,0.634666667,0.587333333,0.73,38.8},
            {0,0.031666667,0.095333333,0.127,1.046666667,0.619,0.444666667,0.873333333,31.8},
            {0,0.033666667,0.259,0.209666667,0.906666667,0.429333333,0.259,0.556666667,25.06666667},
            {0,0.032666667,0.127666667,0.193666667,0.876666667,1.231666667,0.324,0.816666667,25.5},
            {0,0.03575,0.08325,0.20025,0.8375,0.64275,0.3135,0.825,34.7},
            {0,0.016,0.206333333,0.571,1.3,1.857,0.650666667,1.553333333,47.46666667},
            {0,0.068666667,0.268666667,0.459,1.166666667,1.262666667,0.650666667,1.653333333,31.83333333},
            {0,0.04775,0.22925,1.13225,0.5775,0.67625,0.472,0.955,17.475},
            {0,0.032666667,0.111333333,0.438,0.483333333,0.576333333,0.257,0.833333333,18.53333333},
            {0,0,0.095,0.381,0.363333333,0.651,0.285666667,1.046666667,14.8},
            {0,0.025,0.159,0.41375,0.685,0.61425,0.17025,0.955,17.725},
            {0,0.017666667,0.164,0.383333333,0.67,0.489666667,0.322666667,0.873333333,12.7},
            {0,0.02375,0.11025,0.427,0.715,0.7725,0.26875,0.775,18.125},
            {0,0.048666667,0.192,0.613333333,1.326666667,1.016666667,0.45,1.463333333,22.33333333},
            {0,0.06025,0.525,1.9775,0.875,0.622,0.31425,1.01,16.35},
            {0,0,0.303666667,1.585333333,0.83,0.646333333,0.329666667,0.636666667,12.73333333},
            {0,0.024,0.138,0.3185,0.8975,0.78225,0.44825,0.7225,21.75},
            {0,0,0.05,0.125333333,0.43,0.276,0.446,0.703333333,12.16666667},
            {0,0.017666667,0.016,0.168666667,0.716666667,0.369333333,0.473,0.423333333,16.8},
            {0,0.024,0.028,0.492,0.81,1.8015,0.524,0.58,19.55},
            {0,0.048,0,0.095,0.52,1.095,0.619,0.86,23.4},
            {0,0,0,0.143,0.57,1.571,0.905,0.9,29},
            {0,0,0.048,0,0.19,0.476,0.238,1.05,22.4},
            {0,0,0,0.143,0.24,1.095,0.524,0.57,32},
            {0,0.095,0,0.048,0.29,0.333,0.238,0.57,20.5},
            {0,0,0,0,0.33,0.619,0.19,0.29,16.1},
            {0,0,0,0.05,0.5,0.3,0.2,0.35,13.1},
            {0,0,0,0.048,0.76,0.381,0.238,0.57,14.9},
            {0,0,0,0,0.24,0.476,0.19,0.33,9},
            {0,0,0,0,0.52,0.571,0.524,0.38,10.9},
            {0,0,0,0.095,0.38,0.286,0.19,0.29,9},
            {0,0,0.048,0.048,0.29,0.19,0.095,0.48,9.5}
        };

        public override string ToString()
        {
            return $"[{Id}]{Name}";
        }

        public override bool Equals(object obj)
        {
            var c = obj as Station;
            return Id == c.Id;
        }

        // There was an alert about this on my computer, so I added it. P
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }

        /**
         * Find the 15'min interval we have to be in given the current time.
         * 
         * Returns the index for the row to be used with the 2D array.
         */
        public int FindAppropriateInterval(DateTime dt)
        {
            // First, round to the closest 15min interval. 
            // if time is 9:13, it will be rounded to 9:15, if it's 9:04 to 9:00, etc...
            // When we have a better input analysis, this will code will be reduced. 
            DateTime nearest = Utils.RoundToNearest(dt, TimeSpan.FromMinutes(15));

            if (IndexFinderDict.ContainsKey(nearest))
                return IndexFinderDict[nearest];
            else
            {
                DateTime rd = Utils.RoundDown(dt, TimeSpan.FromMinutes(15));
                DateTime ru = Utils.RoundUp(dt, TimeSpan.FromMinutes(15));
                if (IndexFinderDict.ContainsKey(rd))
                {
                    return IndexFinderDict[rd];
                }
                else if (IndexFinderDict.ContainsKey(ru))
                {
                    return IndexFinderDict[ru];
                }
                else
                {
                    return 60;
                }
            }

        }

        //<summary> Returns the index in the table for the coming passengers
        // it will be a number between 0 and 4.
        // For this to work. We assume that the ComingDistrubutions array is sorted as follows:
        // [6-7], [7-9], [9-16], [16-18], [18,21.5] Where, [] are the array positions and inside the time window
        public int GetIndex(int time)
        {
            if (time <= 3600) return 0;
            else if (time > 3600 && time <= 10800) return 1;
            else if (time > 10800 && time <= 36000) return 2;
            else if (time > 36000 && time <= 43200) return 3;
            else return 4;
        }

        public int GetEmbarkingPassengers(Tram tram, int time)
        {
            // Find the index in the array of lambdas for the poisson process.
            var currDT = Utils.SecondsToDateTime(time);
            int index = FindAppropriateInterval(currDT);

            var lambda = 0.0;
            var direction = 1;

            // CS -> P+R Direction 1
            if (Id == 0 && tram.PreviousTerminal.Id == 8)
            {
                // If for any reason we are delayed, we have to go back to the data that we know.
                if (index > RouteCStoPR.GetLength(0) - 1)
                    index = RouteCStoPR.GetLength(0) - 1;

                lambda = RouteCStoPR[index, Id];
                // Special case we are Centraal (id = 0) but we came from P+R
            }
            else if (Id < 8) 
            {
                direction = 1;

                // If for any reason we are delayed, we have to go back to the data that we know.
                if (index > RouteCStoPR.GetLength(0) - 1)
                    index = RouteCStoPR.GetLength(0) - 1;

                lambda = RouteCStoPR[index, Id];
            }
            else if (Id >= 8)
            {
                direction = 0;

                // If for any reason we are delayed, we have to go back to the data that we know.
                if (index > RoutePRtoCS.GetLength(0) - 1)
                    index = RoutePRtoCS.GetLength(0) - 1;

                lambda = RoutePRtoCS[index, Id - 8];
            }
            else // Depot
            {
                return 0;
            }

            if (lambda == 0.0)
                lambda = 0.01;

            // Generate the number of new arrival events *n* that will occur.
            var pd = new Poisson(lambda);
            var n = pd.Sample();
            var my_index = GetIndex(time);

            // Deduct from the expected people the n we computed.
            var shouldEnter = 0;

            if (direction == 0)
            {
                shouldEnter = (int)ComingDistrubutions[my_index].PassIn;

                if (shouldEnter - n >= 0)
                {
                    ComingDistrubutions[my_index].PassIn -= n;
                }
                else
                {
                    n = (int)ComingDistrubutions[my_index].PassIn;
                    ComingDistrubutions[my_index].PassIn = 0;
                }
            }
            else
            {
                shouldEnter = (int)GoingDistrubutions[my_index].PassIn;

                if (shouldEnter - n >= 0)
                {
                    GoingDistrubutions[my_index].PassIn -= n;
                }
                else
                {
                    n = (int)GoingDistrubutions[my_index].PassIn;
                    GoingDistrubutions[my_index].PassIn = 0;
                }
            }

            // Initialize an array to store the arrival times t_i, i = {1,2,..., n}.
            var arrivals = new int[n];

            // We will get the arrival times from the uniform distribution. U[0,T]
            var last = 0;
            if (TimeOfLastTram.HasValue)
            {
                last = TimeOfLastTram.Value;
            }
            var ud = new DiscreteUniform(last, time);

            for (int i = 0; i < n; ++i)
            {
                arrivals[i] = ud.Sample();
            }

            // Compute the total waiting time for the new passengers given the array arrivals. we also need to keep track of the people already waiting.
            var total_waiting_time = 0;
            for (int i = 0; i < n; ++i)
            {
                total_waiting_time += time - arrivals[i];
            }

            TotalWaitingTime += total_waiting_time;

            return n;
        }

        public void IncrementLeftBehindAverageWaiting(int time)
        {
            TotalWaitingTime += LeftBehind * time;
        }

    }
}