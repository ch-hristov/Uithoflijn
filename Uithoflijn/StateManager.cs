using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uithoflijn
{
    public class StateManager
    {
        public int InitialTrams { get; set; }
        public event EventHandler<TransportArgs> TramArrived = delegate { };
        public event EventHandler<TransportArgs> TramDeparture = delegate { };
        public Queue

        public void Start()
        {
            InitialTrams = 1;

            TramArrived += HandleArrival;
            TramDeparture += HandleDeparture;

            var T = 0;
            var track = new Terrain();
            var eventQueue = new Queue<TransportArgs>();
            var trams = new List<Tram>();

            // initialize trams
            for (var i = 0; i < InitialTrams; i++) trams.Add(new Tram() { Id = i });
            foreach (var tram in trams) TramArrived(this, new TransportArgs() { FromStation = null, ToStation = track.GetStationTerminal(0), Tram = tram });

            while (true)
            {
                Console.WriteLine($"Time step : {T}");

                var events = new Queue<TransportArgs>(eventQueue.Where(x => x.TriggerTime == T).OrderByDescending(x => x.Priority));

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
        }

        public void HandleArrival(object sender, TransportArgs e)
        {
            
        }

        public void HandleDeparture(object sender, TransportArgs e)
        {
            
        }
    }
}
