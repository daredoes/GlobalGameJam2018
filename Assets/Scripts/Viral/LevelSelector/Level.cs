using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viral
{
    public class Level
    {

        public enum Location
        {
            NULL = -1,
            BRAIN = 0,
            SPINE = 1,
            COLON = 2,
            BICEPTS = 3,
            TRICEPTS = 4,
            PECTORALS = 5,
            ABDOMEN = 6,
            QUADRICEPS = 7,
            CALVES = 8
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
        public Type type;
        public Priority priority;
        public int daysLeft;

    }
}
