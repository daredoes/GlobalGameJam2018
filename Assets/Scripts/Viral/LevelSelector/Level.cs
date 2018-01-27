using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viral
{
    public class Level
    {

        public struct Location
        {
            public enum Type
            {
                NULL = -1,
                BRAIN = 0,
                SPINE = 1,
                COLON = 2,
                HEART = 3,
                BICEPTS = 4,
                TRICEPTS = 5,
                PECTORALS = 6,
                ABDOMEN = 7,
                QUADRICEPS = 8,
                CALVES = 9
            }

            public enum Side
            {
                NULL = -1,
                NONE = 0,
                LEFT = 1,
                RIGHT = 2
            }

            public Type type;
            public Side side;

            public Location(Type type, Side side)
            {
                this.type = type;
                this.side = side;
            }
        }

        public enum Type
        {
            NULL = -1,
            MUSCLE = 0,
            ORGAN = 1,
            INTESTINE = 2
        }

        public enum Priority
        {
            NULL = -1,
            LOW = 0,
            MEDIUM = 1,
            HIGH = 2
        }

        public string name;
        public Location location;
        public Side side;
        public Type type;
        public Priority priority;
        public int daysLeft;

    }
}
