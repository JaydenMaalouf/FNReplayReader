using System;
using System.Collections.Generic;
using System.Text;

namespace FNReplayReader.Classes
{
    public class ReplayInformation
    {
        public string ReplayName { get; internal set; }
        public DateTime Timestamp { get; internal set; }

        public MatchInformation MatchInformation { get; internal set; } = new MatchInformation();
        public PlayerInformation PlayerInformation { get; internal set; } = new PlayerInformation();
    }
}
