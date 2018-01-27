﻿namespace Viral.Utils.Database
{
    public class BaseDatabase<T> : AbstractDatabase<T> where T : BaseDatabaseAsset
    {
        protected override void OnAddObject(T t)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        protected override void OnRemoveObject(T t)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}