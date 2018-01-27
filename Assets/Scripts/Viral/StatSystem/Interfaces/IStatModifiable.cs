﻿using UnityEngine;
using System.Collections;
namespace Viral.StatSystem.Interfaces
{
    public interface IStatModifiable
    {
        int Modifier { get; }      //get the modvalue
        void AddModifier(StatModifier mod);     //add mod to stat
        void RemoveModifier(StatModifier mod); //remove mod from stat
        void RemoveModifier(string id); //remove mod from stat
        void ClearModifiers();  //clear all mods
        void UpdateModifiers(); //update total mod value when added mods change
    }
}
