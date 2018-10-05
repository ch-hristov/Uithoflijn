﻿using System;
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

        public override string ToString()
        {
            return $"[{Id}]{Name}";
        }

        public override bool Equals(object obj)
        {
            var c = obj as Station;
            return Id == c.Id;
        }

        public int GetEmbarkingPassengers(Tram tram, int time)
        {
            //TODO:
            return 10;
        }
    }
}
