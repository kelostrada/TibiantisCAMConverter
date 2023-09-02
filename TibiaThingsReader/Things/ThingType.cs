using System;
using System.Collections.Generic;
using TibiaThingsReader.Animation;
using TibiaThingsReader.Geometry;
using TibiaThingsReader.Sprites;
using TibiaThingsReader.Things;

namespace TibiaThingsReader
{
    public class ThingType
    {
        //--------------------------------------------------------------------------
        // PROPERTIES
        //--------------------------------------------------------------------------

        public uint Id { get; set; }
        public string Category { get; set; }
        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint ExactSize { get; set; }
        public uint Layers { get; set; }
        public uint PatternX { get; set; }
        public uint PatternY { get; set; }
        public uint PatternZ { get; set; }
        public uint Frames { get; set; }
        public uint[] SpriteIndex { get; set; }
        public bool IsGround { get; set; }
        public uint GroundSpeed { get; set; }
        public bool IsGroundBorder { get; set; }
        public bool IsOnBottom { get; set; }
        public bool IsOnTop { get; set; }
        public bool IsContainer { get; set; }
        public bool Stackable { get; set; }
        public bool ForceUse { get; set; }
        public bool MultiUse { get; set; }
        public bool HasCharges { get; set; }
        public bool Writable { get; set; }
        public bool WritableOnce { get; set; }
        public uint MaxTextLength { get; set; }
        public bool IsFluidContainer { get; set; }
        public bool IsFluid { get; set; }
        public bool IsUnpassable { get; set; }
        public bool IsUnmoveable { get; set; }
        public bool BlockMissile { get; set; }
        public bool BlockPathfind { get; set; }
        public bool NoMoveAnimation { get; set; }
        public bool Pickupable { get; set; }
        public bool Hangable { get; set; }
        public bool IsVertical { get; set; }
        public bool IsHorizontal { get; set; }
        public bool Rotatable { get; set; }
        public bool HasLight { get; set; }
        public uint LightLevel { get; set; }
        public uint LightColor { get; set; }
        public bool DontHide { get; set; }
        public bool IsTranslucent { get; set; }
        public bool FloorChange { get; set; }
        public bool HasOffset { get; set; }
        public uint OffsetX { get; set; }
        public uint OffsetY { get; set; }
        public bool HasElevation { get; set; }
        public uint Elevation { get; set; }
        public bool IsLyingObject { get; set; }
        public bool AnimateAlways { get; set; }
        public bool MiniMap { get; set; }
        public uint MiniMapColor { get; set; }
        public bool IsLensHelp { get; set; }
        public uint LensHelp { get; set; }
        public bool IsFullGround { get; set; }
        public bool IgnoreLook { get; set; }
        public bool Cloth { get; set; }
        public uint ClothSlot { get; set; }
        public bool IsMarketItem { get; set; }
        public string MarketName { get; set; }
        public uint MarketCategory { get; set; }
        public uint MarketTradeAs { get; set; }
        public uint MarketShowAs { get; set; }
        public uint MarketRestrictProfession { get; set; }
        public uint MarketRestrictLevel { get; set; }
        public bool HasDefaultAction { get; set; }
        public uint DefaultAction { get; set; }
        public bool Wrappable { get; set; }
        public bool Unwrappable { get; set; }
        public bool TopEffect { get; set; }
        public bool Usable { get; set; }

        public bool IsAnimation { get; set; }
        public uint AnimationMode { get; set; }
        public int LoopCount { get; set; }
        public int StartFrame { get; set; }
        public FrameDuration[] FrameDurations { get; set; }

        //--------------------------------------------------------------------------
        // CONSTRUCTOR
        //--------------------------------------------------------------------------

        public ThingType()
        {
        }

        //--------------------------------------------------------------------------
        // METHODS
        //--------------------------------------------------------------------------

        //--------------------------------------
        // Public
        //--------------------------------------

        public override string ToString()
        {
            return "[ThingType category=" + Category + ", id=" + Id + "]";
        }

        public uint GetTotalSprites()
        {
            return Width * Height * PatternX * PatternY * PatternZ * Frames * Layers;
        }

        public uint GetTotalTextures()
        {
            return PatternX * PatternY * PatternZ * Frames * Layers;
        }

        public uint GetSpriteIndex(uint width, uint height, uint layer, uint patternX, uint patternY, uint patternZ, uint frame)
        {
            return ((((((frame % Frames) * PatternZ + patternZ) * PatternY + patternY) * PatternX + patternX) * Layers + layer) * Height + height) * Width + width;
        }

        public uint GetTextureIndex(uint layer, uint patternX, uint patternY, uint patternZ, uint frame)
        {
            return (((frame % Frames * PatternZ + patternZ) * PatternY + patternY) * PatternX + patternX) * Layers + layer;
        }

        public Size GetSpriteSheetSize()
        {
            Size size = new Size
            {
                Width = PatternZ * PatternX * Layers * Width * Sprite.DEFAULT_SIZE,
                Height = Frames * PatternY * Height * Sprite.DEFAULT_SIZE
            };
            return size;
        }

        //public ThingType Clone()
        //{
        //    var newThing = new ThingType();
        //    var properties = this.GetType().GetProperties();
        //    foreach (var property in properties)
        //    {
        //        if (property.CanWrite)
        //        {
        //            property.SetValue(newThing, property.GetValue(this));
        //        }
        //    }

        //    if (SpriteIndex != null)
        //        newThing.SpriteIndex = new List<uint>(SpriteIndex);

        //    if (IsAnimation)
        //    {
        //        var durations = new List<FrameDuration>(Frames);
        //        foreach (var frameDuration in FrameDurations)
        //        {
        //            durations.Add(frameDuration.Clone());
        //        }

        //        newThing.AnimationMode = AnimationMode;
        //        newThing.LoopCount = LoopCount;
        //        newThing.StartFrame = StartFrame;
        //        newThing.FrameDurations = durations;
        //    }

        //    return newThing;
        //}

        //--------------------------------------------------------------------------
        // STATIC
        //--------------------------------------------------------------------------

        public static ThingType Create(uint id, string category)
        {
            if (ThingCategory.GetCategory(category) == null)
                throw new Exception("invalidCategory");

            var thing = new ThingType
            {
                Category = category,
                Id = id,
                Width = 1,
                Height = 1,
                Layers = 1,
                Frames = 1,
                PatternX = 1,
                PatternY = 1,
                PatternZ = 1,
                ExactSize = 32
            };

            if (category == ThingCategory.OUTFIT)
            {
                thing.PatternX = 4; // Directions
                thing.Frames = 3;   // Animations
                thing.IsAnimation = true;
                thing.FrameDurations = new FrameDuration[(int)thing.Frames];

                var duration = FrameDuration.GetDefaultDuration(category);
                for (var i = 0; i < thing.Frames; i++)
                {
                    thing.FrameDurations[i] = new FrameDuration(duration, duration);
                }
            }
            else if (category == ThingCategory.MISSILE)
            {
                thing.PatternX = 3;
                thing.PatternY = 3;
            }

            thing.SpriteIndex = new uint[(int)thing.GetTotalSprites()];
            return thing;
        }
    }
}
