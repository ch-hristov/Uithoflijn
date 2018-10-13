using System;

namespace Uithoflijn
{
    public class TransportArgs : EventArgs
    {
        public int TriggerTime { get; set; }

        public int ExpectedTime { get; set; }

        public Tram Tram { get; set; }

        public Station FromStation { get; set; }

        public Station ToStation { get; set; }

        public TransportArgsType Type { get; set; }

        public override string ToString()
        {
            if (Type != TransportArgsType.Idle)
            {
                string fs = "NONE";
                var id = "";

                if (FromStation != null)
                {
                    fs = FromStation.Name;
                    id = "NONE";
                }
                return $"[{Type.ToString("G")}]{Tram.Id} from {fs}[{id}] -> {ToStation.Name}[{ToStation.Id}] / T = {TriggerTime}";
            }
            return "";
        }
    }

    public enum TransportArgsType
    {
        Arrival,
        Departure,
        Idle,
        ExpectedArrival,
        ExpectedDeparture
    }
}
