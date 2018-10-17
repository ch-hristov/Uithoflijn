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
            Trams = new Queue<Tram>();
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

        public Queue<Tram> Trams { get; private set; }

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
            {0.05,0.75,0.3,0.1,0.15,0.475,0.05,1.45,0},
            {0.19,3.5,0,0.095,1.14,2.95,0.952,1.19,0},
            {0.55,3.4,0,1,1.95,3.7,1.35,0.85,0},
            {1.67,3.5,0.095,1.286,0.62,2,0.476,2.24,0},
            {4.65,8.1,1.7,1.15,0.9,5.5,1.5,2.65,0},
            {5.355,6.1,0.72,1.043,1.655,5.75,1.5845,1.89,0},
            {5.89,3.7,0.587333333,0.746,0.966666667,4.253333333,1.412333333,2.396666667,0},
            {3.996666667,3.066666667,0.645333333,0.370666667,1.193333333,2.666666667,0.997,2.336666667,0},
            {6.7875,3.3,0.7595,0.366,1.075,2.265,0.83275,1.895,0},
            {4.605,3.1,0.71425,0.512,1.035,2.0725,0.8095,1.475,0},
            {8.333333333,3.3,0.915,0.340333333,0.58,1.936666667,0.670333333,0.656666667,0},
            {4.2,2.5,0.433333333,0.45,0.533333333,1.916666667,0.616666667,1.4,0},
            {3.583333333,2.866666667,0.655,0.414,0.52,1.593333333,0.501,0.77,0},
            {4.426666667,3.2,0.916666667,0.370666667,0.433333333,1.57,0.4,1.016666667,0},
            {4.483333333,5.633333333,2.105666667,0.864666667,0.786666667,1.893333333,0.474,1.446666667,0},
            {2.176666667,5.033333333,2.680333333,0.653,1.033333333,2.646666667,0.455333333,1.48,0},
            {4.346666667,5.3,6.022,0.603666667,0.866666667,2.373333333,0.551666667,0.71,0},
            {8.235,12.8,10.1975,1.438,1.1675,2.485,0.51975,2.0275,0},
            {6.016666667,35.53333333,9.302,7.426666667,1.233333333,1.973333333,0.486,1.736666667,0},
            {4.77,18.46666667,7.901,4.697333333,1.606666667,2.583333333,0.47,2.03,0},
            {2.813333333,11.7,9.580333333,3.034333333,1.303333333,3.49,0.443666667,2.93,0},
            {5.286666667,10.56666667,7.733333333,1.254,1.416666667,2.47,0.709333333,6.87,0},
            {8.113333333,23.2,10.667,1.508,2.206666667,4.396666667,0.476333333,3.7,0},
            {6.97,21.46666667,13.58766667,1.412666667,2.476666667,4.573333333,0.444333333,5.08,0},
            {4.595,18.2,18.59525,1.54775,2.17,4.94,0.595,5.2025,0},
            {8.666666667,33.36666667,21.079,3.016,1.713333333,4.333333333,0.587666667,5.16,0},
            {10.49333333,71.66666667,25.07933333,8.222,2.096666667,3.49,0.571666667,4.586666667,0},
            {7.79,33.06666667,22.19033333,6.857333333,2.696666667,4.126666667,0.714333333,6.143333333,0},
            {5.75,20.1,19.15866667,2.444666667,2.046666667,4.776666667,0.635,4.523333333,0},
            {6.333333333,16.96666667,18.10633333,1.344333333,1.8,4.503333333,0.421333333,6.083333333,0},
            {5.16,25.2,15.90466667,1.492,2.41,4.033333333,0.603333333,8.126666667,0},
            {7.23,27.85,21.535,1.58825,2.2725,5.2375,0.41125,7.94,0},
            {8.78,20.16666667,24.364,1.556333333,1.826666667,5.506666667,0.551333333,5.833333333,0},
            {10.0925,35.95,24.884,1.76675,2.565,3.6175,0.46775,8.8925,0},
            {13.46666667,52.03333333,19.98333333,3.433333333,4.15,5.05,0.5,6.483333333,0},
            {19.585,55.425,27.159,7.09575,4.1425,4.1825,0.31975,3.145,0},
            {18.40333333,32.46666667,33.39833333,6.169666667,5.956666667,5.316666667,0.304,7.013333333,0},
            {14.3175,30.625,26.717,2.66775,6.2975,5.2575,0.3265,7.2075,0},
            {15.09,42,25.77,3.169,6.053333333,4.923333333,0.212666667,4.436666667,0},
            {21.38333333,35.33333333,26.23966667,4.632666667,6.783333333,3.41,0.319666667,5.883333333,0},
            {24.63,31.575,26.4645,3.131,6.4175,5.6325,0.393,5.69,0},
            {30.08666667,40.46666667,29.73666667,3.352333333,6.46,5.89,0.454,5.32,0},
            {33.1675,57.825,29.23175,3.4385,6,3.7175,0.6515,4.2725,0},
            {35.08666667,69.5,30.826,5.635666667,5.646666667,3.243333333,0.584,2.873333333,0},
            {27.99,33.95,29.831,4.294,7.27,4.0725,0.46025,2.5775,0},
            {15.01,31.16666667,19.11933333,1.501333333,5.053333333,4.086666667,0.388333333,1.753333333,0},
            {13.75333333,24.93333333,16.39933333,1.383333333,4.07,3.41,0.499333333,1.516666667,0},
            {8.356666667,19.5,11.81866667,1.381,4.266666667,3.74,0.454666667,1.03,0},
            {10.045,34,19.5235,1.976,5.02,6.525,0.7385,1.405,0},
            {6.75,27.9,19.6,1.7,2.6,4,0.7,1.75,0},
            {6.52,29,18.9525,2.6905,3.64,5.235,0.5,0.785,0},
            {4.86,51.9,14.857,2.048,2.95,4.48,0.524,1,0},
            {3.1,25.8,14.143,3.19,3.52,3.95,0.619,0.67,0},
            {4.4,17.2,18.9,1.3,2.15,4.15,0.5,0.65,0},
            {4.05,16.7,13.048,1.19,5.05,8.14,1.19,0.57,0},
            {4.14,13.8,18.714,1.619,2.14,3.33,0.857,0.48,0},
            {4.52,12.5,12.81,1.429,3.29,7.33,0.762,0.33,0},
            {2.67,9.4,10.81,0.81,1.67,2.29,0.286,0.62,0},
            {3.43,9.4,4.571,0.81,1.95,6.14,1.238,0.9,0},
            {2.9,9.5,10.667,0.429,1.52,3.52,0.143,0.38,0},
            {2,5.9,8.667,1.238,2.52,6,1,0.9,0},
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
        public int GetIndex(int time) {
            if (time <= 3600) return 0;
            else if (time > 3600 && time <= 10800) return 1;
            else if (time > 10800 && time <= 36000) return 2;
            else if (time > 36000 && time <= 43200) return 3;
            else return 4;
        }

        public int GetEmbarkingPassengers(Tram tram, int time)
        {
            // Find the index in the array of lambdas for the poisson process.
            DateTime currDT = Utils.SecondsToDateTime(time);
            int index = FindAppropriateInterval(currDT);

            double lambda = 0;
            // CS -> P+R
            if (Id >= 0 && Id <= 8)
            {
                // If for any reason we are delayed, we have to go back to the data that we know.
                if (index > RouteCStoPR.GetLength(0) - 1)
                    index = RouteCStoPR.GetLength(0) - 1;
                lambda = RouteCStoPR[index, Id];
            }
            else
            {
                if (Id == -1)
                {
                    return 0;
                }
                else
                {
                    // If for any reason we are delayed, we have to go back to the data that we know.
                    if (index > RoutePRtoCS.GetLength(0) - 1)
                        index = RoutePRtoCS.GetLength(0) - 1;

                    lambda = RoutePRtoCS[index, Id - 8];
                }
            }

            if (lambda == 0.0)
            {
                lambda = 0.01;
            }

            // Generate the number of new arrival events *n* that will occur.
            Poisson pd = new Poisson(lambda);
            int n = pd.Sample();


            int my_index = GetIndex(time);

            // Deduct from the expected people the n we computed.
            var shouldEnter = (int)this.ComingDistrubutions[my_index].PassIn;

            if (shouldEnter - n >= 0) 
            {
                this.ComingDistrubutions[my_index].PassIn -= n;
            }
            else 
            {
                n = (int)this.ComingDistrubutions[my_index].PassIn;
                this.ComingDistrubutions[my_index].PassIn = 0;
            }

            // Initialize an array to store the arrival times t_i, i = {1,2,..., n}.
            int[] arrivals = new int[n];
            //Console.WriteLine($"time: {time}; lambda: {lambda}");

            // We will get the arrival times from the uniform distribution. U[0,T]
            var last = 0;
            if (TimeOfLastTram.HasValue) last = TimeOfLastTram.Value;
            DiscreteUniform ud = new DiscreteUniform(last, time);

            for (int i = 0; i < n; ++i)
            {
                arrivals[i] = ud.Sample();
            }

            // Compute the total waiting time for the new passengers given the array arrivals. we also need to keep track of the people already waiting.
            int total_waiting_time = 0;
            for (int i = 0; i < n; ++i)
            {
                total_waiting_time += time - arrivals[i];
            }

            this.TotalWaitingTime += total_waiting_time;

            return n;
        }

        public void IncrementLeftBehindAverageWaiting(int time)
        {
            this.TotalWaitingTime += this.LeftBehind * time;
        }

    }
}
