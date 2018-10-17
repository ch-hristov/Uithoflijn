using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Uithoflijn.Tests
{
    [TestClass]
    public class TestFrequencyOptimization
    {
        [TestMethod]
        public void TestNoLateTrams()
        {
            // a tuple consists of
            var list = new List<(int, int, int)>()
            {
                (300,5,300)
            };

            //var results = new Program().Run(list);
            //Console.WriteLine(results.FirstOrDefault());

            //Assert.IsTrue(results.Single().HighLatenessTrams == 0);
        }
    }
}
