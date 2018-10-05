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
                    var isTerminal = name == T1 || name == T2;

                    if (isTerminal)
                    {
                        //Note upper case!!
                        if (Vertices.Any(vertex => vertex.Name == name)) return;
                    }

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
                {
                    AddVertex(item);
                }

                // Add the edge weights
                for (var j = 0; j < vertices.Count - 1; j++)
                {
                    var weight = -1;

                    if (x == 0) { weight = forward[j]; }
                    else { weight = backwards[j]; }
                    var edge = new UEdge(vertices[j], vertices[j + 1]) { Weight = weight };
                    AddEdge(edge);
                }
            }



            //add edges to final stations
            AddEdge(new UEdge(Vertices.FirstOrDefault(x => x.Id == 8), Vertices.FirstOrDefault(x => x.Id == 9)));
            AddEdge(new UEdge(Vertices.FirstOrDefault(x => x.Id == 15), Vertices.FirstOrDefault(x => x.Id == 0)));

            foreach (var vertex in Vertices)
            {
                vertex.OutEdges = Edges.Where(x => x.Source == vertex).ToList();
                vertex.InEdges = Edges.Where(x => x.Target == vertex).ToList();
            }

            AddVertex(new Station()
            {
                Name = "Depot",
                Id = -1,
            });

            //Set path from depot to station 1
            AddEdge(new UEdge(Vertices.Single(x => x.Id == -1), Vertices.SingleOrDefault(x => x.Name == T1))
            {
                Weight = 1
            });

        }

        /// <summary>
        /// TODO: Determine the arriving passengers for time T
        /// </summary>
        /// <param name="t"></param>
        public void PassengersArrive(int t)
        {
            foreach (var station in Vertices)
                station.WaitingPeople += 0.05;
        }

        public Station GetCSDepot()
        {
            return Vertices.FirstOrDefault(x => x.Name == "Depot");
        }

        public Station NextStation(Station forStation)
        {
            if (!(GetCSDepot() == forStation))
            {
                var vertex = Vertices.FirstOrDefault(x => x.Id == forStation.Id);
                var neighbours = Edges.Where(v => v.Source.Id == forStation.Id).FirstOrDefault();
                return neighbours.Target;
            }
            //The only station to move from the depot is T1
            return Vertices.Single(x => x.Name == T1);
        }

        public Station GetStationTerminal(int v)
        {
            if (v == 0) return Vertices.FirstOrDefault(x => x.Name == T1);
            if (v == 1) return Vertices.FirstOrDefault(x => x.Name == T2);
            throw new Exception($"Use 0 for {T1} or 1 for {T2}");
        }
    }
}
