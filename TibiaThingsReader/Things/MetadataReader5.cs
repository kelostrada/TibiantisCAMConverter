using System;
using System.IO;

namespace TibiaThingsReader.Things
{
    /// <summary>
    /// Reader for versions 8.60 - 9.86
    /// </summary>
    public class MetadataReader5 : MetadataReader
    {
        public MetadataReader5(Stream stream) : base(stream)
        {

        }

        public override bool ReadProperties(ThingType type)
        {
            uint flag = 0;

            while (flag < MetadataFlags5.LAST_FLAG)
            {
                uint previusFlag = flag;
                flag = ReadByte();

                if (flag == MetadataFlags5.LAST_FLAG)
                    return true;

                switch (flag)
                {
                    case MetadataFlags5.GROUND:
                        type.IsGround = true;
                        type.GroundSpeed = ReadUInt16();
                        break;

                    case MetadataFlags5.GROUND_BORDER:
                        type.IsGroundBorder = true;
                        break;

                    case MetadataFlags5.ON_BOTTOM:
                        type.IsOnBottom = true;
                        break;

                    case MetadataFlags5.ON_TOP:
                        type.IsOnTop = true;
                        break;

                    case MetadataFlags5.CONTAINER:
                        type.IsContainer = true;
                        break;

                    case MetadataFlags5.STACKABLE:
                        type.Stackable = true;
                        break;

                    case MetadataFlags5.FORCE_USE:
                        type.ForceUse = true;
                        break;

                    case MetadataFlags5.MULTI_USE:
                        type.MultiUse = true;
                        break;

                    case MetadataFlags5.WRITABLE:
                        type.Writable = true;
                        type.MaxTextLength = ReadUInt16();
                        break;

                    case MetadataFlags5.WRITABLE_ONCE:
                        type.WritableOnce = true;
                        type.MaxTextLength = ReadUInt16();
                        break;

                    case MetadataFlags5.FLUID_CONTAINER:
                        type.IsFluidContainer = true;
                        break;

                    case MetadataFlags5.FLUID:
                        type.IsFluid = true;
                        break;

                    case MetadataFlags5.UNPASSABLE:
                        type.IsUnpassable = true;
                        break;

                    case MetadataFlags5.UNMOVEABLE:
                        type.IsUnmoveable = true;
                        break;

                    case MetadataFlags5.BLOCK_MISSILE:
                        type.BlockMissile = true;
                        break;

                    case MetadataFlags5.BLOCK_PATHFIND:
                        type.BlockPathfind = true;
                        break;

                    case MetadataFlags5.PICKUPABLE:
                        type.Pickupable = true;
                        break;

                    case MetadataFlags5.HANGABLE:
                        type.Hangable = true;
                        break;

                    case MetadataFlags5.VERTICAL:
                        type.IsVertical = true;
                        break;

                    case MetadataFlags5.HORIZONTAL:
                        type.IsHorizontal = true;
                        break;

                    case MetadataFlags5.ROTATABLE:
                        type.Rotatable = true;
                        break;

                    case MetadataFlags5.HAS_LIGHT:
                        type.HasLight = true;
                        type.LightLevel = ReadUInt16();
                        type.LightColor = ReadUInt16();
                        break;

                    case MetadataFlags5.DONT_HIDE:
                        type.DontHide = true;
                        break;

                    case MetadataFlags5.TRANSLUCENT:
                        type.IsTranslucent = true;
                        break;

                    case MetadataFlags5.HAS_OFFSET:
                        type.HasOffset = true;
                        type.OffsetX = ReadUInt16();
                        type.OffsetY = ReadUInt16();
                        break;

                    case MetadataFlags5.HAS_ELEVATION:
                        type.HasElevation = true;
                        type.Elevation = ReadUInt16();
                        break;

                    case MetadataFlags5.LYING_OBJECT:
                        type.IsLyingObject = true;
                        break;

                    case MetadataFlags5.ANIMATE_ALWAYS:
                        type.AnimateAlways = true;
                        break;

                    case MetadataFlags5.MINI_MAP:
                        type.MiniMap = true;
                        type.MiniMapColor = ReadUInt16();
                        break;

                    case MetadataFlags5.LENS_HELP:
                        type.IsLensHelp = true;
                        type.LensHelp = ReadUInt16();
                        break;

                    case MetadataFlags5.FULL_GROUND:
                        type.IsFullGround = true;
                        break;

                    case MetadataFlags5.IGNORE_LOOK:
                        type.IgnoreLook = true;
                        break;

                    case MetadataFlags5.CLOTH:
                        type.Cloth = true;
                        type.ClothSlot = ReadUInt16();
                        break;

                    case MetadataFlags5.MARKET_ITEM:
                        type.IsMarketItem = true;
                        type.MarketCategory = ReadUInt16();
                        type.MarketTradeAs = ReadUInt16();
                        type.MarketShowAs = ReadUInt16();
                        ushort nameLength = ReadUInt16();
                        // TODO - use encoding Encoding.GetEncoding(MetadataFlags5.STRING_CHARSET)
                        type.MarketName = new string(ReadChars(nameLength));
                        type.MarketRestrictProfession = ReadUInt16();
                        type.MarketRestrictLevel = ReadUInt16();
                        break;

                    default:
                        throw new Exception("Unknown flag: " + flag.ToString("X2") + " (previous: " + previusFlag.ToString("X2") + ", category: " + type.Category + ", id: " + type.Id + ")");
                }
            }

            return true;
        }
    }
}
