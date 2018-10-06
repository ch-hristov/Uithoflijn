using System;

namespace Uithoflijn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var sm = new StateManager();
            Console.WriteLine("Cycle length " + ComputeCycleLength());
            Console.CancelKeyPress += (a, b) => sm.WriteState();
            sm.Start();

            //Write the state on cancel
            sm.WriteState();
        }

        public static double ComputeCycleLength()
        {
            var dt = new DateTime(2000, 1, 1, 6, 30, 0);
            var dt2 = new DateTime(2000, 1, 1, 21, 30, 0);
            Console.WriteLine((dt2 - dt).TotalMinutes);
            return 0;
        }

        public void ComputeIntervalsInDay()
        {
            //TODO>>
            var dt1 = new DateTime(2000, 1, 1, 1, 1, 1);
        }


    }
}
