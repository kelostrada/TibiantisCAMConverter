using System;
namespace TibiaThingsReader.Things
{
    public interface IMetadataReader
    {
        uint ReadSignature();
        uint ReadItemsCount();
        uint ReadOutfitsCount();
        uint ReadEffectsCount();
        uint ReadMissilesCount();
        bool ReadProperties(ThingType type);
        bool ReadTexturePatterns(ThingType type, bool extended, bool frameDurations);
    }
}
