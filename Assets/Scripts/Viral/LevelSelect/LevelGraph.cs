using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viral.LevelSelect
{
    public class LevelGraph
    {
        public struct Node
        {
            public List<Vertex> vertices;
        }

        public struct Vertex
        {
            public int weight; // 3 for high priority, 2 for medium, 1 for low
            public Level.Location endLocation;

            public Vertex(
                Level.Location.Type endLocationType, 
                Level.Location.Side endLocationSide, 
                int weight)
            {
                this.weight = weight;
                this.endLocation = 
                    new Level.Location(endLocationType, endLocationSide);
            }
        }

        public Dictionary<Level.Location, Node> graph;

        public LevelGraph()
        {
            graph = new Dictionary<Level.Location, Node>(); // used for access
            foreach (Level.Location.Type location in System.Enum.GetValues(typeof(Level.Location.Type)))
            {
                switch (location) {
                    case Level.Location.Type.NULL:
                        //Debug.Log("ERROR: NULL value.");
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
                        foreach (Level.Location.Side side in System.Enum.GetValues(typeof(Level.Location.Side)))
                        {
                            switch (side)
                            {
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
                                                    Level.Location.Type.BICEPTS,
                                                    side,
                                                    1),
                                                new Vertex(
                                                    Level.Location.Type.TRICEPTS,
                                                    side == Level.Location.Side.LEFT ? Level.Location.Side.RIGHT : Level.Location.Side.LEFT,
                                                    1)
                                            }));
                                    break;
                            }
                        }
                        break;
                    case Level.Location.Type.PECTORALS:
                        foreach (Level.Location.Side side in System.Enum.GetValues(typeof(Level.Location.Side)))
                        {
                            switch (side)
                            {
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
                                                Level.Location.Type.SPINE,
                                                Level.Location.Side.NONE,
                                                3),
                                            new Vertex(
                                                Level.Location.Type.HEART,
                                                Level.Location.Side.NONE,
                                                3),
                                            new Vertex(
                                                Level.Location.Type.PECTORALS,
                                                side == Level.Location.Side.LEFT ? Level.Location.Side.RIGHT : Level.Location.Side.LEFT,
                                                2),
                                            new Vertex(
                                                Level.Location.Type.BICEPTS,
                                                side,
                                                1)

                                        }));
                                    break;
                            }
                        }
                        break;
                    case Level.Location.Type.ABDOMEN:
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
                                        2),
                                    new Vertex(
                                        Level.Location.Type.PECTORALS,
                                        Level.Location.Side.RIGHT, 
                                        2),
                                    new Vertex(
                                        Level.Location.Type.QUADRICEPS,
                                        Level.Location.Side.LEFT,
                                        1),
                                    new Vertex(
                                        Level.Location.Type.QUADRICEPS,
                                        Level.Location.Side.RIGHT,
                                        1)
                                }));
                        break;
                    case Level.Location.Type.QUADRICEPS:
                        foreach (Level.Location.Side side in System.Enum.GetValues(typeof(Level.Location.Side)))
                        {
                            switch (side)
                            {
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
                                                    Level.Location.Type.ABDOMEN, 
                                                    Level.Location.Side.NONE, 
                                                    2),
                                                new Vertex(
                                                    Level.Location.Type.CALVES,
                                                    side,
                                                    1),
                                            }));
                                    break;
                            }
                        }
                        break;
                    case Level.Location.Type.CALVES:
                        foreach (Level.Location.Side side in System.Enum.GetValues(typeof(Level.Location.Side)))
                        {
                            switch (side)
                            {
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
                                                    Level.Location.Type.QUADRICEPS,
                                                    side,
                                                    1),
                                                new Vertex(
                                                    Level.Location.Type.CALVES,
                                                    side == Level.Location.Side.LEFT ? Level.Location.Side.RIGHT : Level.Location.Side.LEFT,
                                                    1)
                                            }));
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        public LevelGraph(Dictionary<Level.Location, Node> dictionary)
        {
            graph = dictionary;
        }

        public Level.Location GetNextLocationRandomly(Level.Location current)
        {
            int sum = 0;
            List<Vertex> randomizer = new List<Vertex>();

            foreach (Vertex v in graph[current].vertices)
            {
                sum += v.weight;
                for (int i = 0; i < v.weight; ++i)
                    randomizer.Add(v);
            }

            return randomizer[Random.Range(0, sum)].endLocation;
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
