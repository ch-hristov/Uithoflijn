using QuickGraph;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Uithoflijn
{
    public class Terrain : AdjacencyGraph<Station, UEdge>
    {
        private const string T1 = "Centraal Station";
        private const string T2 = "P+R De Uithof";

        public Terrain()
        {
            var names = new List<string>() { T1, "Vaartsche Rijn", "Galgenwaard", "Kromme Rijn","Padualaan",
                                                       "Heidelberglaan",  "UMC", "WKZ" , T2};

            var forward = new List<int>() { 134, 243, 59, 101, 60, 86, 78, 113 };
            var backwards = new List<int>() { 110, 78, 82, 60, 100, 59, 243, 135 };

            var id = 0;

            //For both directions
            for (var x = 0; x < 2; x++)
            {
                var vertices = new List<Station>();

                //reverse to make it into the correct order
                if (x == 1) names.Reverse();

                // add vertices temporarily (not in the graph yet)
                names.ForEach(name =>
                {
                    vertices.Add(new Station()
                    {
                        Name = name,
                        IsTerminalEntry = name == T1 || name == T2,
                        IsTerminalExit = name == T1 || name == T2,
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

            AddVertex(new Station()
            {
                Name = "Depot",
                Id = -1,
            });

            var t = Vertices.Where(x => x.Name == T1).OrderBy(x => x.Id).ToList();
            var t2 = Vertices.Where(x => x.Name == T2).OrderBy(x => x.Id).ToList();

            AddEdge(new UEdge(t2[0], t2[1]) { Weight = 300 });
            AddEdge(new UEdge(t[1], t[0]) { Weight = 300 });

            //Set the terminal exits and add depot edge
            var t1s = Vertices.Where(x => x.Name == T1).OrderByDescending(x => x.Id).ToList();
            var t2s = Vertices.Where(x => x.Name == T2).OrderByDescending(x => x.Id).ToList();

            Debug.Assert(t1s.Count == t2s.Count && t1s.Count == 2);

            //Set terminal entries and exits

            t1s[0].IsTerminalEntry = true;
            t1s[0].IsTerminalExit = false;

            t1s[1].IsTerminalExit = true;
            t1s[1].IsTerminalEntry = false;

            t2s[0].IsTerminalEntry = true;
            t2s[0].IsTerminalExit = false;

            t2s[1].IsTerminalEntry = false;
            t2s[1].IsTerminalExit = true;


            AddEdge(new UEdge(Vertices.Single(x => x.Id == -1), Vertices.SingleOrDefault(x => x.Name == T1 && x.IsTerminalEntry)));

        }

        public Station GetUithofDepot()
        {
            return Vertices.FirstOrDefault(x => x.Name == "Depot");
        }

        public Station NextStation(Station forStation)
        {
            if (!(GetUithofDepot() == forStation))
            {
                var vertex = Vertices.FirstOrDefault(x => x.Id == forStation.Id);
                var neighbours = Edges.Where(v => v.Source.Id == forStation.Id).FirstOrDefault();
                return neighbours.Target;
            }
            //The only station to move from the depot is T1
            return Vertices.Single(x => x.Name == T1 && x.IsTerminalEntry);
        }

        public Station GetStationTerminal(int v)
        {
            if (v == 0) return Vertices.FirstOrDefault(x => x.Name == T1);
            if (v == 1) return Vertices.FirstOrDefault(x => x.Name == T2);
            throw new Exception($"Use 0 for {T1} or 1 for {T2}");
        }
    }
}
