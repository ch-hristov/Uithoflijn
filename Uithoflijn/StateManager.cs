using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Uithoflijn
{
    public struct Interval
    {
        public int LeftInterval;
        public int RightInterval;
    }

    public class StateManager
    {
        private const int Frequency = 2;
        private const int TotalTime = 54000;
        private const int MinimumDistanceBetweenTrains = 40;
        private const int MAX_PASSENGERS_IN_TRAIN = 420;

        private const string opt = "optimal.txt";

        public int InitialTrams { get; set; }
        public List<Interval> Intervals = new List<Interval>();

        public event EventHandler<TransportArgs> TramArrived = delegate { };
        public event EventHandler<TransportArgs> TramDeparture = delegate { };
        public event EventHandler<TransportArgs> Idle = delegate { };

        public Queue<TransportArgs> EventQueue = new Queue<TransportArgs>();
        public ICollection<Tram> Trams = new List<Tram>();

        public Terrain Track { get; set; }
        private List<bool> _state = new List<bool>(54000);
        private const int _DELAY = 10;
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

        public void Start()
        {
            //Issue lots of trams(iliketrains)
            InitialTrams = 1;
            Track = new Terrain();

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
                var events = new Queue<TransportArgs>(EventQueue.Where(x => x.TriggerTime == T)
                                                                .OrderByDescending(x => x.ToStation != null ? x.ToStation.Id : int.MinValue));
                Track.PassengersArrive(T);

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

            Console.WriteLine("Simulation finished...");
        }

        public void WriteState()
        {
            File.WriteAllText(opt, string.Join(" ", _state.Select(x => x ? 1 : 0)));
            Console.WriteLine("State saved");
        }

        private void HandleIdle(object sender, TransportArgs e)
        {
            if (e.TriggerTime % Frequency == 0)
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
            int TotalEmbarkingPassengers = 0;
            int TotalDisembarkingPassengers = 0;
            //Disembark passengers if neccessary
            if (e.ToStation != e.Tram.CurrentStation)
            {
                // handle boarding ? how many people can board?
                TotalDisembarkingPassengers = e.Tram.GetDisembarkingPassengers(e.ToStation, e.TriggerTime);

                // get the number of people that have been here before
                int leftOverPeople = e.ToStation.LeftBehind;

                // increment the waiting time of these people it will be the number of the people * the time the platform has been empty. 
                e.ToStation.IncrementLeftBehindAverageWaiting(e.TriggerTime - e.ToStation.TimeFromLastTram);

                // get the number of new passengers that arrived at the platform while it had no tram.
                int toEmbark = e.ToStation.GetEmbarkingPassengers(e.Tram, e.TriggerTime);
                e.ToStation.CurrentTram = e.Tram;
                e.Tram.CurrentStation = e.ToStation;

                // people exit train
                e.Tram.CurrentPassengers -= TotalDisembarkingPassengers;

                // Increment the number of passengers serviced by the Tram
                e.Tram.ServedPassengers += TotalDisembarkingPassengers;

                // This many people can enter
                var canEnter = Math.Max(0, MAX_PASSENGERS_IN_TRAIN - e.Tram.CurrentPassengers);

                // First we have to board the left behind people because they put the waiting times through the roof.
                if (leftOverPeople <= canEnter)
                {
                    e.Tram.CurrentPassengers += leftOverPeople;
                    TotalEmbarkingPassengers += leftOverPeople;
                    e.ToStation.LeftBehind -= leftOverPeople;
                }
                else 
                {
                    // Some get in, but some people will be left behind (again).
                    e.Tram.CurrentPassengers += canEnter;
                    TotalEmbarkingPassengers += canEnter;

                    // Now, count the people that are left behind.
                    e.ToStation.LeftBehind = leftOverPeople - canEnter;
                }


                // Re-compute the number of people entering
                canEnter = Math.Max(0, MAX_PASSENGERS_IN_TRAIN - e.Tram.CurrentPassengers);

                // Easy case, the people that have been waiting, can board. All of them
                if (toEmbark <= canEnter)
                {
                    e.Tram.CurrentPassengers += toEmbark;
                    TotalEmbarkingPassengers += toEmbark;
                }
                else
                {
                    // Some get in, but some people will be left behind.
                    e.Tram.CurrentPassengers += canEnter;
                    TotalEmbarkingPassengers += canEnter;

                    // Now, the toEmbark, become LeftBehind. 
                    int leftBehind = toEmbark - canEnter;

                    e.ToStation.LeftBehind += leftBehind;
                }

            }

            //Don't immediately schedule departure if the trams just arrived at the depot,
            //This is done by the idle event
            if (e.ToStation != Track.GetCSDepot())
            {
                if (TotalTime <= e.TriggerTime && e.ToStation == Track.GetCS())
                {
                    // Do not issue more trains from CS
                    return;
                }
                else
                {
                    var delay = CanScheduleDeparture(e.Tram, Track.NextStation(e.Tram.CurrentStation));

                    if (delay == 0)
                    {
                        // upon arrival schedule a departure
                        EventQueue.Enqueue(new TransportArgs()
                        {
                            FromStation = e.ToStation,
                            ToStation = Track.NextStation(e.ToStation),
                            TriggerTime = e.TriggerTime + (int)Math.Ceiling(GetStationTime(TotalDisembarkingPassengers, TotalDisembarkingPassengers)),
                            Tram = e.Tram,
                            Type = TransportArgsType.Departure
                        });
                    }
                    else
                    {
                        TramArrived(this, new TransportArgs()
                        {
                            FromStation = e.FromStation,
                            ToStation = e.ToStation,
                            TriggerTime = e.TriggerTime + _DELAY,
                            Type = e.Type,
                            Priority = 0,
                            Tram = e.Tram
                        });

                        TotalDelay += _DELAY;
                    }
                }
            }
        }

        private int CanScheduleDeparture(Tram tram, Station station)
        {
            // return whether a delay should be issued for the departure of this train
            if (station.CurrentTram != null)
                return _DELAY;

            return 0;
        }

        /// <summary>
        /// Returns the amount of time to be waited at the station
        /// </summary>
        /// <param name="atStation">The station for which we're computing the waiting time</param>
        /// <param name="triggerTime">The time when the arrival happens</param>
        /// <returns>The time we're gonna wait at that station</returns>
        public double GetStationTime(int embarkingPassengers, int disembarkingPassengers)
        {
            var stationTime = Math.Max(60,
                12.5 * 0.27 * embarkingPassengers + 0.13 * disembarkingPassengers);

            return Math.Min(stationTime, 300);
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
