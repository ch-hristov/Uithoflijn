namespace Uithoflijn
{
    public class TramStatistics
    {
        public int TotalDelay { get; set; }
        public int Punctuality { get; set; }
        public int TotalPassengersServiced { get; set; }
        public int StationPassengerCongestion { get; set; }
        public int HighLatenessTrams { get; internal set; }
        public double TotalWaitingTime { get; internal set; }

        public override string ToString()
        {
            return $"{TotalDelay};{Punctuality};{TotalPassengersServiced};{StationPassengerCongestion};{HighLatenessTrams};{TotalWaitingTime}";
        }

        public string GetHeader()
        {
            return "delay;punctuality;serviced;congestion;highLatenessTrams;total_waiting_time";
        }
    }
}
