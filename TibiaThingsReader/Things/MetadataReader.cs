using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using TibiaThingsReader.Animation;
using TibiaThingsReader.Sprites;

namespace TibiaThingsReader.Things
{
    public class MetadataReader : BinaryReader, IMetadataReader
    {
        public MetadataReader(Stream stream) : base(stream, Encoding.Unicode)
        {
            //BaseStream.Seek(0, SeekOrigin.Begin);
        }

        public uint ReadSignature()
        {
            BaseStream.Position = MetadataFilePosition.SIGNATURE;
            return ReadUInt32();
        }

        public uint ReadItemsCount()
        {
            BaseStream.Position = MetadataFilePosition.ITEMS_COUNT;
            return ReadUInt16();
        }

        public uint ReadOutfitsCount()
        {
            BaseStream.Position = MetadataFilePosition.OUTFITS_COUNT;
            return ReadUInt16();
        }

        public uint ReadEffectsCount()
        {
            BaseStream.Position = MetadataFilePosition.EFFECTS_COUNT;
            return ReadUInt16();
        }

        public uint ReadMissilesCount()
        {
            BaseStream.Position = MetadataFilePosition.MISSILES_COUNT;
            return ReadUInt16();
        }

        public virtual bool ReadProperties(ThingType type)
        {
            throw new NotImplementedException();
        }

        public virtual bool ReadTexturePatterns(ThingType type, bool extended, bool frameDurations)
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
            type.PatternZ = ReadByte();
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

                    for (int i = 0; i < type.Frames; i++)
                    {
                        uint minimum = ReadUInt32();
                        uint maximum = ReadUInt32();
                        type.FrameDurations[i] = new FrameDuration(minimum, maximum);
                    }
                }
                else
                {
                    uint duration = FrameDuration.GetDefaultDuration(type.Category);
                    for (int i = 0; i < type.Frames; i++)
                    {
                        type.FrameDurations[i] = new FrameDuration(duration, duration);
                    }
                }
            }

            var totalSprites = type.GetTotalSprites();
            if (totalSprites > 4096)
                throw new Exception("A thing type has more than 4096 sprites.");

            type.SpriteIndex = new uint[(int)totalSprites];

            for (int i = 0; i < totalSprites; i++)
            {
                if (extended)
                {
                    type.SpriteIndex[i] = ReadUInt32();
                }
                else
                {
                    type.SpriteIndex[i] = ReadUInt16();
                }
            }

            return true;
        }
    }
}
