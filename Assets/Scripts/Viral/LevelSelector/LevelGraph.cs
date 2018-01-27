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

            public Vertex(
                Level.Location.Type endLocationType, 
                Level.Location.Side endLocationSide, 
                float weight)
            {
                this.weight = weight;
                this.endLocation = 
                    new Level.Location(endLocationType, endLocationSide);
            }
        }

        public Dictionary<Level.Location, Node> graph;

        public LevelGraph()
        {
            graph = new Dictionary<Level.Location, Node>();
            foreach (Level.Location.Type location in System.Enum.GetValues(typeof(Level.Location)))
            {
                Node node = new Node();
                List<Vertex> verts = new List<Vertex>();
                Vertex vert = new Vertex();
                switch (location) {
                    case Level.Location.Type.NULL:
                        Debug.Log("ERROR: NULL value.");
                        break;
                    case Level.Location.Type.BRAIN:
                        graph.Add(
                            new Level.Location(location, Level.Location.Side.NONE),
                            AddVertexArrayToNode(
                                new Vertex[] {
                                    new Vertex(
                                        Level.Location.Type.SPINE, 
                                        Level.Location.Side.NONE,
                                        3)
                                }));
                        break;
                    case Level.Location.Type.SPINE:
                        graph.Add(
                            new Level.Location(location, Level.Location.Side.NONE),
                            AddVertexArrayToNode(
                                new Vertex[] {
                                    new Vertex(
                                        Level.Location.Type.BRAIN,
                                        Level.Location.Side.NONE,
                                        3),
                                    new Vertex(
                                        Level.Location.Type.PECTORALS, 
                                        Level.Location.Side.LEFT,
                                        2),
                                    new Vertex(
                                        Level.Location.Type.PECTORALS,
                                        Level.Location.Side.RIGHT, 
                                        2)
                                }));
                        break;
                    case Level.Location.Type.COLON:
                        graph.Add(
                            new Level.Location(location, Level.Location.Side.NONE),
                            AddVertexArrayToNode(
                                new Vertex[] {
                                    new Vertex(
                                        Level.Location.Type.HEART,
                                        Level.Location.Side.NONE,
                                        3),
                                    new Vertex(
                                        Level.Location.Type.ABDOMEN,
                                        Level.Location.Side.NONE,
                                        2)
                                }));
                        break;
                    case Level.Location.Type.HEART:
                        graph.Add(
                            new Level.Location(location, Level.Location.Side.NONE),
                            AddVertexArrayToNode(
                                new Vertex[] {
                                    new Vertex(
                                        Level.Location.Type.COLON,
                                        Level.Location.Side.NONE,
                                        2),
                                    new Vertex(
                                        Level.Location.Type.PECTORALS,
                                        Level.Location.Side.LEFT,
                                        1),
                                    new Vertex(
                                        Level.Location.Type.PECTORALS,
                                        Level.Location.Side.RIGHT,
                                        1)
                                }));
                        break;
                    case Level.Location.Type.BICEPTS:
                        foreach (Level.Location.Side side in System.Enum.GetValues(typeof(Level.Location.Side)))
                        {
                            switch (side) {
                                case Level.Location.Side.NULL:
                                    break;
                                case Level.Location.Side.NONE:
                                    break;
                                case Level.Location.Side.LEFT:

                                case Level.Location.Side.RIGHT:
                                    graph.Add(
                                    new Level.Location(location, side),
                                    AddVertexArrayToNode(
                                        new Vertex[] {
                                            new Vertex(
                                                Level.Location.Type.PECTORALS,
                                                side,
                                                2),
                                            new Vertex(
                                                Level.Location.Type.TRICEPTS,
                                                side,
                                                1)
                                        }));
                                    break;
                            }
                        }
                        break;
                    case Level.Location.Type.TRICEPTS:
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

        private Node AddVertexArrayToNode(Vertex[] vertex)
        {
            Node node = new Node();
            List<Vertex> vertices = new List<Vertex>();

            for (int i = 0; i < vertex.Length; ++i)
            {
                vertices.Add(vertex[i]);
            }
            node.vertices = vertices;

            return node;
        }
    }
}
