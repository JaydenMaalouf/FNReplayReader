using System;
using System.Collections.Generic;

using FNReplayReader.Enums;

namespace FNReplayReader
{
    public class Player
    {
        public Elimination Eliminator { get; internal set; }
        public Elimination LastDowner { get; internal set; }
        
        //public readonly string Username;
        public readonly Guid PlayerId;

        public uint Placement { get; internal set; } = 100;
        public int Kills => _eliminations.Count;
        public bool Eliminated => Eliminator != null;
        public int TimesDowned => _downs.Count;
        public int TimesDownedEnemy => _enemyDowns.Count;
        
        private readonly List<Elimination> _eliminations = new List<Elimination>();        
        private readonly List<Elimination> _enemyDowns = new List<Elimination>();
        private readonly List<Elimination> _downs = new List<Elimination>();

        internal Player(Guid playerId)
        {
            PlayerId = playerId;
        }

        internal void SetPlacement(uint placement)
        {
            Placement = placement;
        }

        internal Elimination SetElimatedBy(Player eliminatedBy, DamageType damageType)
        {
            Eliminator = new Elimination(eliminatedBy, damageType);
            return Eliminator;
        }

        internal Elimination SetDownedBy(Player downedBy, DamageType damageType)
        {
            LastDowner = new Elimination(downedBy, damageType);
            _downs.Add(LastDowner);
            return LastDowner;
        }

        internal void EliminatedPlayer(Player eliminatedPlayer, DamageType damageType)
        {
            eliminatedPlayer.SetElimatedBy(this, damageType);
            _eliminations.Add(new Elimination(eliminatedPlayer, damageType));
        }

        internal void DownedPlayer(Player downedPlayer, DamageType damageType)
        {
            downedPlayer.SetDownedBy(this, damageType);
            _enemyDowns.Add(new Elimination(downedPlayer, damageType));
        }

        public IEnumerable<Elimination> GetEliminations()
        {
            return _eliminations;
        }

        public IEnumerable<Elimination> GetEnemiesDowned()
        {
            return _enemyDowns;
        }

        public IEnumerable<Elimination> GetDowns()
        {
            return _downs;
        }
    }
}