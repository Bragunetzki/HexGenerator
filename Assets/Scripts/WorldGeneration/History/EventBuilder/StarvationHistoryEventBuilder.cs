using System.Linq;
using WorldGeneration.Creatures;
using WorldGeneration.History.Event;
using WorldGeneration.Sites;

namespace WorldGeneration.History.EventBuilder
{
    public class StarvationHistoryEventBuilder : IHistoryEventBuilder
    {
        public IHistoryEvent Make(Site site, WorldHex hex, WorldAge age, WorldGenSettings settings)
        {
            return new StarvationHistoryEvent();
        }

        public bool IsValidFor(Site site, WorldHex hex)
        {
            return site.Inhabitants.Any(inhabitant =>
                !(inhabitant.Species.HasTrait(CreatureTrait.Undead) ||
                  inhabitant.Species.HasTrait(CreatureTrait.Inorganic)));
        }
    }
}