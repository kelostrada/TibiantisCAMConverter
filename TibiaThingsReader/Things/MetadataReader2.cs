using System;
using System.Collections.Generic;
using System.IO;
using TibiaThingsReader.Animation;
using TibiaThingsReader.Sprites;

namespace TibiaThingsReader.Things
{
    /// <summary>
    /// Reader for versions 7.40 - 7.50
    /// </summary>
    public class MetadataReader2 : MetadataReader
    {
        // CONSTRUCTOR
        public MetadataReader2(Stream stream) : base(stream)
        {

        }

        // Public Override
        public override bool ReadProperties(ThingType type)
        {
            uint flag = 0;
            while (flag < MetadataFlags2.LAST_FLAG)
            {
                uint previusFlag = flag;
                flag = ReadByte();

                if (flag == MetadataFlags2.LAST_FLAG)
                    return true;

                switch (flag)
                {
                    case MetadataFlags2.GROUND:
                        type.IsGround = true;
                        type.GroundSpeed = ReadUInt16();
                        break;

                    case MetadataFlags2.ON_BOTTOM:
                        type.IsOnBottom = true;
                        break;

                    case MetadataFlags2.ON_TOP:
                        type.IsOnTop = true;
                        break;

                    case MetadataFlags2.CONTAINER:
                        type.IsContainer = true;
                        break;

                    case MetadataFlags2.STACKABLE:
                        type.Stackable = true;
                        break;

                    case MetadataFlags2.MULTI_USE:
                        type.MultiUse = true;
                        break;

                    case MetadataFlags2.FORCE_USE:
                        type.ForceUse = true;
                        break;

                    case MetadataFlags2.WRITABLE:
                        type.Writable = true;
                        type.MaxTextLength = ReadUInt16();
                        break;

                    case MetadataFlags2.WRITABLE_ONCE:
                        type.WritableOnce = true;
                        type.MaxTextLength = ReadUInt16();
                        break;

                    case MetadataFlags2.FLUID_CONTAINER:
                        type.IsFluidContainer = true;
                        break;

                    case MetadataFlags2.FLUID:
                        type.IsFluid = true;
                        break;

                    case MetadataFlags2.UNPASSABLE:
                        type.IsUnpassable = true;
                        break;

                    case MetadataFlags2.UNMOVEABLE:
                        type.IsUnmoveable = true;
                        break;

                    case MetadataFlags2.BLOCK_MISSILE:
                        type.BlockMissile = true;
                        break;

                    case MetadataFlags2.BLOCK_PATHFINDER:
                        type.BlockPathfind = true;
                        break;

                    case MetadataFlags2.PICKUPABLE:
                        type.Pickupable = true;
                        break;

                    case MetadataFlags2.HAS_LIGHT:
                        type.HasLight = true;
                        type.LightLevel = ReadUInt16();
                        type.LightColor = ReadUInt16();
                        break;

                    case MetadataFlags2.FLOOR_CHANGE:
                        type.FloorChange = true;
                        break;

                    case MetadataFlags2.FULL_GROUND:
                        type.IsFullGround = true;
                        break;

                    case MetadataFlags2.HAS_ELEVATION:
                        type.HasElevation = true;
                        type.Elevation = ReadUInt16();
                        break;

                    case MetadataFlags2.HAS_OFFSET:
                        type.HasOffset = true;
                        type.OffsetX = 8;
                        type.OffsetY = 8;
                        break;

                    case MetadataFlags2.MINI_MAP:
                        type.MiniMap = true;
                        type.MiniMapColor = ReadUInt16();
                        break;

                    case MetadataFlags2.ROTATABLE:
                        type.Rotatable = true;
                        break;

                    case MetadataFlags2.LYING_OBJECT:
                        type.IsLyingObject = true;
                        break;

                    case MetadataFlags2.HANGABLE:
                        type.Hangable = true;
                        break;

                    case MetadataFlags2.VERTICAL:
                        type.IsVertical = true;
                        break;

                    case MetadataFlags2.HORIZONTAL:
                        type.IsHorizontal = true;
                        break;

                    case MetadataFlags2.ANIMATE_ALWAYS:
                        type.AnimateAlways = true;
                        break;

                    case MetadataFlags2.LENS_HELP:
                        type.IsLensHelp = true;
                        type.LensHelp = ReadUInt16();
                        break;

                    default:
                        throw new Exception("readUnknownFlag");
                            //flag.ToString("X2"),
                            //previusFlag.ToString("X2"),
                            //Resources.GetString(type.Category),
                            //type.Id));
                }
            }

            return true;
        }

        public override bool ReadTexturePatterns(ThingType type, bool extended, bool frameDurations)
        {
            type.Width = ReadByte();
            type.Height = ReadByte();

            if (type.Width > 1 || type.Height > 1)
                type.ExactSize = ReadByte();
            else
                type.ExactSize = Sprite.DEFAULT_SIZE;

            type.Layers = ReadByte();
            type.PatternX = ReadByte();
            type.PatternY = ReadByte();
            type.PatternZ = 1;
            type.Frames = ReadByte();

            FrameDuration[] frameDurationsList = new FrameDuration[(int)type.Frames];
            if (type.Frames > 1)
            {
                type.IsAnimation = true;

                if (frameDurations)
                {
                    type.AnimationMode = ReadByte();
                    type.LoopCount = ReadInt32();
                    type.StartFrame = ReadSByte();

                    for (int i = 0; i < type.Frames; i++)
                    {
                        uint minimum = ReadUInt32();
                        uint maximum = ReadUInt32();
                        frameDurationsList[i] = new FrameDuration(minimum, maximum);
                    }
                }
                else
                {
                    uint duration = FrameDuration.GetDefaultDuration(type.Category);
                    for (int i = 0; i < type.Frames; i++)
                        frameDurationsList[i] = new FrameDuration(duration, duration);
                }
            }

            uint totalSprites = type.GetTotalSprites();
            if (totalSprites > 4096)
                throw new Exception("A thing type has more than 4096 sprites.");

            uint[] spriteIndexList = new uint[(int)totalSprites];
            for (int i = 0; i < totalSprites; i++)
            {
                if (extended)
                    spriteIndexList[i] = ReadUInt32();
                else
                    spriteIndexList[i] = ReadUInt16();
            }

            type.FrameDurations = frameDurationsList;
            type.SpriteIndex = spriteIndexList;

            return true;
        }
    }
}
