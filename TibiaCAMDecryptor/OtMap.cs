﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace TibiaCAMDecryptor {
    public class OtMap {
        private const int SPAWN_RADIUS = 3;
        private const int SPAWN_SIZE = SPAWN_RADIUS * 2 + 1;

        #region Enums

        public enum OtMapNodeTypes {
            ROOTV1 = 0,
            ROOTV2 = 1,
            MAP_DATA = 2,
            ITEM_DEF = 3,
            TILE_AREA = 4,
            TILE = 5,
            ITEM = 6,
            TILE_SQUARE = 7,
            TILE_REF = 8,
            SPAWNS = 9,
            SPAWN_AREA = 10,
            MONSTER = 11,
            TOWNS = 12,
            TOWN = 13,
            HOUSETILE = 14,
            WAYPOINTS = 15,
            WAYPOINT = 16
        };

        public enum OtMapAttribute {
            NONE = 0,
            DESCRIPTION = 1,
            EXT_FILE = 2,
            TILE_FLAGS = 3,
            ACTION_ID = 4,
            UNIQUE_ID = 5,
            TEXT = 6,
            DESC = 7,
            TELE_DEST = 8,
            ITEM = 9,
            DEPOT_ID = 10,
            EXT_SPAWN_FILE = 11,
            RUNE_CHARGES = 12,
            EXT_HOUSE_FILE = 13,
            HOUSEDOORID = 14,
            COUNT = 15,
            DURATION = 16,
            DECAYING_STATE = 17,
            WRITTENDATE = 18,
            WRITTENBY = 19,
            SLEEPERGUID = 20,
            SLEEPSTART = 21,
            CHARGES = 22,
            CONTAINER_ITEMS = 23,
            ATTRIBUTE_MAP = 128
        };

        #endregion

        private Dictionary<uint, OtTown> towns;
        private Dictionary<ulong, OtTile> tiles;
        private Dictionary<Location, int> tileVisits;
        private Dictionary<uint, OtCreature> creatures;
        private Dictionary<string, Dictionary<Location, int>> cumulatedMonsters;
        private Dictionary<string, Dictionary<Location, int>> cumulatedNpcs;
        private Dictionary<ulong, OtSpawn> spawns;

        public uint Version { get; set; }

        public ushort Width { get; set; }
        public ushort Height { get; set; }

        public uint MajorVersionItems { get; set; }
        public uint MinorVersionItems { get; set; }

        public string HouseFile { get; set; }
        public string SpawnFile { get; set; }

        public OtItems Items { get; private set; }

        public List<string> Descriptions { get; private set; }

        public OtMap(OtItems items) {
            tiles = new Dictionary<ulong, OtTile>();
            creatures = new Dictionary<uint, OtCreature>();
            spawns = new Dictionary<ulong, OtSpawn>();
            towns = new Dictionary<uint, OtTown>();
            tileVisits = new Dictionary<Location, int>();
            cumulatedMonsters = new Dictionary<string, Dictionary<Location, int>>();
            cumulatedNpcs = new Dictionary<string, Dictionary<Location, int>>();

            Version = 2;
            Width = 0xFCFC;
            Height = 0xFCFC;

            Items = items;

            Descriptions = new List<string>();
        }

        public bool HasTile(ulong index) {
            return tiles.ContainsKey(index);
        }

        public bool HasTile(Location location) {
            return HasTile(location.ToIndex());
        }

        public OtTile GetTile(Location location) {
            return GetTile(location.ToIndex());
        }

        public OtTile GetTile(ulong index) {
            if (tiles.ContainsKey(index))
                return tiles[index];

            return null;
        }

        public void SetTile(ulong index, OtTile tile) {
            tiles[index] = tile;
        }

        public void SetTile(OtTile tile) {
            SetTile(tile.Location.ToIndex(), tile);
        }

        public void VisitTile(Location location)
        {
            if (tileVisits.ContainsKey(location))
            {
                tileVisits[location]++;
            }
            else
            {
                tileVisits[location] = 1;
            }
        }

        public void AddCreature(OtCreature creature) {
            if (!creatures.ContainsKey(creature.Id)) {
                creatures.Add(creature.Id, creature);
            }
        }

        public IEnumerable<OtTile> Tiles { get { return tiles.Values; } }
        public IEnumerable<OtSpawn> Spawns { get { return spawns.Values; } }

        public int TileCount { get { return tiles.Count; } }

        public void Clear() {
            lock (this) {
                tiles.Clear();
                spawns.Clear();
                creatures.Clear();
                tileVisits.Clear();
            }
        }

        public void CumulateSpawns()
        {
            lock (this)
            {
                foreach (var creature in creatures.Values)
                {

                    if (creature.Type == CreatureType.NPC)
                    {
                        if (!cumulatedNpcs.ContainsKey(creature.Name))
                        {
                            cumulatedNpcs[creature.Name] = new Dictionary<Location, int>();
                        }

                        if (!cumulatedNpcs[creature.Name].ContainsKey(creature.Location))
                        {
                            cumulatedNpcs[creature.Name][creature.Location] = 1;
                        }
                        else
                        {
                            cumulatedNpcs[creature.Name][creature.Location]++;
                        }
                    }

                    if (creature.Type == CreatureType.MONSTER)
                    {
                        if (!cumulatedMonsters.ContainsKey(creature.Name))
                        {
                            cumulatedMonsters[creature.Name] = new Dictionary<Location, int>();
                        }

                        if (!cumulatedMonsters[creature.Name].ContainsKey(creature.Location))
                        {
                            cumulatedMonsters[creature.Name][creature.Location] = 1;
                        }
                        else
                        {
                            cumulatedMonsters[creature.Name][creature.Location]++;
                        }
                    }
                }


                creatures.Clear();
            }
        }

        private static int SpawnRange = 3;
        private static int SpawnRadius = 1;

        public void AggregateSpawns()
        {
            var aggregatedSpawns = new Dictionary<OtCreature, float>();

            foreach (var monster in cumulatedMonsters)
            {
                var handledTiles = new List<Location>();

                foreach (var tile in monster.Value)
                {
                    if (handledTiles.Contains(tile.Key)) continue;

                    handledTiles = walkTiles(tile.Key, tile.Key, SpawnRange, monster, aggregatedSpawns, handledTiles);
                }
            }

            foreach (var spawn in aggregatedSpawns)
            {
                var creature = spawn.Key;
                var spawnLocation = creature.Location;
                var spawnIndex = spawnLocation.ToIndex();
                // var radius = (int) Math.Floor(Math.Pow(Math.Floor(spawn.Value), 1.0/3));
                if (!spawns.ContainsKey(spawnIndex))
                    spawns.Add(spawnIndex, new OtSpawn(spawnLocation, SpawnRadius));

                int spawnCount = spawn.Value > 0.02 ? (int)Math.Ceiling(spawn.Value) : 0;

                for (int i = 0; i < spawnCount; i++)
                {
                    spawns[spawnIndex].AddCreature(creature);
                }
            }
        }

        private List<Location> walkTiles(Location location, Location startLocation, int range, KeyValuePair<string, Dictionary<Location, int>> creatureTiles, Dictionary<OtCreature, float> aggregatedSpawns, List<Location> handledTiles)
        {
            if (handledTiles.Contains(location)) return handledTiles;
            
            if (creatureTiles.Value.ContainsKey(location))
            {
                var creature = new OtCreature() { Location = startLocation, Name = creatureTiles.Key, Type = CreatureType.MONSTER };
                if (!aggregatedSpawns.ContainsKey(creature))
                {
                    aggregatedSpawns.Add(creature, creatureTiles.Value[location] / (float)tileVisits[location]);
                }
                else
                {
                    aggregatedSpawns[creature] += creatureTiles.Value[location] / (float)tileVisits[location];
                }

                range = SpawnRange;
            }

            handledTiles.Add(location);

            if (range == 0) return handledTiles;

            handledTiles = walkTiles(new Location(location.X + 1, location.Y, location.Z), startLocation, range - 1, creatureTiles, aggregatedSpawns, handledTiles);
            handledTiles = walkTiles(new Location(location.X - 1, location.Y, location.Z), startLocation, range - 1, creatureTiles, aggregatedSpawns, handledTiles);
            handledTiles = walkTiles(new Location(location.X, location.Y + 1, location.Z), startLocation, range - 1, creatureTiles, aggregatedSpawns, handledTiles);
            handledTiles = walkTiles(new Location(location.X, location.Y - 1, location.Z), startLocation, range - 1, creatureTiles, aggregatedSpawns, handledTiles);

            return handledTiles;
        }


        #region Save

        public void Save(string fileName) {
            var dir = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var baseFileName = Path.GetFileNameWithoutExtension(fileName);
            string otbmFileName = baseFileName + ".otbm";
            HouseFile = baseFileName + "-house.xml";
            SpawnFile = baseFileName + "-spawn.xml";

            SaveOtbm(Path.Combine(dir, otbmFileName));
            SaveHouses(Path.Combine(dir, HouseFile));
            SaveSpawns(Path.Combine(dir, SpawnFile));
        }

        private void SaveOtbm(string fileName) {
            using (var writer = new OtFileWriter(fileName)) {
                //Header
                writer.Write((uint)0, false);

                writer.WriteNodeStart((byte)OtMapNodeTypes.ROOTV1);

                writer.Write(Version);
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(Items.MajorVersion);
                writer.Write((uint)3);

                //Map Data
                writer.WriteNodeStart((byte)OtMapNodeTypes.MAP_DATA);


                foreach (var description in Descriptions) {
                    writer.Write((byte)OtMapAttribute.DESCRIPTION);
                    writer.Write(description);
                }

                writer.Write((byte)OtMapAttribute.EXT_HOUSE_FILE);
                writer.Write(HouseFile);

                writer.Write((byte)OtMapAttribute.EXT_SPAWN_FILE);
                writer.Write(SpawnFile);

                foreach (var tile in Tiles) {
                    writer.WriteNodeStart((byte)OtMapNodeTypes.TILE_AREA);
                    writer.Write((ushort)(tile.Location.X & 0xFF00));
                    writer.Write((ushort)(tile.Location.Y & 0xFF00));
                    writer.Write((byte)tile.Location.Z);

                    if (tile.HouseId > 0)
                        writer.WriteNodeStart((byte)OtMapNodeTypes.HOUSETILE);
                    else
                        writer.WriteNodeStart((byte)OtMapNodeTypes.TILE);

                    writer.Write((byte)tile.Location.X);
                    writer.Write((byte)tile.Location.Y);

                    if (tile.HouseId > 0)
                        writer.Write(tile.HouseId);

                    if (tile.Flags > 0) {
                        writer.Write((byte)OtMapAttribute.TILE_FLAGS);
                        writer.Write(tile.Flags);
                    }

                    if (tile.Ground != null) {
                        writer.WriteNodeStart((byte)OtMapNodeTypes.ITEM);
                        writer.Write(tile.Ground.Type.Id);
                        tile.Ground.Serialize(writer, writer.GetPropertyWriter());
                        writer.WriteNodeEnd(); //Item
                    }

                    foreach (var item in tile.Items) {
                        writer.WriteNodeStart((byte)OtMapNodeTypes.ITEM);

                        writer.Write(item.Type.Id);
                        item.Serialize(writer, writer.GetPropertyWriter());

                        writer.WriteNodeEnd(); //Item
                    }

                    writer.WriteNodeEnd(); //Tile
                    writer.WriteNodeEnd(); //Tile Area
                }

                writer.WriteNodeStart((byte)OtMapNodeTypes.TOWNS);

                foreach (var town in towns.Values) {
                    writer.WriteNodeStart((byte)OtMapNodeTypes.TOWN);
                    writer.Write(town.Id);
                    writer.Write(town.Name);
                    writer.Write(town.TempleLocation);
                    writer.WriteNodeEnd(); //Town
                }

                writer.WriteNodeEnd(); //Towns

                writer.WriteNodeEnd(); //Map Data
                writer.WriteNodeEnd(); //Root
            }
        }

        private void SaveSpawns(string spawnFileName) {
            XElement spawns = new XElement("spawns");

            foreach (var s in Spawns) {
                XElement spawn = new XElement("spawn");
                spawn.Add(new XAttribute("centerx", s.Location.X));
                spawn.Add(new XAttribute("centery", s.Location.Y));
                spawn.Add(new XAttribute("centerz", s.Location.Z));
                spawn.Add(new XAttribute("radius", s.Radius));

                foreach (var creature in s.GetCreatures()) {
                    XElement creatureSpawn = new XElement(creature.Type == CreatureType.NPC ? "npc" : "monster");
                    creatureSpawn.Add(new XAttribute("name", creature.Name));
                    creatureSpawn.Add(new XAttribute("x", creature.Location.X));
                    creatureSpawn.Add(new XAttribute("y", creature.Location.Y));
                    creatureSpawn.Add(new XAttribute("z", creature.Location.Z));
                    creatureSpawn.Add(new XAttribute("spawntime", "60"));

                    spawn.Add(creatureSpawn);
                }

                spawns.Add(spawn);
            }

            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Encoding = ASCIIEncoding.UTF8;
            xmlWriterSettings.Indent = true;
            using (var xmlWriter = XmlWriter.Create(spawnFileName, xmlWriterSettings)) {
                spawns.Save(xmlWriter);
            }
        }

        private void SaveHouses(string housesFileName) {
            XElement houses = new XElement("houses");

            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Encoding = Encoding.UTF8;
            xmlWriterSettings.Indent = true;
            using (var xmlWriter = XmlWriter.Create(housesFileName, xmlWriterSettings)) {
                houses.Save(xmlWriter);
            }
        }

        #endregion

        #region Load

        public void Load(string fileName, bool replaceTiles) {
            if (!File.Exists(fileName))
                throw new Exception(string.Format("File not found {0}.", fileName));

            string spawnFile = null;
            string houseFile = null;
            var tileLocations = new HashSet<ulong>();

            using (var reader = new OtFileReader(fileName)) {
                OtFileNode node = reader.GetRootNode();

                OtPropertyReader props = reader.GetPropertyReader(node);

                props.ReadByte(); // junk?

                var version = props.ReadUInt32();
                props.ReadUInt16();
                props.ReadUInt16();

                var majorVersionItems = props.ReadUInt32();
                var minorVersionItems = props.ReadUInt32();

                if (version <= 0) {
                    //In otbm version 1 the count variable after splashes/fluidcontainers and stackables
                    //are saved as attributes instead, this solves alot of problems with items
                    //that is changed (stackable/charges/fluidcontainer/splash) during an update.
                    throw new Exception(
                        "This map needs to be upgraded by using the latest map editor version to be able to load correctly.");
                }

                if (version > 3) {
                    throw new Exception("Unknown OTBM version detected.");
                }

                if (majorVersionItems < 3) {
                    throw new Exception(
                        "This map needs to be upgraded by using the latest map editor version to be able to load correctly.");
                }

                if (majorVersionItems > Items.MajorVersion) {
                    throw new Exception("The map was saved with a different items.otb version, an upgraded items.otb is required.");
                }

                if (minorVersionItems > Items.MinorVersion)
                    Trace.WriteLine("This map needs an updated items.otb.");

                node = node.Child;

                if ((OtMapNodeTypes)node.Type != OtMapNodeTypes.MAP_DATA) {
                    throw new Exception("Could not read data node.");
                }

                props = reader.GetPropertyReader(node);

                while (props.PeekChar() != -1) {
                    byte attribute = props.ReadByte();
                    switch ((OtMapAttribute)attribute) {
                        case OtMapAttribute.DESCRIPTION:
                            var description = props.GetString();
                            //Descriptions.Add(description);
                            break;
                        case OtMapAttribute.EXT_SPAWN_FILE:
                            spawnFile = props.GetString();
                            break;
                        case OtMapAttribute.EXT_HOUSE_FILE:
                            houseFile = props.GetString();
                            break;
                        default:
                            throw new Exception("Unknown header node.");
                    }
                }

                OtFileNode nodeMapData = node.Child;

                while (nodeMapData != null) {
                    switch ((OtMapNodeTypes)nodeMapData.Type) {
                        case OtMapNodeTypes.TILE_AREA:
                            ParseTileArea(reader, nodeMapData, replaceTiles, tileLocations);
                            break;
                        case OtMapNodeTypes.TOWNS:
                            ParseTowns(reader, nodeMapData);
                            break;
                    }
                    nodeMapData = nodeMapData.Next;
                }
            }
        }

        private void ParseTileArea(OtFileReader reader, OtFileNode otbNode, bool replaceTiles, ISet<ulong> tileLocations) {
            OtPropertyReader props = reader.GetPropertyReader(otbNode);

            int baseX = props.ReadUInt16();
            int baseY = props.ReadUInt16();
            int baseZ = props.ReadByte();

            OtFileNode nodeTile = otbNode.Child;

            while (nodeTile != null) {
                if (nodeTile.Type == (long)OtMapNodeTypes.TILE ||
                    nodeTile.Type == (long)OtMapNodeTypes.HOUSETILE) {
                    props = reader.GetPropertyReader(nodeTile);

                    var tileLocation = new Location(baseX + props.ReadByte(), baseY + props.ReadByte(), baseZ);

                    var tile = new OtTile(tileLocation);

                    if (nodeTile.Type == (long)OtMapNodeTypes.HOUSETILE) {
                        tile.HouseId = props.ReadUInt32();
                    }

                    while (props.PeekChar() != -1) {
                        byte attribute = props.ReadByte();
                        switch ((OtMapAttribute)attribute) {
                            case OtMapAttribute.TILE_FLAGS: {
                                tile.Flags = props.ReadUInt32();
                                break;
                            }
                            case OtMapAttribute.ITEM: {
                                ushort itemId = props.ReadUInt16();

                                var itemType = Items.GetItem(itemId);
                                if (itemType == null) {
                                    throw new Exception("Unkonw item type " + itemId + " in position " + tileLocation + ".");
                                }

                                var item = OtItem.Create(itemType);
                                tile.InternalAddItem(item);

                                break;
                            }
                            default:
                                throw new Exception(string.Format("{0} Unknown tile attribute.", tileLocation));
                        }
                    }

                    OtFileNode nodeItem = nodeTile.Child;

                    while (nodeItem != null) {
                        if (nodeItem.Type == (long)OtMapNodeTypes.ITEM) {
                            props = reader.GetPropertyReader(nodeItem);

                            ushort itemId = props.ReadUInt16();

                            var itemType = Items.GetItem(itemId);
                            if (itemType == null) {
                                throw new Exception("Unkonw item type " + itemId + " in position " + tileLocation + ".");
                            }

                            var item = OtItem.Create(itemType);
                            item.Deserialize(reader, nodeItem, props, Items);

                            tile.InternalAddItem(item);
                        } else {
                            throw new Exception(string.Format("{0} Unknown node type.", tileLocation));
                        }
                        nodeItem = nodeItem.Next;
                    }

                    var index = tileLocation.ToIndex();
                    var hasTile = HasTile(index);

                    if (!hasTile) {
                        SetTile(tile);
                        tileLocations.Add(tileLocation.ToIndex());
                    } else if (replaceTiles)
                        SetTile(tile);
                }

                nodeTile = nodeTile.Next;
            }
        }

        private void ParseTowns(OtFileReader reader, OtFileNode otbNode) {
            OtFileNode nodeTown = otbNode.Child;

            while (nodeTown != null) {
                OtPropertyReader props = reader.GetPropertyReader(nodeTown);

                uint townid = props.ReadUInt32();
                string townName = props.GetString();
                var templeLocation = props.ReadLocation();

                var town = new OtTown { Id = townid, Name = townName, TempleLocation = templeLocation };
                towns[townid] = town;

                nodeTown = nodeTown.Next;
            }
        }

        #endregion

    }
}
