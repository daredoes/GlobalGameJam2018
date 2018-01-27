using UnityEngine;
using System;
namespace Viral.StatSystem.Interfaces
{
    public interface IStatValueChanged
    {
        event EventHandler OnValueChanged;  //for any stat that needs to update other components when its value changes via an event trigger
    }
}
