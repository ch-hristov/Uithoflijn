using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Uithoflijn
{
    public class StateManager
    {
        public int InitialTrams { get; set; }
        private const int TotalTime = 54000;
        public int TrainIssueFrequency { get; set; }

        public event EventHandler<TransportArgs> TramArrived = delegate { };
        public event EventHandler<TransportArgs> TramDeparture = delegate { };

        public Queue<TransportArgs> EventQueue = new Queue<TransportArgs>();
        public Terrain Track { get; set; }

        public void Start()
        {
            //Issue lots of trams(iliketrains)
            InitialTrams = 50;
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
                T++;
            }

            Console.WriteLine("Simulation finished...");
        }

        public void HandleDeparture(object sender, TransportArgs e)
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

        public void HandleArrival(object sender, TransportArgs e)
        {
            Console.WriteLine($"{e.Tram.Id} arrived at station {e.ToStation.ToString()}");

            // upon arrival schedule a departure
            EventQueue.Enqueue(new TransportArgs()
            {
                FromStation = e.ToStation,
                ToStation = Track.NextStation(e.ToStation),
                TriggerTime = e.TriggerTime + GetStationTime(e.ToStation, e.TriggerTime),
                Tram = e.Tram,
                Type = TransportArgsType.Departure
            });
        }

        /// <summary>
        /// Returns the amount of time to be waited at the station
        /// </summary>
        /// <param name="atStation">The station for which we're computing the waiting time</param>
        /// <param name="triggerTime">The time when the arrival happens</param>
        /// <returns>The time we're gonna wait at that station</returns>
        private int GetStationTime(Station atStation, int triggerTime)
        {
            return 10;
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
