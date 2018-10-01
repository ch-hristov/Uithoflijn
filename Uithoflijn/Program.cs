using System;

namespace Uithoflijn
{
    public class Program
    {
        public static void Main(string[] args)
        {
           

            StateManager sm = new StateManager();
            sm.Start();


        }

        public void ComputeCycleLength()
        {
            DateTime dt = new DateTime(2000, 1, 1, 6, 30, 0);
            DateTime dt2 = new DateTime(2000, 1, 1, 21, 30, 0);
            Console.WriteLine((dt2 - dt).TotalSeconds);
        }
    }
}
