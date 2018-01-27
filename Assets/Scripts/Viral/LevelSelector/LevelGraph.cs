using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viral
{
    public class LevelGraph
    {
        public struct Node
        {
            public List<Vertex> vertices;
        }

        public struct Vertex
        {
            public float weight; // 3 for high priority, 2 for medium, 1 for low
            public Level.Location endLocation;
        }

        public Dictionary<Level.Location, Node> graph;

        public LevelGraph()
        {
            graph = new Dictionary<Level.Location, Node>();
            foreach (Level.Location location in System.Enum.GetValues(typeof(Level.Location)))
            {
                Node node = new Node();
                List<Vertex> verts = new List<Vertex>();
                Vertex vert = new Vertex();
                switch (location) {
                    case Level.Location.NULL:
                        Debug.Log("ERROR: NULL value.");
                        break;
                    case Level.Location.BRAIN:
                        vert.endLocation = Level.Location.SPINE;
                        vert.weight = 3;
                        verts.Add(vert);
                        node.vertices = verts;
                        graph.Add(location, node);
                        break;
                    case Level.Location.SPINE:
                        vert.endLocation = Level.Location.BRAIN;
                        vert.weight = 3;
                        verts.Add(vert);
                        vert.endLocation = Level.Location.PECTORALS;
                        vert.weight = 2;
                        verts.Add(vert);
                        vert.endLocation = Level.Location.BICEPTS;
                        vert.weight = 1;
                        verts.Add(vert);
                        break;
                    case Level.Location.COLON:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                }
            }
        }

        public LevelGraph(Dictionary<Level.Location, Node> dictionary)
        {
            graph = dictionary;
        }
    }
}
