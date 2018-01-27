namespace Viral.StatSystem
{
    public class OzStatCollection : StatCollection
    {
        public override float MoveSpeed
        {
            get
            {
                return 1;
            }
        }

        public override float AttackSpeed
        {
            get
            {
                return base.AttackSpeed;
            }
        }

        protected override void ConfigureStats()
        {
            base.ConfigureStats();

            #region ATTRIBUTES
            ((StatAttribute)this[StatType.AbsorbRate]).Base = 2;
            ((StatAttribute)this[StatType.Vampirsm]).Base = 4;
            #endregion

            #region STATS            
            var health = ((StatVital)this[StatType.Health]);
            health.Base = 100;
            //health.BaseSecondsPerPoint = 100;
            //Strength = 10; and our ratio is 10; 10*10 = 0; health = 100; health + Stamina = 200
            //health.AddLinker(new BasicStatLinker(((StatAttribute)this[StatType.Strength]), 1f, true)); // health should be 200
            //Willpower = 8; and our ratio is 1; 1*8= 8; health.SecondsPerPoint = 1; health.SecondsPerPoint - Endurance = 8
            //health.AddLinker(new BasicStatLinker(((StatAttribute)this[StatType.Willpower]), 1f));
            health.UpdateLinkers();
            health.RestoreCurrentValueToMax();

            //var power = ((StatVital)this[StatType.Power]);
            var power = ((StatAttribute)this[StatType.Power]);
            power.Base = 10;
            //power.AddLinker(new BasicStatLinker(((StatAttribute)this[StatType.Strength]), 1f));
            power.UpdateLinkers();
            //power.RestoreCurrentValueToMax();
            #endregion
        }
    }
}