using C5;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Uithoflijn
{
    public class StateManager
    {

        public List<TransportArgs> DebugLog = new List<TransportArgs>();

        private const int TotalTime = 54000;
        private const int MinimumTimeBetweenTrains = 40;
        private const int MAX_PASSENGERS_IN_TRAIN = 420;

        public int InitialTrams { get; set; }

        public event EventHandler<TransportArgs> TramArrived = delegate { };
        public event EventHandler<TransportArgs> TramDeparture = delegate { };

        public event EventHandler<TransportArgs> TramExpectedDeparture = delegate { };
        public event EventHandler<TransportArgs> TramExpectedArrival = delegate { };

        public event EventHandler<TransportArgs> DebugLine = delegate { };

        private IntervalHeap<TransportArgs> EventQueue = new IntervalHeap<TransportArgs>();
        private System.Collections.Generic.ICollection<Tram> Trams = new List<Tram>();

        public Terrain Track { get; set; }
        public int TotalDelay { get; set; }
        public int TotalPunctuality { get; set; }
        public int TotalHighLatenessTrams { get; set; }

        public int TurnaroundTime { get; private set; }
        public int CurrentFrequency { get; private set; }
        public bool DEBUG { get; private set; }

        public TramStatistics Start(int turnAroundTime, int peakFrequency, int numberOfTrams, bool debug = false)
        {
            //Issue lots of trams(iliketrains)
            Track = new Terrain(peakFrequency, turnAroundTime);
            DEBUG = debug;

            TurnaroundTime = turnAroundTime;
            InitialTrams = numberOfTrams;
            CurrentFrequency = numberOfTrams;

            TramArrived += HandleArrival;
            TramDeparture += HandleDeparture;

            TramExpectedArrival += HandleExpectedArrival;
            TramExpectedDeparture += HandleExpectedDeparture;

            // initialize trams to depot
            for (var i = 0; i < InitialTrams; i++) Trams.Add(new Tram() { Id = i, CurrentStation = Track.GetPRDepot() });

            var time = 0;

            var VIABLE_ISSUE_RANGE = (int)TimeSpan.FromMinutes(17).TotalSeconds + turnAroundTime + (int)TimeSpan.FromMinutes(17).TotalSeconds;

            var pr = Track.GetPR().Timetable;
            int j = 0;

            foreach (var nextTram in Trams)
            {
                EventQueue.Add(new TransportArgs()
                {
                    FromStation = Track.GetPRDepot(),
                    ToStation = Track.NextStation(Track.GetPRDepot()),
                    Tram = nextTram,
                    Type = TransportArgsType.ExpectedArrival,
                    TriggerTime = j + GetTravelTime(nextTram.CurrentStation, Track.NextStation(nextTram.CurrentStation))
                });
                j += 40;
            }

            while (!EventQueue.IsEmpty)
            {
                var next = EventQueue.Min();
                EventQueue.DeleteMin();
                DebugLine(this, next);

                if (next.Type == TransportArgsType.ExpectedArrival)
                    TramExpectedArrival(this, next);

                if (next.Type == TransportArgsType.ExpectedDeparture)
                    TramExpectedDeparture(this, next);

                if (next.Type == TransportArgsType.Arrival)
                    TramArrived(this, next);

                if (next.Type == TransportArgsType.Departure)
                    TramDeparture(this, next);

                next = EventQueue.Min();
            }

            var totalServedInDay = 0;

            foreach (var tram in Trams) totalServedInDay += tram.ServedPassengers;

            // TODO: fill up
            // return output statistics for the output analysis
            return new TramStatistics()
            {
                TotalPassengersServiced = totalServedInDay,
                Punctuality = TotalPunctuality,
                TotalDelay = TotalDelay,
                StationPassengerCongestion = 0,
                HighLatenessTrams = Trams.Count(x => x.WasLate)
            };
        }

        public void HandleExpectedDeparture(object sender, TransportArgs e)
        {
            e.FromStation.CurrentTram = null;

            var delayAtStation = 0;

            if (e.ToStation.TimeOfLastTram.HasValue)
                delayAtStation = (e.ToStation.TimeOfLastTram.Value + MinimumTimeBetweenTrains) - e.TriggerTime;

            if (delayAtStation > 0)
                TotalDelay += delayAtStation;
            else
                delayAtStation = 0;

            EventQueue.Add(new TransportArgs()
            {
                FromStation = e.FromStation,
                Tram = e.Tram,
                ToStation = e.ToStation,
                TriggerTime = e.TriggerTime + delayAtStation,
                Type = TransportArgsType.Departure
            });
        }

        public void HandleExpectedArrival(object sender, TransportArgs e)
        {
            e.Tram.CurrentStation = e.ToStation;

            if (e.ToStation.CurrentTram == null)
            {
                EventQueue.Add(new TransportArgs()
                {
                    Tram = e.Tram,
                    FromStation = e.FromStation,
                    ToStation = e.ToStation,
                    Type = TransportArgsType.Arrival,
                    TriggerTime = e.TriggerTime,
                });
            }
            else
            {
                e.ToStation.Trams.Enqueue(e.Tram);
            }//enqueue it to the station for arrival when the other trams leave it
        }

        private void HandleDeparture(object sender, TransportArgs e)
        {
            e.Tram.CurrentStation = null;

            //Remember when the tram departed from the terminal
            //to know in which timetable cycle it should arrive
            if (e.FromStation.IsTerminal) e.Tram.DepartureFromPreviousTerminal = e.TriggerTime;

            // remove tram from station
            e.FromStation.TimeOfLastTram = e.TriggerTime;

            if (e.FromStation.Trams.Count > 0)
            {
                EventQueue.Add(new TransportArgs()
                {
                    //assume it happens instantly
                    Tram = e.FromStation.Trams.Dequeue(),
                    FromStation = e.Tram.LastActiveStation,
                    ToStation = e.FromStation,
                    TriggerTime = e.TriggerTime,
                    Type = TransportArgsType.Arrival
                });
            }

            // upon departure schedule an arrival
            EventQueue.Add(new TransportArgs()
            {
                FromStation = e.FromStation,
                ToStation = e.ToStation,
                TriggerTime = e.TriggerTime + GetTravelTime(e.FromStation, e.ToStation),
                Tram = e.Tram,
                Type = TransportArgsType.ExpectedArrival
            });
        }

        private void HandleArrival(object sender, TransportArgs e)
        {
            //muy importante not to forget this
            e.ToStation.CurrentTram = e.Tram;

            //train has arrived at station and is ready to serve passengers
            var totalEmbarkingPassengers = 0;
            var totalDisembarkingPassengers = 0;

            //Disembark passengers if neccessary
            EmbarkDisembarkPassengers(e, ref totalEmbarkingPassengers, ref totalDisembarkingPassengers);

            //compute how long we have to wait
            var stationDwell = GetStationTime(totalDisembarkingPassengers, totalDisembarkingPassengers);

            //we need to make a turnaround now, this takes Turnaround time
            if (e.ToStation.IsTerminal && e.FromStation != Track.GetPRDepot())
            {
                // people board while waiting
                var currentTime = e.TriggerTime + Math.Max(TurnaroundTime, stationDwell);
                var tramDepartureFromTerminal = e.Tram.DepartureFromPreviousTerminal;
                //Get next departure time
                var nextDepartureTime = e.ToStation.Timetable.GetNextDepartureTime(tramDepartureFromTerminal);

                if (nextDepartureTime == null) return;
                var dep = nextDepartureTime.Value;

                //we satisfied this departure time
                var sch = e.ToStation.Timetable.Schedule;

                //mark departure time as used
                e.ToStation.Timetable[dep] = 0;

                //how long more did we take than expected?
                var punctuality = currentTime - dep;

                //Check if this tram was very late:)
                if (punctuality > 60)
                {
                    e.Tram.WasLate = true;
                }

                //arriving early is good :))))
                TotalPunctuality += Math.Max(punctuality, 0);

                // upon arrival schedule a departure
                EventQueue.Add(new TransportArgs()
                {
                    FromStation = e.ToStation,
                    ToStation = Track.NextStation(e.ToStation),
                    TriggerTime = dep,
                    Tram = e.Tram,
                    Type = TransportArgsType.ExpectedDeparture
                });
            }
            else
            {
                EventQueue.Add(new TransportArgs()
                {
                    FromStation = e.ToStation,
                    ToStation = Track.NextStation(e.ToStation),
                    TriggerTime = e.TriggerTime + stationDwell,
                    Tram = e.Tram,
                    Type = TransportArgsType.ExpectedDeparture
                });
            }
        }

        private void EmbarkDisembarkPassengers(TransportArgs e, ref int totalEmbarkingPassengers, ref int totalDisembarkingPassengers)
        {
            var min = 0;
            if (e.ToStation.TimeOfLastTram.HasValue)
            {
                min = e.ToStation.TimeOfLastTram.Value;
            }

            // handle boarding ? how many people can board?
            totalDisembarkingPassengers = e.Tram.GetDisembarkingPassengers(e.ToStation, e.TriggerTime);

            // get the number of people that have been here before
            var leftOverPeople = e.ToStation.LeftBehind;

            // increment the waiting time of these people it will be the number of the people * the time the platform has been empty. 
            e.ToStation.IncrementLeftBehindAverageWaiting(e.TriggerTime - min);

            // get the number of new passengers that arrived at the platform while it had no tram.
            var toEmbark = e.ToStation.GetEmbarkingPassengers(e.Tram, e.TriggerTime);

            // people exit train
            e.Tram.CurrentPassengers -= totalDisembarkingPassengers;

            // This many people can enter
            var canEnter = Math.Max(0, MAX_PASSENGERS_IN_TRAIN - e.Tram.CurrentPassengers);

            // compute net entering
            var totalEntering = Math.Min(canEnter, toEmbark);

            // Passengers board
            e.Tram.CurrentPassengers += totalEntering;
            e.Tram.ServedPassengers += totalEntering;

            // First we have to board the left behind people because they put the waiting times through the roof.
            if (leftOverPeople <= canEnter)
            {
                e.Tram.CurrentPassengers += leftOverPeople;
                totalEmbarkingPassengers += leftOverPeople;
                e.ToStation.LeftBehind -= leftOverPeople;
            }
            else
            {
                // Some get in, but some people will be left behind (again).
                e.Tram.CurrentPassengers += canEnter;
                totalEmbarkingPassengers += canEnter;

                // Now, count the people that are left behind.
                e.ToStation.LeftBehind = leftOverPeople - canEnter;
            }

            // Re-compute the number of people entering
            canEnter = Math.Max(0, MAX_PASSENGERS_IN_TRAIN - e.Tram.CurrentPassengers);

            // Easy case, the people that have been waiting, can board. All of them
            if (toEmbark <= canEnter)
            {
                e.Tram.CurrentPassengers += toEmbark;
                totalEmbarkingPassengers += toEmbark;
            }
            else
            {
                // Some get in, but some people will be left behind.
                e.Tram.CurrentPassengers += canEnter;
                totalEmbarkingPassengers += canEnter;

                // Now, the toEmbark, become LeftBehind. 
                var leftBehind = toEmbark - canEnter;

                e.ToStation.LeftBehind += leftBehind;
            }
        }

        /// <summary>
        /// Returns the amount of time to be waited at the station
        /// </summary>
        /// <param name="atStation">The station for which we're computing the waiting time</param>
        /// <param name="triggerTime">The time when the arrival happens</param>
        /// <returns>The time we're gonna wait at that station</returns>
        public int GetStationTime(int embarkingPassengers, int disembarkingPassengers)
        {
            var stationTime = 12.5 * 0.27 * embarkingPassengers + 0.13 * disembarkingPassengers;
            return Math.Max(10, (int)Math.Ceiling(stationTime));
        }

        /// <summary>
        /// Get travel time between stations in secs
        /// </summary>
        /// <param name="fromStation"></param>
        /// <param name="toStation"></param>
        /// <returns></returns>
        public int GetTravelTime(Station fromStation, Station toStation)
        {
            if (Track.TryGetEdges(fromStation, toStation, out IEnumerable<UEdge> e))
            {
                Debug.Assert(e.Count() == 1);
                var edge = e.FirstOrDefault();
                return int.Parse(Math.Round(TimeSpan.FromSeconds(edge.Weight).TotalSeconds, 1).ToString());
            }
            throw new Exception($"Unable to find connection between {fromStation.Name} and {toStation.Name}");
        }
    }
}