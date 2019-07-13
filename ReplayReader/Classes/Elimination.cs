using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using FNReplayReader.Enums;

namespace FNReplayReader
{
    public class Elimination
    {
        public Player Player { get; internal set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DamageType DamageType { get; internal set; }

        public Elimination(Player player, DamageType damageType)
        {
            Player = player;
            DamageType = damageType;
        }
    }
}