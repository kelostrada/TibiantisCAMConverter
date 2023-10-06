using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Security.Authentication;
using System.Net;
using Newtonsoft.Json;
using System.Threading;

namespace TibiaCAMDecryptor {
    class Program {
        static void Main(string[] args) {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Title = "Tibia CAM Decryptor";
            Console.Clear();


            // dat
            Console.WriteLine("Loading files...");

            Instance.otItems = new OtItems();
            Instance.otItems.Load("items.otb");
            ProtocolGame.map = new OtMap(Instance.otItems);
            Instance.Map.Updated += ProtocolGame.Map_Updated;

            Instance.items = new Items();
            Instance.items.Load("Tibia.dat", 760);

            //Console.WriteLine("Input api key:");
            //var apiKey = Console.ReadLine();
            var apiKey = "<apikey>";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            HttpClient client = new HttpClient { BaseAddress = new Uri("https://tibiantis.net")};
            FetchedRecording lastFetchedRecording = null;

            Console.WriteLine("Preparing a map...");

            var r = client.GetAsync($"map.php");
            r.Wait();
            var rc = r.Result.Content.ReadAsByteArrayAsync();
            rc.Wait();

            ProtocolGame.FillMap(rc.Result);

            Console.WriteLine("Done preparing... saving map...");
            ProtocolGame.map.Save(Environment.CurrentDirectory + "/test.otbm");

            while (true)
            {
                Thread.Sleep(1000);

                var response = client.GetAsync($"fetch_recording.php?key={apiKey}");
                response.Wait();
                var responseContent = response.Result.Content.ReadAsStringAsync();
                responseContent.Wait();
                // Console.WriteLine(responseContent.Result);

                var fetchedRecording = JsonConvert.DeserializeObject<FetchedRecording>(responseContent.Result);

                if (fetchedRecording.id == -1)
                {
                    if (lastFetchedRecording == null || lastFetchedRecording.id != -1)
                    {
                        Console.WriteLine("Waiting for new recordings...!");
                    }

                    lastFetchedRecording = fetchedRecording;

                    continue;
                }

                lastFetchedRecording = fetchedRecording;

                var filePath = $"Recordings\\{fetchedRecording.stored_file}";

                if (!File.Exists(filePath))
                {
                    response = client.GetAsync($"fetch_recording_file.php?key={apiKey}&recording_id={fetchedRecording.id}");
                    response.Wait();

                    using (var fs = new FileStream(filePath, FileMode.CreateNew))
                    {
                        var copyResult = response.Result.Content.CopyToAsync(fs);
                        copyResult.Wait();
                    }
                }

                Recording recording = new Recording(filePath);
                recording.Parse();

                if (recording.HasProblem)
                {
                    Console.WriteLine($"Recording {fetchedRecording.id} has a problem. {fetchedRecording.stored_file}");
                    return;
                }

                Player.name = null;
                ProtocolGame.AllTiles = new Dictionary<Location, List<MapItem>>();
                ProtocolGame.map.Clear();
                Instance.Map.Clear();

                foreach (Packet packet in recording.Packets)
                {
                    ProtocolGame.ParsePacket(recording, packet);
                }

                Console.WriteLine($"[{DateTime.UtcNow}]Parsed Recording: player: {Player.name}, File: {recording.FilePath}, tiles: {ProtocolGame.AllTiles.Count}");

                dynamic tiles = new
                {

                    recordingId = fetchedRecording.id,
                    characterName = Player.name,
                    tiles = ProtocolGame.AllTiles.Select(t => new
                    {
                        x = t.Key.X,
                        y = t.Key.Y,
                        z = t.Key.Z,
                        items = t.Value
                    })
                };

                string json = JsonConvert.SerializeObject(tiles);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                response = client.PostAsync($"api.php?key={apiKey}", httpContent);
                response.Wait();
                responseContent = response.Result.Content.ReadAsStringAsync();
                responseContent.Wait();
                // Console.WriteLine(responseContent.Result);

                var insertResult = JsonConvert.DeserializeObject<InsertResult>(responseContent.Result);

                if (insertResult.result != ProtocolGame.AllTiles.Count)
                {
                    Console.WriteLine("There was some problem inserting tiles to database! {fetchedRecording.id}");
                    return;
                }

            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
