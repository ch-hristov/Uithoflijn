using System;

namespace Uithoflijn
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var sm = new StateManager();
            Console.CancelKeyPress += (a, b) => sm.WriteState();
            sm.Start();

            //Write the state on cancel
            sm.WriteState();
        }

        public void ComputeCycleLength()
        {
            DateTime dt = new DateTime(2000, 1, 1, 6, 30, 0);
            DateTime dt2 = new DateTime(2000, 1, 1, 21, 30, 0);
            Console.WriteLine((dt2 - dt).TotalSeconds);
        }

        public void ComputeIntervalsInDay()
        {
            //TODO>>
            var dt1 = new DateTime(2000, 1, 1, 1, 1, 1);

        }
    }
}
