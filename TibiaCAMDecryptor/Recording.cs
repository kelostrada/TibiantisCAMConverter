﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TibiaCAMDecryptor {
    public class Recording {
        public string FilePath { get; set; }
        public byte[] FileBuffer { get; set; }
        public bool IsValid { get; set; }
        public Location Location { get; internal set; }
        public bool HasProblem { get; internal set; }

        public List<Packet> Packets = new List<Packet>();
        public Recording(string filePath) {
            FilePath = filePath;
            IsValid = true;
            HasProblem = false;
        }

        public void Parse() {
            FileBuffer = File.ReadAllBytes(FilePath);
            BinaryReader br = new BinaryReader(new MemoryStream(FileBuffer));

            br.ReadUInt32();
            br.ReadUInt32();
            br.ReadUInt32();

            int index = 0;

            try {
                while(br.BaseStream.Position < br.BaseStream.Length) {

                    uint packetTime = br.ReadUInt32();

                    // zeroes?
                    var zeroes = br.ReadUInt32();

                    uint packetLength = br.ReadUInt16();

                    if (packetLength == 0)
                    {
                        Console.WriteLine("Invalid packet length! PacketID: " + index + " [" + FilePath + "]");
                        Packets.Clear();
                        IsValid = false;
                        HasProblem = true;
                        break;
                    }

                    // get packet data
                    byte[] packetData = br.ReadBytes((int)packetLength);

                    Packets.Add(new Packet(packetData, packetLength, packetTime));

                    index++;
                }
            } catch (Exception ex) {
                IsValid = false;
                HasProblem = true;
            }

            /*Console.WriteLine("Recording: " + FilePath);
            Console.WriteLine("Status: Done");
            Console.WriteLine("Packets Count: " + Packets.Count);*/
        }

        public void Decrypt(ref byte[] packetData, uint packetTime) {
            Crypt.DecryptPacket(ref packetData, packetTime);
        }
    }
}