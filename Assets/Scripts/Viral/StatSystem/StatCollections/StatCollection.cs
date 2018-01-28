using UnityEngine;
using System.Collections.Generic;
using Viral.StatSystem.Interfaces;
using Viral.StatSystem.Database;

namespace Viral.StatSystem
{
    public abstract class StatCollection : MonoBehaviour
    {
        private bool initialized = false;
        private Dictionary<StatType, Stat> _statDictionary;

        #region Constructors
        public StatCollection()
        {
            //empty constructor
        }
        #endregion

        #region Properties - Getters/Setters
        public Dictionary<StatType, Stat> StatDict
        {
            get
            {
                if (_statDictionary == null)
                {
                    //initialize the stat collection if it hasn't already
                    _statDictionary = new Dictionary<StatType, Stat>();
                }
                return _statDictionary;
            }
        }

        //Gets the coresponding stat if it's in the collection
        //read only
        public Stat this[StatType StatType]
        {
            get
            {
                if (ContainsStat(StatType))
                {
                    return StatDict[StatType];
                }
                return null;
            }
        }

        /// <summary>
        /// Returns a List of just regenerating stats
        /// </summary>
        /// <returns></returns>
        public List<StatRegeneratable> RegeneratingStats
        {
            get
            {
                List<StatRegeneratable> re = new List<StatRegeneratable>();

                //INTERFACE CHECKING FOR STAT REGEN ABILITY
                foreach (var i in StatDict.Keys)
                {
                    var stat = this[(StatType)i];
                    IStatRegeneratable s = stat as IStatRegeneratable;
                    if (s != null)
                    {
                        re.Add(stat as StatRegeneratable);
                    }
                }
                return re;
            }
        }

        /// <summary>
        /// Returns list of just vital stats (which includes regenerating stats as well)
        /// </summary>
        public List<StatVital> VitalStats
        {
            get
            {
                List<StatVital> vs = new List<StatVital>();
                foreach (StatType i in StatDict.Keys)
                {
                    var stat = this[i];
                    if ((stat as IStatCurrentValueChanged) != null)
                    {
                        vs.Add(stat as StatVital);
                    }
                }
                return vs;
            }
        }

        /// <summary>
        /// Rerturns list of just Attributes
        /// </summary>
        public List<StatAttribute> AttributeStats
        {
            get
            {
                List<StatAttribute> sa = new List<StatAttribute>();
                foreach (StatType i in StatDict.Keys)
                {
                    var stat = this[i];
                    if ((stat as IStatLinkable) != null && (stat as IStatCurrentValueChanged) == null)
                    {
                        sa.Add(stat as StatAttribute);
                    }
                }
                return sa;
            }
        }


        public virtual float MoveSpeed
        {
            get
            {
                return 1;
            }
        }

        public virtual float AttackSpeed
        {
            get
            {
                return 1;
            }
        }
        #endregion
        /// <summary>
        /// Configured the stats
        /// </summary>
        public void Init()
        {
            //configure stats
           // Debug.Log("[STAT COLLECTION]: Init");
            ConfigureStats();
            initialized = true;
        }

        //regenerating stats
        void FixedUpdate()
        {
            if (initialized)
            {
                RegenStats();
            }
        }

        //Only way to make regen stats work
        void RegenStats()
        {
            if(RegeneratingStats.Count > 0)
            {
                foreach (StatRegeneratable i in RegeneratingStats)
                {
                    if (i.Max != i.Value)
                    {
                        if (Time.time > i.TimeForNextRegen)
                        {
                            i.Regenerate();
                            i.TimeForNextRegen = Time.time + i.SecondsPerPoint;
                        }
                    }
                }
            }
        }

        //Check for stat in collection
        public bool ContainsStat(StatType StatType)
        {
            return StatDict.ContainsKey(StatType);
        }


        //returns a Stat of StatType T
        public T GetStat<T>(StatType StatType) where T : Stat
        {
            //return GetStat(StatType) as T;
            return this[StatType] as T;
        }

        //Creates the <T> Stat and adds it to the collection
        protected T CreateStat<T>(StatType StatType) where T : Stat
        {
            T stat = System.Activator.CreateInstance<T>();
            //Debug.Log("CREATING STAT OF StatType " + StatType + ": " + stat.ToString());
            StatDict.Add(StatType, stat);
            return stat;
        }

        //This method returns a stat and if it's not in the collection it creates it and adds it to the collection
        protected T CreateOrGetStat<T>(StatType StatType) where T : Stat
        {
            T stat = GetStat<T>(StatType);
            if (stat == null)
            {
                stat = CreateStat<T>(StatType);
                //Debug.Log("GOT STAT BACK: " + stat.ToString());
            }
            return stat;
        }

        /// <summary>
        /// Adds a Stat Modifier to the Target stat.
        /// </summary>
        public void AddStatModifier(StatType target, StatModifier mod)
        {
            AddStatModifier(target, mod, false);
        }

        /// <summary>
        /// Adds a Stat Modifier to the Target stat and then updates the stat's value.
        /// </summary>
        public void AddStatModifier(StatType target, StatModifier mod, bool update)
        {
            if (ContainsStat(target))
            {
                var modStat = this[target] as IStatModifiable;
                if (modStat != null)
                {
                    modStat.AddModifier(mod);
                    if (update == true)
                    {
                        modStat.UpdateModifiers();
                    }
                }
                else
                {
                    Debug.Log("[Stats] Trying to add Stat Modifier to non modifiable stat \"" + target.ToString() + "\"");
                }
            }
            else
            {
                Debug.Log("[Stats] Trying to add Stat Modifier to \"" + target.ToString() + "\", but Stats does not contain that stat");
            }
        }

        /// <summary>
        /// Removes a Stat Modifier to the Target stat.
        /// </summary>
        public void RemoveStatModifier(StatType target, StatModifier mod)
        {
            RemoveStatModifier(target, mod, false);
        }

        public void RemoveStatModifier(StatType target, string id, bool update)
        {
            if (ContainsStat(target))
            {
                var modStat = this[target] as IStatModifiable;
                if (modStat != null)
                {
                    modStat.RemoveModifier(id);
                    if (update == true)
                    {
                        Debug.Log("REMOVING STAT MOD");
                        modStat.UpdateModifiers();
                    }
                }
                else
                {
                    Debug.Log("[Stats] Trying to remove Stat Modifier from non modifiable stat \"" + target.ToString() + "\"");
                }
            }
            else
            {
                Debug.Log("[Stats] Trying to remove Stat Modifier from \"" + target.ToString() + "\", but StatCollection does not contain that stat");
            }
        }

        /// <summary>
        /// Removes a Stat Modifier to the Target stat and then updates the stat's value.
        /// </summary>
        public void RemoveStatModifier(StatType target, StatModifier mod, bool update)
        {
            if (ContainsStat(target))
            {
                var modStat = this[target] as IStatModifiable;
                if (modStat != null)
                {
                    modStat.RemoveModifier(mod);
                    if (update == true)
                    {
                        modStat.UpdateModifiers();
                    }
                }
                else
                {
                    Debug.Log("[Stats] Trying to remove Stat Modifier from non modifiable stat \"" + target.ToString() + "\"");
                }
            }
            else
            {
                Debug.Log("[Stats] Trying to remove Stat Modifier from \"" + target.ToString() + "\", but StatCollection does not contain that stat");
            }
        }

        /// <summary>
        /// Clears all stat modifiers from all stats in the collection.
        /// </summary>
        public void ClearStatModifiers()
        {
            ClearStatModifiers(false);
        }

        /// <summary>
        /// Clears all stat modifiers from all stats in the collection then updates all the stat's values.
        /// </summary>
        /// <param name="update"></param>
        public void ClearStatModifiers(bool update)
        {
            foreach (var key in StatDict.Keys)
            {
                ClearStatModifier(key, update);
            }
        }

        /// <summary>
        /// Clears all stat modifiers from the target stat.
        /// </summary>
        public void ClearStatModifier(StatType target)
        {
            ClearStatModifier(target, false);
        }

        /// <summary>
        /// Clears all stat modifiers from the target stat then updates the stat's value.
        /// </summary>
        public void ClearStatModifier(StatType target, bool update)
        {
            if (ContainsStat(target))
            {
                var modStat = this[target] as IStatModifiable;
                if (modStat != null)
                {
                    modStat.ClearModifiers();
                    if (update == true)
                    {
                        modStat.UpdateModifiers();
                    }
                }
                else
                {
                    Debug.Log("[Stats] Trying to clear Stat Modifiers from non modifiable stat \"" + target.ToString() + "\"");
                }
            }
            else
            {
                Debug.Log("[Stats] Trying to clear Stat Modifiers from \"" + target.ToString() + "\", but StatCollection does not contain that stat");
            }
        }

        /// <summary>
        /// Updates all stat modifier's values
        /// </summary>
        public void UpdateStatModifiers()
        {
            foreach (var key in StatDict.Keys)
            {
                UpdateStatModifer(key);
            }
        }

        /// <summary>
        /// Updates the target stat's modifier value
        /// </summary>
        public void UpdateStatModifer(StatType target)
        {
            if (ContainsStat(target))
            {
                var modStat = this[target] as IStatModifiable;
                if (modStat != null)
                {
                    modStat.UpdateModifiers();
                }
                else
                {
                    Debug.Log("[Stats] Trying to Update Stat Modifiers for a non modifiable stat \"" + target.ToString() + "\"");
                }
            }
            else
            {
                Debug.Log("[Stats] Trying to Update Stat Modifiers for \"" + target.ToString() + "\", but StatCollection does not contain that stat");
            }
        }

        /// <summary>
        /// Scales all stats in the collection to the same target level
        /// </summary>
        public void ScaleStatCollection(int level)
        {
            foreach (var key in StatDict.Keys)
            {
                ScaleStat(key, level);
            }
        }

        /// <summary>
        /// Scales the target stat in the collection to the target level
        /// </summary>
        public void ScaleStat(StatType target, int level)
        {
            if (ContainsStat(target))
            {
                var stat = this[target] as IStatScalable;
                if (stat != null)
                {
                    stat.ScaleStat(level);
                }
                else
                {
                    Debug.Log("[RPGStats] Trying to Scale Stat with a non scalable stat \"" + target.ToString() + "\"");
                }
            }
            else
            {
                Debug.Log("[RPGStats] Trying to Scale Stat for \"" + target.ToString() + "\", but RPGStatCollection does not contain that stat");
            }
        }

        public override string ToString()
        {
            string i = string.Empty;
            foreach (var key in StatDict.Keys)
            {
                var stat = this[key];
                i += stat.Name + ": " + stat.Value + "\n";
            }
            return i;
        }

        protected virtual void ConfigureStats()
        {
            //Stats
            CreateOrGetStat<StatVital>(StatType.Health).Name = StatTypeDatabase.GetByType(StatType.Health).Name;
            CreateOrGetStat<StatAttribute>(StatType.Power).Name = StatTypeDatabase.GetByType(StatType.Power).Name;
            CreateOrGetStat<StatAttribute>(StatType.Vampirism).Name = StatTypeDatabase.GetByType(StatType.Vampirism).Name;
            CreateOrGetStat<StatAttribute>(StatType.AbsorbRate).Name = StatTypeDatabase.GetByType(StatType.AbsorbRate).Name;
        }
    }
}