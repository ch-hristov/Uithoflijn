using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Uithoflijn
{
    public class StateManager
    {
        private const int TotalTime = 54000;
        private const int MinimumTimeBetweenTrains = 40;
        private const int MAX_PASSENGERS_IN_TRAIN = 420;

        private const int _DELAY_PROBE = 10;

        private const string opt = "optimal.txt";

        public int InitialTrams { get; set; }

        public event EventHandler<TransportArgs> TramArrived = delegate { };
        public event EventHandler<TransportArgs> TramDeparture = delegate { };

        public event EventHandler<TransportArgs> TramExpectedDeparture = delegate { };
        public event EventHandler<TransportArgs> TramExpectedArrival = delegate { };

        public event EventHandler<TransportArgs> Idle = delegate { };

        public Queue<TransportArgs> EventQueue = new Queue<TransportArgs>();
        public ICollection<Tram> Trams = new List<Tram>();

        public Terrain Track { get; set; }
        public int TotalDelay { get; set; }
        public int TotalPunctuality { get; set; }

        public int TurnaroundTime { get; private set; }
        public int CurrentFrequency { get; private set; }

        public TramStatistics Start(int turnAround, int frequency, int numberOfTrams)
        {
            //Issue lots of trams(iliketrains)
            Track = new Terrain(frequency, turnAround);

            this.TurnaroundTime = turnAround;
            InitialTrams = numberOfTrams;
            CurrentFrequency = numberOfTrams;

            TramArrived += HandleArrival;
            TramDeparture += HandleDeparture;

            Idle += HandleIdle;

            TramExpectedArrival += HandleExpectedArrival;
            TramExpectedDeparture += HandleExpectedDeparture;

            // initialize trams
            for (var i = 0; i < InitialTrams; i++)
                Trams.Add(new Tram() { Id = i, CurrentStation = Track.GetPRDepot() });

            //all go to depot
            int arr_i = 0;
            foreach (var tram in Trams)
            {
                int arr = arr_i * frequency;
                HandleArrival(this, new TransportArgs()
                {
                    FromStation = null,
                    ToStation = Track.GetPRDepot(),
                    Tram = tram,
                    Type = TransportArgsType.Arrival,
                    TriggerTime = arr
                });
                arr_i++;
            }

            var T = 0;

            Idle(this, new TransportArgs()
            {
                TriggerTime = 0
            });

            while (T <= TotalTime || EventQueue.Any())
            {
                //if (T % 9000 == 0) Console.WriteLine($"Progress: " + 100 * Math.Round((double)T / TotalTime, 2));
                //process events from the end station to the first station to compute
                //the delays correctly

                while (EventQueue.Any(x => x.TriggerTime == T))
                {
                    var set = new Queue<TransportArgs>(EventQueue.Where(x => x.TriggerTime == T));
                    EventQueue = new Queue<TransportArgs>(EventQueue.Except(set));

                    while (set.Any())
                    {
                        var next = set.Dequeue();

                        if (next.ToString() != "")
                            Console.WriteLine(next.ToString());

                        if (next.Type == TransportArgsType.ExpectedArrival)
                            TramExpectedArrival(this, next);

                        if (next.Type == TransportArgsType.ExpectedDeparture)
                            TramExpectedDeparture(this, next);

                        if (next.Type == TransportArgsType.Arrival)
                            TramArrived(this, next);

                        if (next.Type == TransportArgsType.Departure)
                            TramDeparture(this, next);

                        if (T <= TotalTime && next.Type == TransportArgsType.Idle)
                            Idle(this, next);
                    }
                }

                EventQueue = new Queue<TransportArgs>(EventQueue.Where(x => x.TriggerTime > T));
                T++;
            }

            var totalServedInDay = 0;

            foreach (var tram in Trams)
                totalServedInDay += tram.ServedPassengers;

            return new TramStatistics()
            {
                TotalPassengersServiced = totalServedInDay,
                Punctuality = TotalPunctuality,
                TotalDelay = TotalDelay
            };
        }

        public void HandleExpectedDeparture(object sender, TransportArgs e)
        {
            var diff = -1;

            if (e.ToStation.TimeOfLastTram != null) diff = Math.Abs(e.TriggerTime - e.ToStation.TimeOfLastTram.Value);

            if (diff < 40 && diff != -1)
            {
                EventQueue.Enqueue(new TransportArgs()
                {
                    FromStation = e.FromStation,
                    Tram = e.Tram,
                    ToStation = e.ToStation,
                    TriggerTime = e.TriggerTime + (40 - diff),
                    Type = TransportArgsType.ExpectedDeparture
                });
                TotalDelay += 40 - diff;
            }
            else
            {
                EventQueue.Enqueue(new TransportArgs()
                {
                    FromStation = e.FromStation,
                    Tram = e.Tram,
                    ToStation = e.ToStation,
                    TriggerTime = e.TriggerTime,
                    Type = TransportArgsType.Departure
                });
            }
        }

        public void HandleExpectedArrival(object sender, TransportArgs e)
        {
            var expectedAtTime = e.ExpectedTime;
            var currentTime = e.TriggerTime;

            if (!e.ToStation.Trams.Contains(e.Tram))
                e.ToStation.Trams.Enqueue(e.Tram);

            if (e.ToStation.CurrentTram == null)
            {
                EventQueue.Enqueue(new TransportArgs()
                {
                    Tram = e.Tram,
                    FromStation = e.FromStation,
                    ToStation = e.ToStation,
                    Type = TransportArgsType.Arrival,
                    TriggerTime = e.TriggerTime,
                    ExpectedTime = e.ExpectedTime
                });
            }
            else
            {
                EventQueue.Enqueue(new TransportArgs()
                {
                    Tram = e.Tram,
                    ExpectedTime = e.ExpectedTime,
                    TriggerTime = e.TriggerTime + _DELAY_PROBE,
                    FromStation = e.FromStation,
                    ToStation = e.ToStation,
                    Type = TransportArgsType.ExpectedArrival
                });

                TotalDelay += _DELAY_PROBE;
            }
        }

        private void HandleIdle(object sender, TransportArgs e)
        {
            IssueTram(e);

            EventQueue.Enqueue(new TransportArgs()
            {
                TriggerTime = e.TriggerTime + 1,
                Type = TransportArgsType.Idle,
            });
        }

        private void IssueTram(TransportArgs e)
        {
            foreach (var terminal in Track.Vertices.Where(station => station.IsTerminal))
            {
                if (terminal.Timetable.ShouldIssueAtTime(e.TriggerTime))
                {
                    //check if we should issue a tram
                    foreach (var tram in Trams)
                    {
                        if (Track.GetPRDepot() == tram.CurrentStation)
                        {
                            EventQueue.Enqueue(new TransportArgs()
                            {
                                FromStation = tram.CurrentStation,
                                ToStation = Track.NextStation(tram.CurrentStation),
                                Tram = tram,
                                Type = TransportArgsType.Arrival,
                                TriggerTime = e.TriggerTime
                                                + GetTravelTime(tram.CurrentStation, Track.NextStation(tram.CurrentStation))
                            });
                            break;
                        }
                    }
                }
            }
        }

        private void HandleDeparture(object sender, TransportArgs e)
        {
            // remove tram from station
            if (e.FromStation.CurrentTram != null && e.FromStation != Track.GetPRDepot())
            {
                e.FromStation.Trams.Dequeue();
                e.FromStation.TimeOfLastTram = e.TriggerTime;
            }

            e.FromStation.CurrentTram = null;
            e.Tram.CurrentStation = null;

            // upon departure schedule an arrival
            EventQueue.Enqueue(new TransportArgs()
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
            //Don't immediately schedule departure if the trams just arrived at the depot(happens initially)
            if (TotalTime <= e.TriggerTime && e.ToStation == Track.GetPR())
                return;

            e.ToStation.CurrentTram = e.Tram;
            e.Tram.CurrentStation = e.ToStation;

            //train has arrived at station and is ready to serve passengers
            var totalEmbarkingPassengers = 0;
            var totalDisembarkingPassengers = 0;

            //Disembark passengers if neccessary
            EmbarkDisembarkPassengers(e, ref totalEmbarkingPassengers, ref totalDisembarkingPassengers);

            var dwell = (int)Math.Ceiling(GetStationTime(totalDisembarkingPassengers, totalDisembarkingPassengers));

            if (e.ToStation == Track.GetPRDepot()) dwell = 0;

            var scheduleCorrection = 0;
            var turnAroundAdd = 0;

            if (e.ToStation.IsTerminal && e.TriggerTime > 50)
            {
                var timeCorrection = e.ToStation.Timetable.NextFromSchedule(e.TriggerTime);

                turnAroundAdd = TurnaroundTime;

                if (timeCorrection != null)
                {
                    var punctuality = e.TriggerTime - timeCorrection.Value;
                    TotalPunctuality += punctuality;
                }
                else
                {
                    //can't jump to the next schedule
                }
            }

            // upon arrival schedule an departure
            EventQueue.Enqueue(new TransportArgs()
            {
                FromStation = e.ToStation,
                ToStation = Track.NextStation(e.ToStation),
                TriggerTime = e.TriggerTime + Math.Max(Math.Max(dwell, scheduleCorrection), turnAroundAdd),
                Tram = e.Tram,
                Type = TransportArgsType.ExpectedDeparture
            });
        }

        private void EmbarkDisembarkPassengers(TransportArgs e, ref int totalEmbarkingPassengers, ref int totalDisembarkingPassengers)
        {
            if (e.ToStation != Track.GetPRDepot() && e.ToStation.TimeOfLastTram.HasValue)
            {
                // handle boarding ? how many people can board?
                totalDisembarkingPassengers = e.Tram.GetDisembarkingPassengers(e.ToStation, e.TriggerTime);

                // get the number of people that have been here before
                var leftOverPeople = e.ToStation.LeftBehind;

                // increment the waiting time of these people it will be the number of the people * the time the platform has been empty. 
                e.ToStation.IncrementLeftBehindAverageWaiting(e.TriggerTime - e.ToStation.TimeOfLastTram.Value);

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
        }

        /// <summary>
        /// Returns the amount of time to be waited at the station
        /// </summary>
        /// <param name="atStation">The station for which we're computing the waiting time</param>
        /// <param name="triggerTime">The time when the arrival happens</param>
        /// <returns>The time we're gonna wait at that station</returns>
        public double GetStationTime(int embarkingPassengers, int disembarkingPassengers)
        {
            //TODO: Fix this
            var stationTime = Math.Max(60,
                12.5 * 0.27 * embarkingPassengers + 0.13 * disembarkingPassengers);

            //TODO check?
            return Math.Min(stationTime, 1000);
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