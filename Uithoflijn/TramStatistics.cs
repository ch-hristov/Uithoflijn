namespace Uithoflijn
{
    public class TramStatistics
    {
        public int TotalDelay { get; set; }
        public int Punctuality { get; set; }
        public int TotalPassengersServiced { get; set; }
        public int StationPassengerCongestion { get; set; }

        public override string ToString()
        {
            return $"{TotalDelay};{Punctuality};{TotalPassengersServiced}";
        }
    }
}
