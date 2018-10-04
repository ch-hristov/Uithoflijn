using System;
using System.Collections.Generic;

namespace Uithoflijn
{
    public class Tram
    {
        public List<double> _disembarkmentGeneralProbCSPR = new List<double>()
        {
            0,0.086763924,0.062557774,0.052848369,0.517963103,0.642401152,0.797902237,0.977346689,1
        };

        //TODO stuff;
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


        public int Id { get; set; }

        public int CurrentPassengers { get; set; }

        public int ServedPassengers { get; set; }

        public Station CurrentStation { get; internal set; }

        public int GetDisembarkingPassengers(Station atStation, int time)
        {
            return 0;
        }

    }
}
