using QuickGraph;
using QuickGraph.Serialization.DirectedGraphML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uithoflijn
{
    public class Terrain : AdjacencyGraph<Station, UEdge>
    {
        private const string T1 = "Centraal Station";
        private const string T2 = "P+R De Uithof";

        public Terrain()
        {
            var names = new List<string>() {  T1, "Vaartsche Rijn", "Galgenwaard", "Kromme Rijn","Padualaan",
                                                       "Heidelberglaan",  "UMC", "WKZ" , T2};


            var forward = new List<int>() { 134, 243, 59, 101, 60, 86, 78, 113 };
            var backwards = new List<int>() { 110, 78, 82, 60, 100, 59, 243, 135 };
            var id = 0;

            //For both directions
            for (var x = 0; x < 2; x++)
            {
                var vertices = new List<Station>();

                //reverse to make it into the correct order
                if (x == 1)
                    names.Reverse();

                // add vertices temporarily (not in the graph yet)
                names.ForEach(name =>
                {
                    vertices.Add(new Station()
                    {
                        Name = name,
                        IsTerminal = name == T1 || name == T2,
                        Id = id
                    });
                    id++;
                });

                // add the to the graph
                foreach (var item in vertices)
                    AddVertex(item);

                // Add the edge weights
                for (var j = 0; j < names.Count - 1; j++)
                {
                    var weight = -1;

                    if (x == 0)
                        weight = forward[j];
                    else
                        weight = backwards[j];

                    var edge = new UEdge(vertices[j], vertices[j + 1])
                    {
                        Weight = weight
                    };

                    AddEdge(edge);
                }
            }

            foreach (var item in Edges)
                Console.WriteLine(item.Weight);
        }

        public Station GetStationTerminal(int v)
        {
            if (v == 0) return Vertices.FirstOrDefault(x => x.Name == T1);
            if (v == 1) return Vertices.FirstOrDefault(x => x.Name == T2);
            throw new Exception($"Use 0 for {T1} or 1 for {T2}");
        }
    }
}
