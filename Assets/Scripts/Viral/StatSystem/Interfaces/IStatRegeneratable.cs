using UnityEngine;
using System.Collections;
namespace Viral.StatSystem.Interfaces
{
    public interface IStatRegeneratable
    {
        int SecondsPerPoint { get; set; }

        float RegenAmount { get; }

        float TimeForNextRegen { get; set; }

        void Regenerate();
    }
}