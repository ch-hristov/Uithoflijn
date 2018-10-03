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
        private const int TotalTime = 54000;
        private const int MinimumDistanceBetweenTrains = 40;

        private const string opt = "optimal.txt";

        public int InitialTrams { get; set; }

        public List<Interval> Intervals = new List<Interval>();

        public event EventHandler<TransportArgs> TramArrived = delegate { };
        public event EventHandler<TransportArgs> TramDeparture = delegate { };
        public event EventHandler<TransportArgs> TramIdle = delegate { };

        public Queue<TransportArgs> EventQueue = new Queue<TransportArgs>();
        public Terrain Track { get; set; }

        private List<bool> _state = new List<bool>(54000);

        public List<double> _disembarkmentGeneralProbCSPR = new List<double>() {  0
                                                                ,0.086763924
                                                                ,0.062557774
                                                                ,0.052848369
                                                                ,0.517963103
                                                                ,0.642401152
                                                                ,0.797902237
                                                                ,0.977346689,
                                                                1};
        public List<double> _disembarkmentGeneralProbPRCS = new List<double>();

        //TODO stuff;
        public List<double> peak_morningCSPR = new List<double>();
        public List<double> peak_eveningCSPR = new List<double>();

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
            InitialTrams = 5;
            Track = new Terrain();

            TramArrived += HandleArrival;
            TramDeparture += HandleDeparture;

            var trams = new List<Tram>();

            // initialize trams
            for (var i = 0; i < InitialTrams; i++)
                trams.Add(new Tram() { Id = i });

            foreach (var tram in trams)
            {
                TramArrived(this, new TransportArgs()
                {
                    FromStation = null,
                    ToStation = Track.GetUithofDepot(),
                    Tram = tram
                });
            }

            var T = 0;

            while (T <= TotalTime)
            {

                var events = new Queue<TransportArgs>(EventQueue.Where(x => x.TriggerTime == T)
                                                                .OrderByDescending(x => x.Priority));
                while (events.Any())
                {
                    var next = events.Dequeue();

                    if (next.Type == TransportArgsType.Arrival)
                        TramArrived(this, next);

                    if (next.Type == TransportArgsType.Departure)
                        TramDeparture(this, next);
                }

                EventQueue = new Queue<TransportArgs>(EventQueue.Where(x => x.TriggerTime >= T));
                T++;

                if (T % 1000 == 0)
                {
                    Console.WriteLine("---------------------");
                }
            }

            Console.WriteLine("Simulation finished...");
        }

        public void WriteState()
        {
            File.WriteAllText(opt, string.Join(" ", _state.Select(x => x ? 1 : 0)));
            Console.WriteLine("State saved");
        }

        private void HandleDeparture(object sender, TransportArgs e)
        {
            Console.WriteLine($"{e.Tram.Id} departure to station {e.ToStation.ToString()}");

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
            Console.WriteLine($"{e.Tram.Id} arrived at station {e.ToStation.ToString()}");

            // handle boarding ? how many people can board?
            var toDisembark = e.Tram.GetDisembarkingPassengers(e.Tram);
            var toEmbark = e.ToStation.GetEmbarkingPassengers(e.Tram);

            // upon arrival schedule a departure
            EventQueue.Enqueue(new TransportArgs()
            {
                FromStation = e.ToStation,
                ToStation = Track.NextStation(e.ToStation),
                TriggerTime = e.TriggerTime + (int)Math.Ceiling(GetStationTime(e.ToStation, e.Tram, e.TriggerTime)),
                Tram = e.Tram,
                Type = TransportArgsType.Departure
            });
        }


        /// <summary>
        /// TODO:
        /// </summary>
        /// <param name="tram"></param>
        /// <param name="triggerTime"></param>
        /// <param name="toStation"></param>
        /// <returns></returns>
        private int GetDisembarkingPassengers(Tram tram, int triggerTime, Station toStation)
        {
            return 1;
        }

        /// <summary>
        /// Returns the amount of time to be waited at the station
        /// </summary>
        /// <param name="atStation">The station for which we're computing the waiting time</param>
        /// <param name="triggerTime">The time when the arrival happens</param>
        /// <returns>The time we're gonna wait at that station</returns>
        private double GetStationTime(Station atStation, Tram tram, int triggerTime)
        {
            return 12.5 * 0.27 * atStation.WaitingPeople + 0.13 * tram.PassengersNextOut;
        }

        private int GetTravelTime(Station fromStation, Station toStation)
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
