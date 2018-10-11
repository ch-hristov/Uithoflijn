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
        private const int MinimumDistanceBetweenTrains = 40;
        private const int MAX_PASSENGERS_IN_TRAIN = 420;

        private const string opt = "optimal.txt";

        public int InitialTrams { get; set; }
        public event EventHandler<TransportArgs> TramArrived = delegate { };
        public event EventHandler<TransportArgs> TramDeparture = delegate { };
        public event EventHandler<TransportArgs> Idle = delegate { };

        public Queue<TransportArgs> EventQueue = new Queue<TransportArgs>();
        public ICollection<Tram> Trams = new List<Tram>();

        public Terrain Track { get; set; }
        public int CurrentFrequency { get; private set; }

        private List<bool> _state = new List<bool>(54000);
        private const int _DELAY_PROBE = 10;
        public int TotalDelay { get; set; }

        /// <summary>
        /// Generate a timetable for each second
        /// The way to generate the time table must be determined
        /// </summary>
        public void GenerateTimeTable()
        {
            if (File.Exists(opt))
            {
                var text = File.ReadAllText(opt);

                _state = text.Split(' ')
                             .Select(x => int.Parse(x) == 0 ? false : true)
                             .ToList();
            }
            else
            {
                for (int i = 0; i < TotalTime; i++)
                    _state[i] = false;
            }
        }


        public class TramStatistics
        {
            public int TotalDelay { get; set; }
            public int Punctuality { get; set; }
            public int TotalPassengersServiced { get; set; }


            public override string ToString()
            {
                return $"{TotalDelay};{Punctuality};{TotalPassengersServiced}";
            }
        }

        public TramStatistics Start(int frequency, int numberOfTrams)
        {
            //Issue lots of trams(iliketrains)
            InitialTrams = numberOfTrams;
            Track = new Terrain();

            CurrentFrequency = numberOfTrams;

            TramArrived += HandleArrival;
            TramDeparture += HandleDeparture;
            Idle += HandleIdle;

            // initialize trams
            for (var i = 0; i < InitialTrams; i++)
                Trams.Add(new Tram() { Id = i });

            foreach (var tram in Trams)
            {
                TramArrived(this, new TransportArgs()
                {
                    FromStation = null,
                    ToStation = Track.GetCSDepot(),
                    Tram = tram
                });
            }

            var T = 0;
            Idle(this, new TransportArgs() { TriggerTime = -1 });

            while (T <= TotalTime || EventQueue.Any())
            {
                //if (T % 9000 == 0) Console.WriteLine($"Progress: " + 100 * Math.Round((double)T / TotalTime, 2));
                //process events from the end station to the first station to compute
                //the delays correctly
                var events = new Queue<TransportArgs>(EventQueue.Where(x => x.TriggerTime == T)
                                                                .OrderByDescending(x => x.ToStation != null ? x.ToStation.Id : int.MinValue));
                while (events.Any())
                {
                    var next = events.Dequeue();

                    if (next.Type == TransportArgsType.Arrival)
                        TramArrived(this, next);

                    if (next.Type == TransportArgsType.Departure)
                        TramDeparture(this, next);

                    if (T <= TotalTime && next.Type == TransportArgsType.Idle)
                        Idle(this, next);
                }

                EventQueue = new Queue<TransportArgs>(EventQueue.Where(x => x.TriggerTime >= T));
                T++;
            }

            var totalServedInDay = 0;

            foreach (var tram in Trams)
                totalServedInDay += tram.ServedPassengers;


            return new TramStatistics()
            {
                TotalPassengersServiced = totalServedInDay,
                Punctuality = 0,
                TotalDelay = 0
            };
        }

        public void WriteState()
        {
            File.WriteAllText(opt, string.Join(" ", _state.Select(x => x ? 1 : 0)));
            Console.WriteLine("State saved");
        }

        private void HandleIdle(object sender, TransportArgs e)
        {
            if (e.TriggerTime % CurrentFrequency == 0)
            {
                foreach (var tram in Trams)
                {
                    if (Track.GetCSDepot() == tram.CurrentStation)
                    {
                        EventQueue.Enqueue(new TransportArgs()
                        {
                            FromStation = tram.CurrentStation,
                            ToStation = Track.NextStation(tram.CurrentStation),
                            Priority = 0,
                            Tram = tram,
                            Type = TransportArgsType.Departure,
                            TriggerTime = e.TriggerTime + GetTravelTime(tram.CurrentStation, Track.NextStation(tram.CurrentStation))
                        });
                        break;
                    }
                }
            }

            EventQueue.Enqueue(new TransportArgs()
            {
                TriggerTime = e.TriggerTime + 1,
                Type = TransportArgsType.Idle,
            });
        }

        private void HandleDeparture(object sender, TransportArgs e)
        {
            // remove tram from station
            e.FromStation.CurrentTram = null;
            e.FromStation.TimeFromLastTram = e.TriggerTime;

            // upon departure schedule an arrival
            EventQueue.Enqueue(new TransportArgs()
            {
                FromStation = e.FromStation,
                ToStation = e.ToStation,
                TriggerTime = e.TriggerTime + GetTravelTime(e.FromStation, e.ToStation),
                Tram = e.Tram,
                Type = TransportArgsType.Arrival
            });
        }

        private void HandleArrival(object sender, TransportArgs e)
        {
            //Disembark passengers if neccessary
            if (e.ToStation != e.Tram.CurrentStation)
            {
                // handle boarding ? how many people can board?
                var toDisembark = e.Tram.GetDisembarkingPassengers(e.ToStation, e.TriggerTime);

                //TODO: Write the function inside the station to retrieve embarking passengers
                var toEmbark = e.ToStation.GetEmbarkingPassengers(e.Tram, e.TriggerTime);
                e.ToStation.CurrentTram = e.Tram;
                e.Tram.CurrentStation = e.ToStation;

                // people exit train
                e.Tram.CurrentPassengers -= toDisembark;

                // This many people can enter
                var canEnter = Math.Max(0, MAX_PASSENGERS_IN_TRAIN - e.Tram.CurrentPassengers);

                // compute net entering
                var totalEntering = Math.Min(canEnter, toEmbark);

                // Passengers board
                e.Tram.CurrentPassengers += totalEntering;
                e.Tram.ServedPassengers += totalEntering;

                //people enter train through station
                e.ToStation.WaitingPeople -= totalEntering;
            }

            //Don't immediately schedule departure if the trams just arrived at the depot,
            //This is done by the idle event
            if (e.ToStation != Track.GetCSDepot())
            {
                if (TotalTime <= e.TriggerTime && e.ToStation == Track.GetCS())
                {
                    // Do not issue more trains from depot, 
                    return;
                }
                else
                {
                    var delay = GetDelaymentTime(e.Tram, Track.NextStation(e.Tram.CurrentStation));

                    if (delay == 0)
                    {
                        // upon arrival schedule a departure if that's possible
                        EventQueue.Enqueue(new TransportArgs()
                        {
                            FromStation = e.ToStation,
                            ToStation = Track.NextStation(e.ToStation),
                            TriggerTime = e.TriggerTime + (int)Math.Ceiling(GetStationTime(e.ToStation, e.Tram, e.TriggerTime)),
                            Tram = e.Tram,
                            Type = TransportArgsType.Departure
                        });
                    }
                    else
                    {
                        foreach (var tram in Trams)
                        {
                            if (tram.LastActiveStation.Id < e.ToStation.Id)
                            {

                            }
                        }

                        TramArrived(this, new TransportArgs()
                        {
                            FromStation = e.FromStation,
                            ToStation = e.ToStation,
                            TriggerTime = e.TriggerTime + delay,
                            Type = e.Type,
                            Priority = 0,
                            Tram = e.Tram
                        });

                        TotalDelay += _DELAY_PROBE;
                    }
                }
            }
        }

        private int GetDelaymentTime(Tram tram, Station nextStation)
        {
            //there's no tram at next station so it's safe to go
            if (nextStation.CurrentTram == null) return 0;

            //there's a tram in the next station, but will it leave after we travel?
            var nextStationTram = nextStation.CurrentTram;

            // is the next tram late, and by how much?
            var delay = 0;

            return delay;
        }

        /// <summary>
        /// Returns the amount of time to be waited at the station
        /// </summary>
        /// <param name="atStation">The station for which we're computing the waiting time</param>
        /// <param name="triggerTime">The time when the arrival happens</param>
        /// <returns>The time we're gonna wait at that station</returns>
        public double GetStationTime(Station atStation, Tram tram, int triggerTime)
        {
            //TODO: Fix this
            var stationTime = Math.Max(60,
                12.5 * 0.27 * atStation.WaitingPeople + 0.13 * tram.GetDisembarkingPassengers(atStation, triggerTime));

            //TODO check?
            return Math.Min(stationTime, 1000);
        }

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
