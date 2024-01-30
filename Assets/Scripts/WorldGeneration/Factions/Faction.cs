using System.Collections.Generic;
using Hexes;
using WorldGeneration.History;
using WorldGeneration.History.Event;

namespace WorldGeneration.Factions
{
    public class Faction
    {
        public string Name { get; private set; }
        public int Tier { get; private set; }

        public int TechLevel { get; private set; }

        public List<HexCoordinates> Territory { get; private set; }

        public HashSet<FactionTrait> Traits { get; private set; }
        
        public List<IHistoryEvent> History { get; private set; }

        //public List<Role> Roles { get; private set; }
        
        public Dictionary<Faction, List<DiplomaticState>> DiplomacyStates { get; private set; }

        public Dictionary<Faction, int> Relationships { get; private set; }
        
        public int Authority { get; private set; } 

        public Faction(string name, int tier)
        {
            Name = name;
            Tier = tier;
        }
    }
}