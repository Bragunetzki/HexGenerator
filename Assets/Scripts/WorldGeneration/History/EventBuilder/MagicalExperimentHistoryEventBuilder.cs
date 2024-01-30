using System.Linq;
using WorldGeneration.Creatures;
using WorldGeneration.History.Event;
using WorldGeneration.Sites;

namespace WorldGeneration.History.EventBuilder
{
    public class MagicalExperimentHistoryEventBuilder : IHistoryEventBuilder
    {
        public IHistoryEvent Make(Site site, WorldHex hex, WorldAge age, WorldGenSettings settings)
        {
            return new MagicalExperimentHistoryEvent(settings.Random);
        }

        public bool IsValidFor(Site site, WorldHex hex)
        {
            return site.Inhabitants.Any(inhabitant => inhabitant.Class.HasTrait(CreatureTrait.Spellcaster));
        }
    }
}