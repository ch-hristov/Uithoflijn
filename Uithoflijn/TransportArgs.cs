using System;

namespace Uithoflijn
{
    public class TransportArgs : EventArgs
    {
        public int Priority { get; internal set; }
        public int TriggerTime { get; set; }

        public Tram Tram { get; set; }
        public Station FromStation { get; set; }
        public Station ToStation { get; set; }
        public TransportArgsType Type { get; set; }


        public override string ToString()
        {
            return $"[{Type.ToString("G")}]{Tram.Id} from {FromStation.Name}[{FromStation.Id}] -> {ToStation.Name}[{ToStation.Id}] / T = {TriggerTime}";
        }
    }

    public enum TransportArgsType
    {
        Arrival,
        Departure
    }
}
