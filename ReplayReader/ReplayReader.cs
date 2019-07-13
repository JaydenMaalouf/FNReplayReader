using System;
using System.IO;

using FNReplayReader.Enums;
using FNReplayReader.Classes;

namespace FNReplayReader
{
    public class ReplayReader
    {
        private const uint FILE_MAGIC = 0x1CA2E27F;
        private const uint NETWORK_MAGIC = 0x2CF5A13D;
        private const uint HEADER_GUID = 11;

        private readonly FileStream _replayStream;

        public ReplayReader(string replayFilePath)
        {
            _replayStream = File.Open(replayFilePath, FileMode.Open, FileAccess.Read);
        }

        public ReplayReader(FileStream replayStream)
        {
            _replayStream = replayStream;
        }

        public ReplayInformation ReadReplayInfo()
        {
            var replayInfo = new ReplayInformation();
            using (BinaryReader reader = new BinaryReader(_replayStream))
            {
                var magicNumber = reader.ReadUInt32();
                if (magicNumber != FILE_MAGIC)
                {
                    throw new InvalidReplayFileException();
                }

                var fileVersion = reader.ReadUInt32();
                replayInfo.MatchInformation.LengthInMs = reader.ReadUInt32();
                var networkVersion = reader.ReadUInt32();
                var changeList = reader.ReadUInt32();
                replayInfo.ReplayName = reader.ReadFString();

                var isLive = reader.ReadUInt32() != 0;

                if (fileVersion >= (uint)VersionHistoryType.HISTORY_RECORDED_TIMESTAMP)
                {
                    replayInfo.Timestamp = new DateTime(reader.ReadInt64());
                }

                if (fileVersion >= (uint)VersionHistoryType.HISTORY_COMPRESSION)
                {
                    var isCompressed = reader.ReadUInt32() != 0;
                }

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    var chunkType = (ChunkType)reader.ReadUInt32();
                    var chunkSizeInBytes = reader.ReadInt32();

                    var offset = reader.BaseStream.Position;

                    switch (chunkType)
                    {
                        case ChunkType.Header:
                            ParseHeaderChunk(reader, ref replayInfo);
                            break;
                        case ChunkType.Checkpoint:
                            ParseCheckpointChunk(reader);
                            break;
                        case ChunkType.ReplayData:
                            ParseReplayData(reader);
                            break;
                        case ChunkType.Event:
                            ParseEventData(reader, ref replayInfo);
                            break;
                    }

                    reader.BaseStream.Seek(offset + chunkSizeInBytes, SeekOrigin.Begin);
                }
            }
            _replayStream.Close();
            return replayInfo;
        }

        #region OLD FUNCTIONS FOR OLDER VERSIONS

        //private void ParseEliminationEvent_OLD(BinaryReader reader, ref ReplayInformation replayInfo)
        //{
        //    reader.SkipBytes(45);
        //    var eliminatedPlayerName = reader.ReadFString();
        //    var eliminatorPlayerName = reader.ReadFString();
        //    var damageType = (DamageType)reader.ReadByte();
        //    var isElimination = reader.ReadByte() == 0;

        //    var eliminated = replayInfo.MatchInformation.AddPlayer(eliminatedPlayerName);
        //    var eliminator = replayInfo.MatchInformation.AddPlayer(eliminatorPlayerName);

        //    if (isElimination)
        //    {
        //        eliminator.EliminatedPlayer(eliminated, damageType);
        //    }
        //    else
        //    {
        //        eliminator.DownedPlayer(eliminated, damageType);
        //    }

        //    replayInfo.MatchInformation.AddToFeed(eliminated, eliminator, damageType, isElimination);
        //}        

        //private void ParseHeaderChunk_OLD(BinaryReader reader, ref FNReplayReader.Classes.ReplayInformation replayInfo)
        //{
        //    //reader.SkipBytes(21); // 6 + 13 in 3 man squads

        //    reader.SkipBytes(4);
        //    var headerVersion = reader.ReadUInt32();
        //    var serverSideVersion = reader.ReadUInt32();
        //    var season = reader.ReadUInt32();
        //    reader.SkipBytes(4);
        //    if (headerVersion > 11)
        //    {
        //        var guid = reader.ReadBytes(16);
        //        replayInfo.MatchInformation.SetGameId(new Guid(guid));
        //    }
        //    reader.SkipBytes(6);
        //    var fortniteVersion = reader.ReadUInt32();
        //    var release = reader.ReadFString();
        //    if (reader.ReadBoolean())
        //    {
        //        var map = reader.ReadFString();
        //    }
        //    reader.SkipBytes(8);
        //    //if (reader.ReadBoolean())
        //    //{
        //    //    var subGame = reader.ReadFString();
        //    //}
        //}

        #endregion

        private void ParseCheckpointChunk(BinaryReader reader)
        {
            var checkpointId = reader.ReadFString();
            var checkpoint = reader.ReadFString();
            var something0 = reader.ReadUInt32(); // 2 in full party squads + playground
            reader.SkipBytes(26);
            //var r = reader.ReadFString();
            // checkpoint0 33 00 6A EA 00 00 6A EA 00 00 E4 E2 01 00 A4 33 05 00 DC E2 01 00 8C 02 07 64 F4 19 9B 03 00 A1 3F 29 C7 00 00 00
            // checkpoint1 36 00 EC D4 01 00 EC D4 01 00 44 7C 02 00 10 9C 07 00 3C 7C 02 00 8C 02 07 64 F4 AA B3 05 00 A1 3F 29 7E 01 00 00
        }

        private void ParseHeaderChunk(BinaryReader reader, ref FNReplayReader.Classes.ReplayInformation replayInfo)
        {
            var magic = reader.ReadUInt32();
            if (magic != NETWORK_MAGIC)
            {
                throw new Exception("Cannot parse header.");
            }
            var fileVersion = reader.ReadUInt32();
            var lengthInMs = reader.ReadUInt32();
            var networkVersion = reader.ReadUInt32();
            var networkChecksum = reader.ReadUInt32();
            var engineNetworkVersion = reader.ReadUInt32();
            var gameNetworkProtocol = reader.ReadUInt32();

            if (networkVersion > HEADER_GUID)
            {
                var guid = reader.ReadGuid();
                replayInfo.MatchInformation.SetGameId(guid);
            }

            var major = reader.ReadUInt16();
            var minor = reader.ReadUInt16();
            var patch = reader.ReadUInt16();
            var changeList = reader.ReadUInt32();
            var branch = reader.ReadFString();

            //var levelNamesAndTimes = reader.ReadSpecialArray();
            //var flags = reader.ReadUInt32();
            //var gameSpecificData = reader.ReadFStringArray();
            //STILL WIP
        }

        private void ParseReplayData(BinaryReader reader)
        {
            var start = reader.ReadUInt32();
            var end = reader.ReadUInt32(); // number of events?
            var length = reader.ReadUInt32(); // remaining chunksize
            var unknown = reader.ReadUInt32(); // 21 9B 14 00, 85 ED 10 00, BA 01 0E 00
            length = reader.ReadUInt32(); // remaining chunksize
        }

        private void ParseEventData(BinaryReader reader, ref FNReplayReader.Classes.ReplayInformation replayInfo)
        {
            var eventId = reader.ReadFString();
            var eventGroup = reader.ReadFString();
            var eventMetaData = reader.ReadFString();
            var startTime = reader.ReadUInt32();
            var endTime = reader.ReadUInt32();
            var sizeInBytes = reader.ReadUInt32();

            var currentPosition = reader.BaseStream.Position;
            if (eventGroup == "playerElim")
            {
                try
                {
                    ParseEliminationEventData(reader, ref replayInfo);
                }
                catch
                {
                    reader.BaseStream.Position = currentPosition + sizeInBytes;
                }
            }
            if (eventMetaData == "AthenaMatchStats")
            {
                reader.SkipBytes(4);
                replayInfo.PlayerInformation.Accuracy = reader.ReadUInt32();
                replayInfo.PlayerInformation.Assists = reader.ReadUInt32();
                replayInfo.PlayerInformation.Eliminations = reader.ReadUInt32();
                replayInfo.PlayerInformation.WeaponDamageToPlayers = reader.ReadUInt32();
                replayInfo.PlayerInformation.OtherDamageToPlayers = reader.ReadUInt32();
                replayInfo.PlayerInformation.Revives = reader.ReadUInt32();
                replayInfo.PlayerInformation.DamageTaken = reader.ReadUInt32();
                replayInfo.PlayerInformation.DamageToStructures = reader.ReadUInt32();
                replayInfo.PlayerInformation.MaterialsGathered = reader.ReadUInt32();
                replayInfo.PlayerInformation.MaterialsUsed = reader.ReadUInt32();
                replayInfo.PlayerInformation.CentimetersTraveled = reader.ReadUInt32();
            }
            else if (eventMetaData == "AthenaMatchTeamStats")
            {
                reader.SkipBytes(4);
                replayInfo.PlayerInformation.Placement = reader.ReadUInt32();
                replayInfo.MatchInformation.TotalPlayers = reader.ReadUInt32();
            }
        }

        private void ParseEliminationEventData(BinaryReader reader, ref ReplayInformation replayInfo)
        {
            reader.SkipBytes(87);
            var eliminatedId = reader.ReadGuid();
            reader.SkipBytes(2);
            var eliminatorId = reader.ReadGuid();

            var eliminated = replayInfo.MatchInformation.AddPlayer(eliminatedId);
            var eliminator = replayInfo.MatchInformation.AddPlayer(eliminatorId);

            var damageType = (DamageType)reader.ReadByte();
            var isElimination = reader.ReadByte() == 0;

            if (isElimination)
            {
                eliminator.EliminatedPlayer(eliminated, damageType);
            }
            else
            {
                eliminator.DownedPlayer(eliminated, damageType);
            }

            replayInfo.MatchInformation.AddToFeed(eliminated, eliminator, damageType, isElimination);
        }

    }
}