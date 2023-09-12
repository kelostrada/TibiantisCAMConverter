using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TibiaCAMDecryptor {
    public class InputMessage {
        public static Dictionary<byte, string> PacketHeads = new Dictionary<byte, string>() {
            { 0xA, "ParseLogin" },
            { 0x0B, "ParseGmActions ??" },
            { 0x14, "ParseDisconnectClient" },
            { 0x16, "ParseWaitList" },
            { 0x1E, "ParseClientPing" },
            { 0x64, "ParseMapDescription" },
            { 0x65, "ParseNorthMove" },
            { 0x66, "ParseEastMove" },
            { 0x67, "ParseSouthMove" },
            { 0x68, "ParseWestMove" },
            { 0x69, "ParseUpdateTile" },
            { 0x6A, "ParseAddTileItem" },
            { 0x6B, "ParseUpdateTileItem" },
            { 0x6C, "ParseRemoveTileItem" },
            { 0x6D, "ParseMoveCreature" },
            { 0x6E, "ParseContainer" },
            { 0x6F, "ParseCloseContainer" },
            { 0x70, "ParseAddContainerItem" },
            { 0x71, "ParseUpdateContainerItem" },
            { 0x72, "ParseRemoveContainerItem" },
            { 0x78, "ParseInventoryItem" },
            { 0x79, "ParseInventoryItem" },
            { 0x7D, "ParseOwnTradeItemRequest" },
            { 0x7E, "ParseCounterTradeItemRequest" },
            { 0x7F, "ParseCloseTrade" },
            { 0x82, "ParseWorldLight" },
            { 0x83, "ParseMagicEffect" },
            { 0x84, "ParseAnimatedText" },
            { 0x85, "ParseDistanceShoot" },
            { 0x86, "ParseCreatureSquare" },
            { 0x8C, "ParseCreatureHealth" },
            { 0x8D, "ParseCreatureLight" },
            { 0x8E, "ParseCreatureOutfit" },
            { 0x8F, "ParseChangeSpeed" },
            { 0x90, "ParseCreatureSkull" },
            { 0x91, "ParseCreatureShield" },
            { 0x96, "ParseTextWindow" },
            { 0x97, "ParseHouseWindow" },
            { 0xA0, "ParsePlayerStats" },
            { 0xA1, "ParsePlayerSkills" },
            { 0xA2, "ParsePlayerIcons" },
            { 0xA3, "CancelTarget" },
            { 0xAA, "ParseCreatureSpeak" },
            { 0xAB, "ParseChannelsDialog" },
            { 0xAC, "ParseChannel" },
            { 0xAD, "ParseOpenPrivateChannel" },
            { 0xAE, "ParseRuleViolationsChannel" },
            { 0xAF, "ParseRemoveReport" },
            { 0xB0, "ParsesRuleViolationCancel" },
            { 0xB1, "ParseLockRuleViolation" },
            { 0xB2, "ParseCreatePrivateChannel" },
            { 0xB3, "ParseClosePrivate" },
            { 0xB4, "ParseTextMessage" },
            { 0xB5, "ParseCancelWalk" },
            { 0xBE, "ParseFloorChangeUp" },
            { 0xBF, "ParseFloorChangeDown" },
            { 0xC8, "ParseOutfitWindow" },
            { 0xD2, "ParseVIP" },
            { 0xD3, "ParseVIPLogin" },
            { 0xD4, "ParseVIPLogout" }
        };

        private byte[] buffer;
        private int position;

        public InputMessage(byte[] buffer) {
            this.buffer = buffer; // initialize buffer
            position = 0;
        }

        public Location getLocation() {
            return new Location(getU16(), getU16(), getByte());
        }

        public byte getByte() {
            return buffer[position++];
        }

        public ushort getU16() {
            ushort val = BitConverter.ToUInt16(buffer, position);
            position += 2;
            return val;
        }

        public uint getU32() {
            uint val = BitConverter.ToUInt32(buffer, position);
            position += 4;
            return val;
        }

        public ulong getU64() {
            ulong val = BitConverter.ToUInt64(buffer, position);
            position += 8;
            return val;
        }

        public ushort PeekU16() {
            return BitConverter.ToUInt16(buffer, position);
        }

        public string getString() {
            ushort stringLen = getU16();
            string val = Encoding.ASCII.GetString(buffer, position, stringLen);
            position += stringLen;
            return val;
        }

        public Outfit getOutfit() {
            var lookType = getU16();
            if (lookType != 0)
                return new Outfit(lookType, getByte(), getByte(), getByte(), getByte());
            else
                return new Outfit(lookType, getU16());
        }

        public double getDouble() {
            byte precision = getByte();
            uint val = getU32();
            return 0; // not yet
        }

        public bool getBool() {
            return (getByte() == 1) ? true : false;
        }

        public int getLength() {
            return buffer.Length;
        }

        public List<byte> getBuffer() {
            return buffer.ToList();
        }

        public int getPosition() {
            return position;
        }

        public void skipBytes(int v) {
            position += v;
        }

        public void setPosition(int v) {
            position = v;
        }

        public override string ToString()
        {
            return $"Pos: {position}, packet: {buffer.Select(i => $"{i:X2}").Aggregate("", (acc, s) => $"{acc}, 0x{s}")}";
        }
    }
}
