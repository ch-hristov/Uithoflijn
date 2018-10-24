﻿namespace Uithoflijn
{
    public class TramStatistics
    {
        public int TotalDelay { get; set; }
        public int Punctuality { get; set; }
        public int TotalPassengersServiced { get; set; }

        /// <summary>
        /// this is not used but can be implemented as a way to measure how many people stack up at the station
        /// </summary>
        public int StationPassengerCongestion { get; set; }
        public int HighLatenessTrams { get; internal set; }
        public double TotalAverageWaitingTime { get; internal set; }
        public int MaximumDepartureLateness { get; internal set; }

        public override string ToString()
        {
            return $"{TotalDelay},{Punctuality},{TotalPassengersServiced},{StationPassengerCongestion},{HighLatenessTrams},{TotalAverageWaitingTime}";
        }

        public string GetHeader()
        {
            return "delay,punctuality,serviced,congestion,highLatenessTramsCount,total_avg_waiting_time";
        }
    }
}
