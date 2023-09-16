using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TibiaCAMDecryptor {
    class Program {
        static void Main(string[] args) {
            // dat
            Console.WriteLine("Loading .dat file");

            Instance.otItems = new OtItems();
            Instance.otItems.Load("items.otb");
            ProtocolGame.map = new OtMap(Instance.otItems);
            Instance.Map.Updated += ProtocolGame.Map_Updated;

            Instance.items = new Items();
            Instance.items.Load("Tibia.dat", 760);

            int i = 1;
            //get all recordings
            foreach (string filePath in Directory.GetFiles("Recordings")) {
                if (filePath.EndsWith(".DS_Store")) continue;
                //Console.WriteLine("Parsing record: " + Path.GetFileName(filePath));
                Recording recording = new Recording(filePath);
                recording.Parse();

                if (recording.HasProblem)
                {
                    File.Move(recording.FilePath, Environment.CurrentDirectory + "/bad_recordings/" + Path.GetFileName(recording.FilePath));
                    continue;
                }

                foreach (Packet packet in recording.Packets)
                {
                    ProtocolGame.ParsePacket(recording, packet);
                }

                Console.WriteLine($"Parsed {i++} Recording: player: {Player.name}, File: {recording.FilePath}, tiles: {ProtocolGame.AllTiles[recording.FilePath]?.Count}");
            }

            //recordings.Sort((r1, r2) => {
            //    if (r1.Time.CompareTo("09:10:00") > 0 && r2.Time.CompareTo("09:10:00") > 0)
            //    {
            //        return r1.Time.CompareTo(r2.Time);
            //    }

            //    if (r1.Time.CompareTo("09:10:00") > 0)
            //    {
            //        return -1;
            //    }

            //    if (r2.Time.CompareTo("09:10:00") > 0)
            //    {
            //        return 1;
            //    }

            //    return r1.Time.CompareTo(r2.Time);
            //});

            Console.WriteLine($"Total tiles: {ProtocolGame.AllTiles.Values.Sum(v => v.Count)}");

            Console.WriteLine("Done parsing packets... saving map...");
            ProtocolGame.map.Save(Environment.CurrentDirectory + "/test.otbm");

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
