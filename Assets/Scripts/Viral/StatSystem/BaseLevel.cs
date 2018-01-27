using UnityEngine;
using Viral.LevelSystem;

namespace Viral.StatSystem
{
    public class Level : BaseLevel
    {
        public override int GetExpRequiredForLevel(int level)
        {
            return (int)(Mathf.Pow(Level, 2f) * 100) + 100;
        }
    }
}