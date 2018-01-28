using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Viral
{
    public class Level
    {
        [Serializable]
        public struct Location
        {
            public enum Type
            {
                NULL = -1,
                BRAIN = 0,
                SPINE = 1,
                HEART = 2,
                COLON = 3,
                PECTORALS = 4,
                ABDOMEN = 5,
                BICEPTS = 6,
                TRICEPTS = 7,
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

            public Location(int i = 0)
            {
                type = Type.NULL;
                side = Side.NULL;
            }
        }

        public enum Type
        {
            NULL = -1,
            MUSCLE = 0,
            ORGAN = 1,
            INTESTINE = 2,
            BONE = 3
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
        public bool isActive;
        public int daysLeft;

        public Level(Location? location = null, Type? type = null, 
            Priority? priority = null, string name = null)
        {
            if (location == null)
                this.location = new Location();
            else
                this.location = (Location)location;

            if (type == null)
                this.type = Type.NULL;
            else
                this.type = (Type)type;

            if (priority == null)
                this.priority = Priority.NULL;
            else
                this.priority = (Priority)priority;

            if (name == null)
            {
                string addon = "";
                if (this.location.side > Location.Side.NONE)
                    addon = this.location.side.ToString().ToLower() + " ";
                this.name = addon + this.location.type.ToString().ToLower();
            }
            else
                this.name = name;
        }

        public override string ToString()
        {
            return base.ToString()+": "+name;
        }
    }
}
