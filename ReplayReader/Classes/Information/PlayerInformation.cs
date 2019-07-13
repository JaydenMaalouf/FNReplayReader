using System;
using System.Collections.Generic;
using System.Text;

namespace FNReplayReader.Classes
{
    public class PlayerInformation
    {
        public uint Placement { get; internal set; }
        public float Accuracy { get; internal set; }
        public uint Assists { get; internal set; }
        public uint Eliminations { get; internal set; }
        public uint WeaponDamageToPlayers { get; internal set; }
        public uint OtherDamageToPlayers { get; internal set; }
        public uint TotalDamageToPlayers => WeaponDamageToPlayers + OtherDamageToPlayers;
        public uint Revives { get; internal set; }
        public uint DamageTaken { get; internal set; }
        public uint DamageToStructures { get; internal set; }
        public uint MaterialsGathered { get; internal set; }
        public uint MaterialsUsed { get; internal set; }
        public uint CentimetersTraveled { get; internal set; }
    }
}
