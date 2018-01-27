using UnityEngine;
using System;
namespace Viral.StatSystem.Interfaces
{
    public interface IStatCurrentValueChanged
    {
        event EventHandler OnCurrentValueChanged;
    }
}
