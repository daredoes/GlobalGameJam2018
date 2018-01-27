﻿using Viral.Utils.Database;

namespace Viral.StatSystem.Database
{
    public class StatTypeDatabase : BaseDatabase<StatTypeAsset>
    {
        const string DatabasePath = @"Resources/Systems/StatSystem/Database/";
        const string DatabaseName = @"StatTypeDatabase.asset";

        private static StatTypeDatabase _instance = null;

        public static StatTypeDatabase Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = GetDatabase<StatTypeDatabase>(DatabasePath, DatabaseName);
                }
                return _instance;
            }

            set
            {
                _instance = value;
            }
        }

        static public StatTypeAsset GetAt(int index)
        {
            return Instance.GetAtIndex(index);
        }

        static public StatTypeAsset GetAsset(int id)
        {
            return Instance.GetByID(id);
        }

        static public StatTypeAsset GetByType(StatType sT)
        {
            return Instance.GetByName(sT.ToString());
        }

        static public int GetAssetCount()
        {
            return Instance.Count;
        }
    }
}