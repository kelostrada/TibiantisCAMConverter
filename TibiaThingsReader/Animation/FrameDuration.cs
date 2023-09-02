using System;
using TibiaThingsReader.Things;

namespace TibiaThingsReader.Animation
{
    public class FrameDuration
    {
        //--------------------------------------------------------------------------
        // PROPERTIES
        //--------------------------------------------------------------------------

        public uint Minimum { get; set; }
        public uint Maximum { get; set; }

        //--------------------------------------
        // Getters / Setters
        //--------------------------------------

        public uint Duration
        {
            get
            {
                if (Minimum == Maximum)
                {
                    return Minimum;
                }

                var random = new Random();
                return (uint)(Minimum + random.Next((int)(Maximum - Minimum + 1)));
            }
        }

        //--------------------------------------------------------------------------
        // CONSTRUCTOR
        //--------------------------------------------------------------------------

        public FrameDuration(uint minimum = 0, uint maximum = 0)
        {
            if (minimum > maximum)
            {
                throw new ArgumentException("The minimum value may not be greater than the maximum value.");
            }

            Minimum = minimum;
            Maximum = maximum;
        }

        //--------------------------------------------------------------------------
        // METHODS
        //--------------------------------------------------------------------------

        //--------------------------------------
        // Public
        //--------------------------------------

        public override string ToString()
        {
            return "[FrameDuration minimum=" + Minimum + ", maximum=" + Maximum + "]";
        }

        public bool Equals(FrameDuration frameDuration)
        {
            return (Minimum == frameDuration.Minimum && Maximum == frameDuration.Maximum);
        }

        public FrameDuration Clone()
        {
            return new FrameDuration(Minimum, Maximum);
        }

        //--------------------------------------------------------------------------
        // STATIC
        //--------------------------------------------------------------------------

        public static uint GetDefaultDuration(string category)
        {
            switch (category)
            {
                case ThingCategory.ITEM:
                    return 500;

                case ThingCategory.OUTFIT:
                    return 300;

                case ThingCategory.EFFECT:
                    return 100;
            }

            return 0;
        }
    }
}
