using System.Collections.Generic;
using WorldGeneration.Factions;

namespace WorldGeneration.Sites
{
    public class Site
    {
        public Faction ControlledBy;
        public WorldAge Age;
        public SiteSize Size;
        public SitePurpose InitialPurpose;
        public SiteType Type;
        public List<Creatures.ICreature> Inhabitants;
    }
}