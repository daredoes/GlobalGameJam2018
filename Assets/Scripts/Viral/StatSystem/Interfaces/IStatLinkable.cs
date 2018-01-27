using UnityEngine;
using System.Collections;
namespace Viral.StatSystem.Interfaces
{
    public interface IStatLinkable
    {
        int Linker { get; }

        void AddLinker(StatLinker linker);
        void ClearLinkers();
        void UpdateLinkers();
    }
}
