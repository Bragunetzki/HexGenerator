using System.Collections.Generic;
using WorldGeneration.Creatures;
using WorldGeneration.Factions;
using WorldGeneration.History.Event;
using WorldGeneration.Sites.Purpose;

namespace WorldGeneration.Sites
{
    public class Site
    {
        public Faction ControlledBy;
        public WorldAge Age;
        public SiteSize Size;
        public SitePurpose InitialPurpose;
        public SitePurpose CurrentPurpose;
        public readonly List<IInhabitant> Inhabitants;
        public readonly List<IInhabitant> DeadInhabitants;
        public readonly List<IHistoryEvent> History;
        public readonly HashSet<SiteTrait> Traits;

        public Site()
        {
            Inhabitants = new List<IInhabitant>();
            DeadInhabitants = new List<IInhabitant>();
            History = new List<IHistoryEvent>();
            Traits = new HashSet<SiteTrait>();
        }

        public bool HasTrait(SiteTrait trait)
        {
            return Traits.Contains(trait);
        }
    }
}