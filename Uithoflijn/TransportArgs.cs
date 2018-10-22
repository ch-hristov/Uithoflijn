using System;

namespace Uithoflijn
{
    public class TransportArgs : EventArgs, IComparable<TransportArgs>
    {
        public int TriggerTime { get; set; }

        public Tram Tram { get; set; }

        public Station FromStation { get; set; }

        public Station ToStation { get; set; }

        public TransportArgsType Type { get; set; }

        /// <summary>
        /// Used by the priority queue
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(TransportArgs other)
        {
            if (TriggerTime > other.TriggerTime) return 1;
            if (TriggerTime < other.TriggerTime) return -1;
            return 0;
        }

        public override string ToString()
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
    }

    public enum TransportArgsType
    {
        Departure,
        Arrival,
        ExpectedDeparture,
        ExpectedArrival,
    }
}
