namespace Viral.StatSystem
{
    public class OzStatCollection : StatCollection
    {
        public override int MaxInventoryCapasity
        {
            get
            {
                return ((StatAttribute)this[StatType.Strength]).Value + ((StatAttribute)this[StatType.Willpower]).Value;
            }
        }

        public override float DefenseProtection
        {
            get
            {
                return (0.01f * ((StatAttribute)this[StatType.Constitution]).Value);
            }
        }

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
            ((StatAttribute)this[StatType.Strength]).Base = 3;
            ((StatAttribute)this[StatType.Wisdom]).Base = 2;
            ((StatAttribute)this[StatType.Willpower]).Base = 4;
            ((StatAttribute)this[StatType.Dexterity]).Base = 3;
            ((StatAttribute)this[StatType.Intellect]).Base = 2;
            ((StatAttribute)this[StatType.Constitution]).Base = 3;
            ((StatAttribute)this[StatType.Fortitude]).Base = 1;
            ((StatAttribute)this[StatType.Agility]).Base = 4;
            #endregion

            #region STATS            
            var health = ((StatRegeneratable)this[StatType.Health]);
            health.Base = 125;
            health.BaseSecondsPerPoint = 100;
            //Strength = 10; and our ratio is 10; 10*10 = 0; health = 100; health + Stamina = 200
            health.AddLinker(new BasicStatLinker(((StatAttribute)this[StatType.Strength]), 1f, true)); // health should be 200
            //Willpower = 8; and our ratio is 1; 1*8= 8; health.SecondsPerPoint = 1; health.SecondsPerPoint - Endurance = 8
            health.AddLinker(new BasicStatLinker(((StatAttribute)this[StatType.Willpower]), 1f));
            health.UpdateLinkers();
            health.RestoreCurrentValueToMax();
            
            var magic = ((StatRegeneratable)this[StatType.Magic]);
            magic.Base = 50;
            magic.BaseSecondsPerPoint = 100;
            //Strength = 10; and our ratio is 10; 10*10 = 0; health = 100; health + Stamina = 200
            magic.AddLinker(new BasicStatLinker(((StatAttribute)this[StatType.Wisdom]), 1f)); // health should be 200
            //Willpower = 8; and our ratio is 1; 1*8= 8; health.SecondsPerPoint = 1; health.SecondsPerPoint - Endurance = 8
            magic.AddLinker(new BasicStatLinker(((StatAttribute)this[StatType.Intellect]), 1f, true));
            magic.UpdateLinkers();
            magic.RestoreCurrentValueToMax();

            //var defense = ((StatVital)this[StatType.Defense]);
            var defense = ((StatAttribute)this[StatType.Defense]);
            defense.Base = 8;
            defense.AddLinker(new BasicStatLinker(((StatAttribute)this[StatType.Fortitude]), 1f));
            defense.UpdateLinkers();
            //defense.RestoreCurrentValueToMax();

            //var power = ((StatVital)this[StatType.Power]);
            var power = ((StatAttribute)this[StatType.Power]);
            power.Base = 10;
            power.AddLinker(new BasicStatLinker(((StatAttribute)this[StatType.Strength]), 1f));
            power.UpdateLinkers();
            //power.RestoreCurrentValueToMax();
            #endregion
        }
    }
}