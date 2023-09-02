using System;
using System.Collections.Generic;
using System.IO;
using TibiaThingsReader.Animation;
using TibiaThingsReader.Sprites;

namespace TibiaThingsReader.Things
{
    /// <summary>
    /// Reader for versions 7.10 - 7.30
    /// </summary>
    public class MetadataReader1 : MetadataReader
    {
        public MetadataReader1(Stream stream) : base(stream)
        {

        }

        public override bool ReadProperties(ThingType type)
        {
            uint flag = 0;
            while (flag < MetadataFlags1.LAST_FLAG)
            {
                uint previousFlag = flag;
                flag = ReadByte();

                if (flag == MetadataFlags1.LAST_FLAG)
                    return true;

                switch (flag)
                {
                    case MetadataFlags1.GROUND:
                        type.IsGround = true;
                        type.GroundSpeed = ReadUInt16();
                        break;

                    case MetadataFlags1.ON_BOTTOM:
                        type.IsOnBottom = true;
                        break;

                    case MetadataFlags1.ON_TOP:
                        type.IsOnTop = true;
                        break;

                    case MetadataFlags1.CONTAINER:
                        type.IsContainer = true;
                        break;

                    case MetadataFlags1.STACKABLE:
                        type.Stackable = true;
                        break;

                    case MetadataFlags1.MULTI_USE:
                        type.MultiUse = true;
                        break;

                    case MetadataFlags1.FORCE_USE:
                        type.ForceUse = true;
                        break;

                    case MetadataFlags1.WRITABLE:
                        type.Writable = true;
                        type.MaxTextLength = ReadUInt16();
                        break;

                    case MetadataFlags1.WRITABLE_ONCE:
                        type.WritableOnce = true;
                        type.MaxTextLength = ReadUInt16();
                        break;

                    case MetadataFlags1.FLUID_CONTAINER:
                        type.IsFluidContainer = true;
                        break;

                    case MetadataFlags1.FLUID:
                        type.IsFluid = true;
                        break;

                    case MetadataFlags1.UNPASSABLE:
                        type.IsUnpassable = true;
                        break;

                    case MetadataFlags1.UNMOVEABLE:
                        type.IsUnmoveable = true;
                        break;

                    case MetadataFlags1.BLOCK_MISSILE:
                        type.BlockMissile = true;
                        break;

                    case MetadataFlags1.BLOCK_PATHFINDER:
                        type.BlockPathfind = true;
                        break;

                    case MetadataFlags1.PICKUPABLE:
                        type.Pickupable = true;
                        break;

                    case MetadataFlags1.HAS_LIGHT:
                        type.HasLight = true;
                        type.LightLevel = ReadUInt16();
                        type.LightColor = ReadUInt16();
                        break;

                    case MetadataFlags1.FLOOR_CHANGE:
                        type.FloorChange = true;
                        break;

                    case MetadataFlags1.FULL_GROUND:
                        type.IsFullGround = true;
                        break;

                    case MetadataFlags1.HAS_ELEVATION:
                        type.HasElevation = true;
                        type.Elevation = ReadUInt16();
                        break;

                    case MetadataFlags1.HAS_OFFSET:
                        type.HasOffset = true;
                        type.OffsetX = 8;
                        type.OffsetY = 8;
                        break;

                    case MetadataFlags1.MINI_MAP:
                        type.MiniMap = true;
                        type.MiniMapColor = ReadUInt16();
                        break;

                    case MetadataFlags1.ROTATABLE:
                        type.Rotatable = true;
                        break;

                    case MetadataFlags1.LYING_OBJECT:
                        type.IsLyingObject = true;
                        break;

                    case MetadataFlags1.ANIMATE_ALWAYS:
                        type.AnimateAlways = true;
                        break;

                    case MetadataFlags1.LENS_HELP:
                        type.IsLensHelp = true;
                        type.LensHelp = ReadUInt16();
                        break;

                    default:
                        throw new Exception("readUnknownFlag");
                             //+ flag.ToString("X") +
                             //                               previousFlag.ToString("X") +
                             //                               type.Category.ToString() + 
                             //                               type.Id);
                }
            }

            return true;
        }

        public override bool ReadTexturePatterns(ThingType type, bool extended, bool frameDurations)
        {
            int i;

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

            if (type.Frames > 1)
            {
                type.IsAnimation = true;
                type.FrameDurations = new FrameDuration[(int)type.Frames];

                if (frameDurations)
                {
                    type.AnimationMode = ReadByte();
                    type.LoopCount = ReadInt32();
                    type.StartFrame = ReadSByte();

                    for (i = 0; i < type.Frames; i++)
                    {
                        uint minimum = ReadUInt32();
                        uint maximum = ReadUInt32();
                        type.FrameDurations[i] = new FrameDuration(minimum, maximum);
                    }
                }
                else
                {
                    uint duration = FrameDuration.GetDefaultDuration(type.Category);
                    for (i = 0; i < type.Frames; i++)
                        type.FrameDurations[i] = new FrameDuration(duration, duration);
                }
            }

            uint totalSprites = type.GetTotalSprites();
            if (totalSprites > 4096)
                throw new Exception("A thing type has more than 4096 sprites.");

            type.SpriteIndex = new uint[(int)totalSprites];
            for (i = 0; i < totalSprites; i++)
            {
                if (extended)
                    type.SpriteIndex[i] = ReadUInt32();
                else
                    type.SpriteIndex[i] = ReadUInt16();
            }

            return true;
        }
    }
}
