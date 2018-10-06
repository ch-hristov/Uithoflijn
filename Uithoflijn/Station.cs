using System;
using System.Collections.Generic;
using System.Text;

namespace Uithoflijn
{
    public class Station
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsTerminal { get; internal set; }

        public double WaitingPeople { get; internal set; }

        public IEnumerable<UEdge> OutEdges { get; internal set; }

        public IEnumerable<UEdge> InEdges { get; internal set; }
        public Tram CurrentTram { get; internal set; }

        Dictionary<DateTime, int> IndexFinderDict = new Dictionary<DateTime, int>()
        {
            {new DateTime(2000, 1, 1, 6, 0, 0), 0},
            {new DateTime(2000, 1, 1, 6, 15, 0), 1},
            {new DateTime(2000, 1, 1, 6, 30, 0), 2},
            {new DateTime(2000, 1, 1, 6, 45, 0), 3},
            {new DateTime(2000, 1, 1, 7, 0, 0), 4},
            {new DateTime(2000, 1, 1, 7, 15, 0), 5},
            {new DateTime(2000, 1, 1, 7, 30, 0), 6},
            {new DateTime(2000, 1, 1, 7, 45, 0), 7},
            {new DateTime(2000, 1, 1, 8, 0, 0), 8},
            {new DateTime(2000, 1, 1, 8, 15, 0), 9},
            {new DateTime(2000, 1, 1, 8, 30, 0), 10},
            {new DateTime(2000, 1, 1, 8, 45, 0), 11},
            {new DateTime(2000, 1, 1, 9, 0, 0), 12},
            {new DateTime(2000, 1, 1, 9, 15, 0), 13},
            {new DateTime(2000, 1, 1, 9, 30, 0), 14},
            {new DateTime(2000, 1, 1, 9, 45, 0), 15},
            {new DateTime(2000, 1, 1, 10, 0, 0), 16},
            {new DateTime(2000, 1, 1, 10, 15, 0), 17},
            {new DateTime(2000, 1, 1, 10, 30, 0), 18},
            {new DateTime(2000, 1, 1, 10, 45, 0), 19},
            {new DateTime(2000, 1, 1, 11, 0, 0), 20},
            {new DateTime(2000, 1, 1, 11, 15, 0), 21},
            {new DateTime(2000, 1, 1, 11, 30, 0), 22},
            {new DateTime(2000, 1, 1, 11, 45, 0), 23},
            {new DateTime(2000, 1, 1, 12, 0, 0), 24},
            {new DateTime(2000, 1, 1, 12, 15, 0), 25},
            {new DateTime(2000, 1, 1, 12, 30, 0), 26},
            {new DateTime(2000, 1, 1, 12, 45, 0), 27},
            {new DateTime(2000, 1, 1, 13, 0, 0), 28},
            {new DateTime(2000, 1, 1, 13, 15, 0), 29},
            {new DateTime(2000, 1, 1, 13, 30, 0), 30},
            {new DateTime(2000, 1, 1, 13, 45, 0), 31},
            {new DateTime(2000, 1, 1, 14, 0, 0), 32},
            {new DateTime(2000, 1, 1, 14, 15, 0), 33},
            {new DateTime(2000, 1, 1, 14, 30, 0), 34},
            {new DateTime(2000, 1, 1, 14, 45, 0), 35},
            {new DateTime(2000, 1, 1, 15, 0, 0), 36},
            {new DateTime(2000, 1, 1, 15, 15, 0), 37},
            {new DateTime(2000, 1, 1, 15, 30, 0), 38},
            {new DateTime(2000, 1, 1, 15, 45, 0), 39},
            {new DateTime(2000, 1, 1, 16, 0, 0), 40},
            {new DateTime(2000, 1, 1, 16, 15, 0), 41},
            {new DateTime(2000, 1, 1, 16, 30, 0), 42},
            {new DateTime(2000, 1, 1, 16, 45, 0), 43},
            {new DateTime(2000, 1, 1, 17, 0, 0), 44},
            {new DateTime(2000, 1, 1, 17, 15, 0), 45},
            {new DateTime(2000, 1, 1, 17, 30, 0), 46},
            {new DateTime(2000, 1, 1, 17, 45, 0), 47},
            {new DateTime(2000, 1, 1, 18, 0, 0), 48},
            {new DateTime(2000, 1, 1, 18, 15, 0), 49},
            {new DateTime(2000, 1, 1, 18, 30, 0), 50},
            {new DateTime(2000, 1, 1, 18, 45, 0), 51},
            {new DateTime(2000, 1, 1, 19, 0, 0), 52},
            {new DateTime(2000, 1, 1, 19, 15, 0), 53},
            {new DateTime(2000, 1, 1, 19, 30, 0), 54},
            {new DateTime(2000, 1, 1, 19, 45, 0), 55},
            {new DateTime(2000, 1, 1, 20, 0, 0), 56},
            {new DateTime(2000, 1, 1, 20, 15, 0), 57},
            {new DateTime(2000, 1, 1, 20, 30, 0), 58},
            {new DateTime(2000, 1, 1, 20, 45, 0), 59}
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
            DateTime nearest = Utils.RoundToNearest(dt, TimeSpan.FromMinutes(15));

            return IndexFinderDict[nearest];
        }

        public int GetEmbarkingPassengers(Tram tram, int time)
        {
            //TODO:
            return (int)(WaitingPeople * 0.1);
        }


    }
}
