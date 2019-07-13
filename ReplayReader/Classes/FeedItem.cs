using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using FNReplayReader.Enums;

namespace FNReplayReader
{
    public class FeedItem
    {
        public bool IsElimination { get; internal set; } = false;
        public Player Eliminator { get; internal set; }
        public Player Eliminated { get; internal set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DamageType DamageType { get; internal set; }
    }
}