using System;
using System.IO;

namespace TibiaThingsReader.Things
{
    /// <summary>
    /// Reader for versions 7.55 - 7.72
    /// </summary>
    public class MetadataReader3 : MetadataReader
    {
        public MetadataReader3(Stream stream) : base(stream)
        {

        }

        public override bool ReadProperties(ThingType type)
        {
            uint flag = 0;
            while (flag < MetadataFlags3.LAST_FLAG)
            {
                uint previusFlag = flag;
                flag = ReadByte();

                if (flag == MetadataFlags3.LAST_FLAG)
                    return true;

                switch (flag)
                {
                    case MetadataFlags3.GROUND:
                        type.IsGround = true;
                        type.GroundSpeed = ReadUInt16();
                        break;

                    case MetadataFlags3.GROUND_BORDER:
                        type.IsGroundBorder = true;
                        break;

                    case MetadataFlags3.ON_BOTTOM:
                        type.IsOnBottom = true;
                        break;

                    case MetadataFlags3.ON_TOP:
                        type.IsOnTop = true;
                        break;

                    case MetadataFlags3.CONTAINER:
                        type.IsContainer = true;
                        break;

                    case MetadataFlags3.STACKABLE:
                        type.Stackable = true;
                        break;

                    case MetadataFlags3.MULTI_USE:
                        type.MultiUse = true;
                        break;

                    case MetadataFlags3.FORCE_USE:
                        type.ForceUse = true;
                        break;

                    case MetadataFlags3.WRITABLE:
                        type.Writable = true;
                        type.MaxTextLength = ReadUInt16();
                        break;

                    case MetadataFlags3.WRITABLE_ONCE:
                        type.WritableOnce = true;
                        type.MaxTextLength = ReadUInt16();
                        break;

                    case MetadataFlags3.FLUID_CONTAINER:
                        type.IsFluidContainer = true;
                        break;
                    case MetadataFlags3.FLUID:
                        type.IsFluid = true;
                        break;

                    case MetadataFlags3.UNPASSABLE:
                        type.IsUnpassable = true;
                        break;

                    case MetadataFlags3.UNMOVEABLE:
                        type.IsUnmoveable = true;
                        break;

                    case MetadataFlags3.BLOCK_MISSILE:
                        type.BlockMissile = true;
                        break;

                    case MetadataFlags3.BLOCK_PATHFINDER:
                        type.BlockPathfind = true;
                        break;

                    case MetadataFlags3.PICKUPABLE:
                        type.Pickupable = true;
                        break;

                    case MetadataFlags3.HANGABLE:
                        type.Hangable = true;
                        break;

                    case MetadataFlags3.VERTICAL:
                        type.IsVertical = true;
                        break;
                    case MetadataFlags3.HORIZONTAL:
                        type.IsHorizontal = true;
                        break;

                    case MetadataFlags3.ROTATABLE:
                        type.Rotatable = true;
                        break;

                    case MetadataFlags3.HAS_LIGHT:
                        type.HasLight = true;
                        type.LightLevel = ReadUInt16();
                        type.LightColor = ReadUInt16();
                        break;

                    case MetadataFlags3.FLOOR_CHANGE:
                        type.FloorChange = true;
                        break;

                    case MetadataFlags3.HAS_OFFSET:
                        type.HasOffset = true;
                        type.OffsetX = ReadUInt16();
                        type.OffsetY = ReadUInt16();
                        break;

                    case MetadataFlags3.HAS_ELEVATION:
                        type.HasElevation = true;
                        type.Elevation = ReadUInt16();
                        break;

                    case MetadataFlags3.LYING_OBJECT:
                        type.IsLyingObject = true;
                        break;

                    case MetadataFlags3.ANIMATE_ALWAYS:
                        type.AnimateAlways = true;
                        break;

                    case MetadataFlags3.MINI_MAP:
                        type.MiniMap = true;
                        type.MiniMapColor = ReadUInt16();
                        break;

                    case MetadataFlags3.LENS_HELP:
                        type.IsLensHelp = true;
                        type.LensHelp = ReadUInt16();
                        break;

                    case MetadataFlags3.FULL_GROUND:
                        type.IsFullGround = true;
                        break;

                    default:
                        throw new Exception("Unknown flag: " + flag.ToString("X2") + " (previous: " + previusFlag.ToString("X2") + ", category: " + type.Category + ", id: " + type.Id + ")");
                }
            }

            return true;
        }
    }
}
