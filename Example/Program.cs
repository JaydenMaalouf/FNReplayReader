using System;
using System.IO;

using FNReplayReader;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var replayFilesFolder = Path.Combine(localAppDataFolder, @"FortniteGame\Saved\Demos");
            if (!Directory.Exists(replayFilesFolder))
            {
                Console.WriteLine("Unable to find replays.");
                Console.WriteLine("Press any key to exit...");
                Console.Read();
                return;
            }

            var replayFiles = Directory.EnumerateFiles(replayFilesFolder, "*.replay");
            foreach (var replayFile in replayFiles)
            {
                var replayReader = new ReplayReader(replayFile);
                var replayInfo = replayReader.ReadReplayInfo();

                Console.WriteLine($"Game Id: {replayInfo.MatchInformation.GameId}");

                Console.WriteLine($"Name: {replayInfo.ReplayName}");
                Console.WriteLine($"Date: {replayInfo.Timestamp:dd/MM/yyyy HH:mm:ss}");
                Console.WriteLine($"Total time: {replayInfo.MatchInformation.LengthInMs.ToString()}");
                Console.WriteLine($"Eliminations: {replayInfo.PlayerInformation.Eliminations}");
                Console.WriteLine($"Position: {replayInfo.PlayerInformation.Placement}");
                Console.WriteLine($"Total players: {replayInfo.MatchInformation.TotalPlayers}");

                foreach (var elimination in replayInfo.MatchInformation.Feed)
                {
                    Console.WriteLine($"Player {elimination.Eliminated.PlayerId} was {(elimination.IsElimination ? "eliminated" : "downed")} by {elimination.Eliminator.PlayerId} with {elimination.DamageType}");
                }

                foreach (var player in replayInfo.MatchInformation.Players)
                {
                    //player.
                }
            }
            Console.ReadLine();
        }
    }
}
