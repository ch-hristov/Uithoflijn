using System;
using System.Collections.Generic;
using System.Text;

namespace Uithoflijn
{
    public class Station
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsTerminal { get; internal set; }

        public double WaitingPeople { get; internal set; }

        public IEnumerable<UEdge> OutEdges { get; internal set; }

        public IEnumerable<UEdge> InEdges { get; internal set; }
        public Tram CurrentTram { get; internal set; }

        public override string ToString()
        {
            return $"[{Id}]{Name}";
        }

        public override bool Equals(object obj)
        {
            var c = obj as Station;
            return Id == c.Id;
        }

        // There was an alert about this on my computer, so I added it. P
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }

        public int GetEmbarkingPassengers(Tram tram, int time)
        {
            //TODO:
            return (int)(WaitingPeople * 0.1);
        }


    }
}
