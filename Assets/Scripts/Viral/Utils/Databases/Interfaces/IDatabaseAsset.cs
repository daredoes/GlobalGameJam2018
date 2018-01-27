using UnityEngine;
using System.Collections;

namespace Viral.Utils.Database.Interfaces
{
    public interface IDatabaseAsset
    {
        int ID { get; set; }
        string Name { get; set; }
    }
}