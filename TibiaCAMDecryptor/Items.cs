using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TibiaThingsReader;
using TibiaThingsReader.Things;

namespace TibiaCAMDecryptor {
    public class Items {
        private Dictionary<ushort, ItemType> items;

        public int ItemCount { get; private set; }

        public Items() {
            items = new Dictionary<ushort, ItemType>();
        }

        public ItemType Get(ushort id) {
            if (items.ContainsKey(id))
                return items[id];

            return null;
        }

        public bool Load(string filename, int version) {
            FileStream fileStream = File.OpenRead(filename);
            try {
                using (MetadataReader reader = new MetadataReader3(fileStream)) {

                    if (reader.BaseStream.Length < 12)
                    {
                        throw new ArgumentException("Not enough data.");
                    }

                    var signature = reader.ReadSignature();
                    ItemCount = (int)reader.ReadItemsCount();
                    reader.ReadOutfitsCount();
                    reader.ReadOutfitsCount();
                    reader.ReadMissilesCount();

                    // Load item list.
                    if (!LoadItemsList(reader))
                        throw new Exception("Items list cannot be created.");


                    //UInt32 datSignature = reader.ReadUInt32();
                    ////if (signature != 0 && datSignature != signature)
                    ////{
                    ////    Trace.WriteLine(String.Format("Plugin: Bad signature, dat signature is {0} and signature is {0}", datSignature, signature));
                    ////    return false;
                    //// }

                    ////get max id
                    //ItemCount = reader.ReadUInt16();
                    //UInt16 creatureCount = reader.ReadUInt16();
                    //UInt16 effectCount = reader.ReadUInt16();
                    //UInt16 distanceCount = reader.ReadUInt16();

                    //UInt16 minclientID = 100; //items starts at 100
                    //int maxclientID = ItemCount + creatureCount;

                    //UInt16 id = minclientID;
                    //while (id <= maxclientID) {
                    //    ItemType item = new ItemType();

                    //    item.Id = id;
                    //    item.AlwaysOnTopOrder = 5;
                    //    item.IsMoveable = true;

                    //    items[id] = item;

                    //    // read the options until we find 0xff
                    //    byte optbyte;
                    //    do {
                    //        optbyte = reader.ReadByte();
                    //        //Trace.WriteLine(String.Format("{0:X}", optbyte));

                    //        switch (optbyte) {
                    //            case 0x00: //groundtile
                    //                item.GroundSpeed = reader.ReadUInt16();
                    //                item.AlwaysOnTopOrder = 0;
                    //                item.Group = ItemGroup.Ground;
                    //                break;
                    //            case 0x01: //all OnTop
                    //                item.AlwaysOnTop = true;
                    //                item.AlwaysOnTopOrder = 1;
                    //                break;
                    //            case 0x02: //can walk trough (open doors, arces, bug pen fence)
                    //                item.AlwaysOnTop = true;
                    //                item.AlwaysOnTopOrder = 2;
                    //                break;
                    //            case 0x03: //container
                    //                item.Group = ItemGroup.Container;
                    //                break;
                    //            case 0x04: //stackable
                    //                item.IsStackable = true;
                    //                break;
                    //            case 0x05: //unknown
                    //                break;
                    //            case 0x06: //useable
                    //                item.HasUseWith = true;
                    //                break;
                    //            case 0x07: //read/write-able
                    //                item.IsReadable = true;
                    //                //item.isWriteable = true;
                    //                item.MaxReadWriteChars = reader.ReadUInt16();
                    //                break;
                    //            case 0x08: //readable
                    //                item.IsReadable = true;
                    //                item.MaxReadChars = reader.ReadUInt16();
                    //                break;
                    //            case 0x09: //fluid containers
                    //                item.Group = ItemGroup.FluidContainer;
                    //                break;
                    //            case 0x0A: //splashes
                    //                item.Group = ItemGroup.Splash;
                    //                break;
                    //            case 0x0B: //blocks solid objects (creatures, walls etc)
                    //                item.BlockObject = true;
                    //                break;
                    //            case 0x0C: //not moveable
                    //                item.IsMoveable = false;
                    //                break;
                    //            case 0x0D: //blocks missiles (walls, magic wall etc)
                    //                item.BlockProjectile = true;
                    //                break;
                    //            case 0x0E: //blocks pathfind algorithms (monsters)
                    //                item.BlockPathFind = true;
                    //                break;
                    //            case 0x0F: // pickupable
                    //                break;
                    //            case 0x10: // has light
                    //                item.LightLevel = reader.ReadUInt16();
                    //                item.LightColor = reader.ReadUInt16();
                    //                break;
                    //            case 0x11: // floor change
                    //                break;
                    //            case 0x12: //full ground
                    //                break;
                    //            case 0x13: // elevation
                    //                item.HasHeight = true;
                    //                reader.ReadUInt16();
                    //                break;
                    //            case 0x14: // has offset
                    //                break;
                    //            case 0x15: //rotatable
                    //                break;
                    //            case 0x16: // minimap
                    //                item.MinimapColor = reader.ReadUInt16();
                    //                break;
                    //            case 0x17: // rotatable
                    //                break;
                    //            case 0x18: // lying object
                    //                break;
                    //            case 0x19: // hangable
                    //                break;
                    //            case 0x1A: // vertical
                    //                break;
                    //            case 0x1B: //horizontal
                    //                break;
                    //            case 0x1C: //animate always
                    //                break;
                    //            case 0x1D: // lens help
                    //                reader.ReadUInt16();
                    //                break;
                    //            case 0x1E: //in-game help info
                    //                UInt16 opt = reader.ReadUInt16();
                    //                if (opt == 1112)
                    //                    item.IsReadable = true;
                    //                break;
                    //            case 0xFF: //end of attributes
                    //                break;
                    //            default:
                    //                Console.WriteLine(String.Format("Plugin: Error while parsing, unknown optbyte 0x{0:X} at id {1}", optbyte, id));
                    //                return false;
                    //        }
                    //    } while (optbyte >= 0 && optbyte != 0xFF);

                    //    var width = reader.ReadByte();
                    //    var height = reader.ReadByte();
                    //    if ((width > 1) || (height > 1)) {
                    //        reader.ReadByte();
                    //    }

                    //    var xdiv = reader.ReadByte();
                    //    var ydiv = reader.ReadByte();
                    //    var zdiv = reader.ReadByte();
                    //    var animationLength = reader.ReadByte();
                    //    item.IsAnimation = animationLength > 1;

                    //    var numSprites =
                    //        (UInt32)width * (UInt32)height *
                    //        (UInt32)xdiv * (UInt32)ydiv * zdiv *
                    //        (UInt32)animationLength;

                    //    // Read the sprite ids
                    //    for (UInt32 i = 0; i < numSprites; ++i) {
                    //        var spriteId = reader.ReadUInt16();
                    //        /*Sprite sprite;
                    //        if (!sprites.TryGetValue(spriteId, out sprite))
                    //        {
                    //            sprite = new Sprite();
                    //            sprite.id = spriteId;
                    //            sprites[spriteId] = sprite;
                    //        }
                    //        item.spriteList.Add(sprite);*/
                    //    }
                    //    ++id;
                    //}
                }
            } finally {
                fileStream.Close();
            }

            return true;

        }

        private bool LoadItemsList(IMetadataReader reader )
        {

            for (uint id = 100; id <= ItemCount; id++)
            {
                ThingType thing = new ThingType();
                thing.Id = id;
                thing.Category = ThingCategory.ITEM;

                if (!reader.ReadProperties(thing))
                    return false;

                if (!reader.ReadTexturePatterns(thing, false, false))
                    return false;

                ItemType item = new ItemType();
                item.Id = (ushort)thing.Id;
                item.AlwaysOnTopOrder = 5;

                if (thing.IsGround)
                {
                    item.Group = ItemGroup.Ground;
                    item.AlwaysOnTopOrder = 0;
                }

                if (thing.IsOnBottom)
                {
                    item.AlwaysOnTopOrder = 1;
                }

                if (thing.IsOnTop)
                {
                    item.AlwaysOnTopOrder = 2;
                }

                if (thing.IsContainer)
                {
                    item.Group = ItemGroup.Container;
                }

                if (thing.IsFluidContainer)
                {
                    item.Group = ItemGroup.FluidContainer;
                }

                if (thing.IsFluid)
                {
                    item.Group = ItemGroup.Splash;
                }

                item.IsMoveable = !thing.IsUnmoveable;
                item.GroundSpeed = (ushort)thing.GroundSpeed;
                item.AlwaysOnTop = thing.IsOnTop;
                item.IsStackable = thing.Stackable;
                item.HasUseWith = thing.MultiUse;
                item.IsReadable = thing.Writable || thing.WritableOnce;
                item.MaxReadWriteChars = (ushort)thing.MaxTextLength;
                item.BlockObject = thing.IsUnpassable;
                item.BlockProjectile = thing.BlockMissile;
                item.BlockPathFind = thing.BlockPathfind;
                item.LightLevel = (ushort)thing.LightLevel;
                item.LightColor = (ushort)thing.LightColor;
                item.HasHeight = thing.HasElevation;
                item.MinimapColor = (ushort)thing.MiniMapColor;

                items[(ushort)id] = item;
            }
            return true;
        }


    }
}
