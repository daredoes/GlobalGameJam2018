namespace Viral.StatSystem
{
    //example Linker
    //Basic StatLinker
    public class BasicStatLinker : StatLinker
    {
        private float _ratio;

        #region Constructors
        public BasicStatLinker() : base()
        {
            //empty constructor
            _ratio = 0;
        }
        public BasicStatLinker(Stat stat, float ratio) : base(stat)
        {
            _ratio = ratio;
        }

        public BasicStatLinker(Stat stat, float ratio, bool secondary) : base(stat, secondary)
        {
            _ratio = ratio;
        }
        #endregion

        public float Ratio
        {
            get { return _ratio; }
        }

        public override int Value
        {
            get
            {
                //The attributes value * the given ratio which is later added to the linked stat's base value
                return (int)(StatThatsLinking.Value * _ratio);
            }
        }


    }
}