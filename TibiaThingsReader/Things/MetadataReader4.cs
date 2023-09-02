using System;
using System.IO;

namespace TibiaThingsReader.Things
{
    /// <summary>
    /// Reader for versions 7.80 - 8.54
    /// </summary>
    public class MetadataReader4 : MetadataReader
    {
        public MetadataReader4(Stream stream) : base(stream)
        {

        }

        public override bool ReadProperties(ThingType type)
        {
            uint flag = 0;
            while (flag < MetadataFlags4.LAST_FLAG)
            {
                uint previusFlag = flag;
                flag = ReadByte();

                if (flag == MetadataFlags4.LAST_FLAG)
                    return true;

                switch (flag)
                {
                    case MetadataFlags4.GROUND:
                        type.IsGround = true;
                        type.GroundSpeed = ReadUInt16();
                        break;

                    case MetadataFlags4.GROUND_BORDER:
                        type.IsGroundBorder = true;
                        break;

                    case MetadataFlags4.ON_BOTTOM:
                        type.IsOnBottom = true;
                        break;

                    case MetadataFlags4.ON_TOP:
                        type.IsOnTop = true;
                        break;

                    case MetadataFlags4.CONTAINER:
                        type.IsContainer = true;
                        break;

                    case MetadataFlags4.STACKABLE:
                        type.Stackable = true;
                        break;

                    case MetadataFlags4.FORCE_USE:
                        type.ForceUse = true;
                        break;

                    case MetadataFlags4.MULTI_USE:
                        type.MultiUse = true;
                        break;

                    case MetadataFlags4.HAS_CHARGES:
                        type.HasCharges = true;
                        break;

                    case MetadataFlags4.WRITABLE:
                        type.Writable = true;
                        type.MaxTextLength = ReadUInt16();
                        break;

                    case MetadataFlags4.WRITABLE_ONCE:
                        type.WritableOnce = true;
                        type.MaxTextLength = ReadUInt16();
                        break;

                    case MetadataFlags4.FLUID_CONTAINER:
                        type.IsFluidContainer = true;
                        break;

                    case MetadataFlags4.FLUID:
                        type.IsFluid = true;
                        break;

                    case MetadataFlags4.UNPASSABLE:
                        type.IsUnpassable = true;
                        break;

                    case MetadataFlags4.UNMOVEABLE:
                        type.IsUnmoveable = true;
                        break;

                    case MetadataFlags4.BLOCK_MISSILE:
                        type.BlockMissile = true;
                        break;

                    case MetadataFlags4.BLOCK_PATHFIND:
                        type.BlockPathfind = true;
                        break;

                    case MetadataFlags4.PICKUPABLE:
                        type.Pickupable = true;
                        break;

                    case MetadataFlags4.HANGABLE:
                        type.Hangable = true;
                        break;

                    case MetadataFlags4.VERTICAL:
                        type.IsVertical = true;
                        break;

                    case MetadataFlags4.HORIZONTAL:
                        type.IsHorizontal = true;
                        break;

                    case MetadataFlags4.ROTATABLE:
                        type.Rotatable = true;
                        break;

                    case MetadataFlags4.HAS_LIGHT:
                        type.HasLight = true;
                        type.LightLevel = ReadUInt16();
                        type.LightColor = ReadUInt16();
                        break;

                    case MetadataFlags4.DONT_HIDE:
                        type.DontHide = true;
                        break;

                    case MetadataFlags4.FLOOR_CHANGE:
                        type.FloorChange = true;
                        break;

                    case MetadataFlags4.HAS_OFFSET:
                        type.HasOffset = true;
                        type.OffsetX = ReadUInt16();
                        type.OffsetY = ReadUInt16();
                        break;

                    case MetadataFlags4.HAS_ELEVATION:
                        type.HasElevation = true;
                        type.Elevation = ReadUInt16();
                        break;

                    case MetadataFlags4.LYING_OBJECT:
                        type.IsLyingObject = true;
                        break;

                    case MetadataFlags4.ANIMATE_ALWAYS:
                        type.AnimateAlways = true;
                        break;

                    case MetadataFlags4.MINI_MAP:
                        type.MiniMap = true;
                        type.MiniMapColor = ReadUInt16();
                        break;

                    case MetadataFlags4.LENS_HELP:
                        type.IsLensHelp = true;
                        type.LensHelp = ReadUInt16();
                        break;

                    case MetadataFlags4.FULL_GROUND:
                        type.IsFullGround = true;
                        break;

                    case MetadataFlags4.IGNORE_LOOK:
                        type.IgnoreLook = true;
                        break;

                    default:
                        throw new Exception("Unknown flag: " + flag.ToString("X2") + " (previous: " + previusFlag.ToString("X2") + ", category: " + type.Category + ", id: " + type.Id + ")");
                }
            }

            return true;
        }
    }
}
