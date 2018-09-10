using QuickGraph;
using System;
using System.Collections.Generic;
using System.Text;

namespace Uithoflijn
{
    public class UEdge : Edge<Station>
    {
        public int Weight { get; set; }

        public UEdge(Station source, Station target) : base(source, target)
        {

        }
    }
}
