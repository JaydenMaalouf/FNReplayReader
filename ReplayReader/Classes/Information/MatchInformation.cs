using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

using FNReplayReader.Enums;

namespace FNReplayReader.Classes
{
    public class MatchInformation
    {
        public Guid GameId { get; internal set; } = Guid.Empty;

        public uint TotalPlayers { get; internal set; }
        public uint LengthInMs { get; internal set; }

        public IEnumerable<FeedItem> Feed => _feed;
        private readonly List<FeedItem> _feed = new List<FeedItem>();

        public IEnumerable<Player> Players => _players.Values;
        private readonly ConcurrentDictionary<Guid, Player> _players = new ConcurrentDictionary<Guid, Player>();

        internal void SetGameId(Guid gameId)
        {
            GameId = gameId;
        }

        public IEnumerable<FeedItem> GetEliminations()
        {
            return _feed.Where(x => x.IsElimination);
        }

        public IEnumerable<FeedItem> GetDowns()
        {
            return _feed.Where(x => x.IsElimination == false);
        }

        internal Player AddPlayer(Guid playerId)
        {
            return _players.GetOrAdd(playerId, new Player(playerId));
        }

        public Player GetPlayer(Guid playerId)
        {
            if (_players.TryGetValue(playerId, out var player))
            {
                return player;
            }

            return null;
        }

        internal void AddToFeed(Player eliminated, Player eliminator, DamageType damageType, bool isElimination)
        {
            _feed.Add(new FeedItem
            {
                IsElimination = isElimination,
                Eliminated = eliminated,
                Eliminator = eliminator,
                DamageType = damageType
            });
        }
    }
}