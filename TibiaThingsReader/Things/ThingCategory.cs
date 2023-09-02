using System;
namespace TibiaThingsReader.Things
{
    public class ThingCategory
    {
        //--------------------------------------------------------------------------
        // CONSTRUCTOR
        //--------------------------------------------------------------------------

        private ThingCategory()
        {
        }

        //--------------------------------------------------------------------------
        // STATIC
        //--------------------------------------------------------------------------

        public const string ITEM = "item";
        public const string OUTFIT = "outfit";
        public const string EFFECT = "effect";
        public const string MISSILE = "missile";

        public static bool IsValid(string category)
        {
            return (category == ITEM || category == OUTFIT || category == EFFECT || category == MISSILE);
        }

        public static string GetCategory(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                //value = StringUtil.ToKeyString(value);
                switch (value)
                {
                    case "item":
                        return ITEM;
                    case "outfit":
                        return OUTFIT;
                    case "effect":
                        return EFFECT;
                    case "missile":
                        return MISSILE;
                }
            }
            return null;
        }

        public static string GetCategoryByValue(uint value)
        {
            switch (value)
            {
                case 1:
                    return ITEM;
                case 2:
                    return OUTFIT;
                case 3:
                    return EFFECT;
                case 4:
                    return MISSILE;
            }
            return null;
        }

        public static uint GetValue(string category)
        {
            if (!string.IsNullOrEmpty(category))
            {
                switch (category)
                {
                    case "item":
                        return 1;
                    case "outfit":
                        return 2;
                    case "effect":
                        return 3;
                    case "missile":
                        return 4;
                }
            }
            return 0;
        }
    }
}
