using System;
namespace TibiaThingsReader.Things
{
    /// <summary>
    /// The MetadataFlags5 class defines the valid constant values for the client versions 8.60 - 9.86
    /// </summary>
    public static class MetadataFlags5
    {
        public const uint GROUND = 0x00;
        public const uint GROUND_BORDER = 0x01;
        public const uint ON_BOTTOM = 0x02;
        public const uint ON_TOP = 0x03;
        public const uint CONTAINER = 0x04;
        public const uint STACKABLE = 0x05;
        public const uint FORCE_USE = 0x06;
        public const uint MULTI_USE = 0x07;
        public const uint WRITABLE = 0x08;
        public const uint WRITABLE_ONCE = 0x09;
        public const uint FLUID_CONTAINER = 0x0A;
        public const uint FLUID = 0x0B;
        public const uint UNPASSABLE = 0x0C;
        public const uint UNMOVEABLE = 0x0D;
        public const uint BLOCK_MISSILE = 0x0E;
        public const uint BLOCK_PATHFIND = 0x0F;
        public const uint PICKUPABLE = 0x10;
        public const uint HANGABLE = 0x11;
        public const uint VERTICAL = 0x12;
        public const uint HORIZONTAL = 0x13;
        public const uint ROTATABLE = 0x14;
        public const uint HAS_LIGHT = 0x15;
        public const uint DONT_HIDE = 0x16;
        public const uint TRANSLUCENT = 0x17;
        public const uint HAS_OFFSET = 0x18;
        public const uint HAS_ELEVATION = 0x19;
        public const uint LYING_OBJECT = 0x1A;
        public const uint ANIMATE_ALWAYS = 0x1B;
        public const uint MINI_MAP = 0x1C;
        public const uint LENS_HELP = 0x1D;
        public const uint FULL_GROUND = 0x1E;
        public const uint IGNORE_LOOK = 0x1F;
        public const uint CLOTH = 0x20;
        public const uint MARKET_ITEM = 0x21;
        public const uint LAST_FLAG = 0xFF;
        public const string STRING_CHARSET = "iso-8859-1";
    }
}
