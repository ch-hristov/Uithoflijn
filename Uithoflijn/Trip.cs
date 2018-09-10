using QuickGraph;
using QuickGraph.Serialization.DirectedGraphML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uithoflijn
{
    public class Trip : AdjacencyGraph<Station, UEdge>
    {
        public Trip()
        {
            var names = new List<string>() {  "Centraal Station", "Vaartsche Rijn", "Galgenwaard", "Kromme Rijn","Padualaan",
                                                       "Heidelberglaan",  "UMC", "WKZ" ,"P+R De Uithof"};

            var forward = new List<int>() { 134, 243, 59, 101, 60, 86, 78, 113 };
            var backwards = new List<int>() { 110, 78, 82, 60, 100, 59, 243, 135 };
            var id = 0;

            for (int x = 0; x < 2; x++)
            {
                var vertices = new List<Station>();

                if (x == 1)
                {
                    names.Reverse();
                }

                names.ForEach(name =>
                {
                    vertices.Add(new Station()
                    {
                        Name = name,
                        IsTerminal = name == "Centraal Station" || name == "P+R De Uithof",
                        Id = id
                    });
                    id++;
                });

                foreach (var item in vertices)
                    AddVertex(item);

                for (int j = 0; j < names.Count - 1; j++)
                {
                    int weight = -1;
                    if (x == 0)
                    {
                        weight = forward[j];
                    }
                    else
                    {
                        weight = backwards[j];
                    }
                    var edge = new UEdge(vertices[j], vertices[j + 1]);
                    AddEdge(edge);
                }
            }

            Console.WriteLine();
        }
    }
}
