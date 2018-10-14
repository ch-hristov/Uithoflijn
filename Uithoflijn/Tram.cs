using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;

namespace Uithoflijn
{
    public class Tram
    {
        public override string ToString()
        {
            return $"{Id}@{CurrentStation.Name}";
        }

        public List<double> _disembarkmentGeneralProbCSPR = new List<double>()
        {
            0,0.086763924,0.062557774,0.052848369,0.517963103,0.642401152,0.797902237,0.977346689,1
        };

        public List<double> peak_morningCSPR = new List<double>()
        {
            0,0.036248402,0.034717693,0.026773621,0.538356319,0.648449401,0.800210006,0.974718246,1
        };

        public List<double> peak_eveningCSPR = new List<double>()
        {
            0,0.138608172,0.10572386,0.149689112,0.430903442,0.522978061,0.694063584,0.997447291,1
        };

        public List<double> _disembarkmentGeneralProbPRCS = new List<double>()
        {
            0,0.00143747,0.000338226,4.34E-05   ,0.018402118,0.001572231,0.013160988,0.026607992,1
        };

        public List<Double> peak_morningPRCS = new List<double>()
        {
            0,0,0.000642275,5.48E-05,0.19169781,0.001543533,0.039547378,0.084536474,1
        };

        public List<double> peak_eveningPRCS = new List<double>()
        {
            0,0.00305499,0.000179395,9.48E-06,0.000681801,0.000750849,0.009020428,0.030002259,1
        };

        private Station _currentStation;

        public int Id { get; set; }

        public int CurrentPassengers { get; set; }

        public int ServedPassengers { get; set; }

        public Station CurrentStation { get => _currentStation; set => SetValue(value); }

        public int DepartureFromPreviousTerminal { get; set; }

        private void SetValue(Station val)
        {
            //Remember last active station
            if (CurrentStation != null)
                LastActiveStation = CurrentStation;

            _currentStation = val;
        }

        public Station LastActiveStation { get; private set; }

        public int Lateness { get; internal set; }

        public int GetDisembarkingPassengers(Station atStation, int time)
        {
            double p = 0;
            if (atStation.Id >= 0 && atStation.Id <= 8)
            {
                //morning peak 7-9
                if (time >= Utils.TimeToSeconds(Utils.GetForMinuteHour(7, 0)) && time <= Utils.TimeToSeconds(Utils.GetForMinuteHour(9, 0)))
                {
                    p = peak_morningPRCS[atStation.Id];
                }
                else if (time >= Utils.TimeToSeconds(Utils.GetForMinuteHour(16, 0)) && time <= Utils.TimeToSeconds(Utils.GetForMinuteHour(18, 0)))
                {
                    // afternoon peak 16-18
                    p = peak_eveningPRCS[atStation.Id];
                }
                else
                {
                    //otherwise
                    if (atStation.Id == -1)
                    {
                        //Everyone has to get off at the depot
                        p = 1;
                    }
                    else
                    {
                        p = _disembarkmentGeneralProbPRCS[atStation.Id];
                    }
                }
            }
            else
            {
                //morning peak 7-9
                if (time >= Utils.TimeToSeconds(Utils.GetForMinuteHour(7, 0)) && time <= Utils.TimeToSeconds(Utils.GetForMinuteHour(9, 0)))
                {
                    p = peak_morningPRCS[atStation.Id - 8];
                }
                else if (time >= Utils.TimeToSeconds(Utils.GetForMinuteHour(16, 0)) && time <= Utils.TimeToSeconds(Utils.GetForMinuteHour(18, 0)))
                {
                    // afternoon peak 16-18
                    p = peak_eveningPRCS[atStation.Id - 8];
                }
                else
                {
                    //otherwise
                    //Everyone has to get off at the depot or end station
                    if (atStation.Id == -1)
                    {
                        p = 1;
                    }
                    else
                    {
                        p = _disembarkmentGeneralProbPRCS[atStation.Id - 8];
                    }
                }
            }

            // Now that we have the probability, compute the number of passengers that come off. 
            Binomial binomialDistribution = new Binomial(p, CurrentPassengers);

            int toDisembark = binomialDistribution.Sample();
            // Return the number of successes from the binomial given the number of passengers and the probability of exiting the station.
            return toDisembark;
        }
    }
}