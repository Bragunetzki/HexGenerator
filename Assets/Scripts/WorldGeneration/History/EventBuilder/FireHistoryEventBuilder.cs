using WorldGeneration.History.Event;
using WorldGeneration.Sites;

namespace WorldGeneration.History.EventBuilder
{
    public class FireHistoryEventBuilder : IHistoryEventBuilder
    {
        public IHistoryEvent Make(Site site, WorldHex hex, WorldAge age, WorldGenSettings settings)
        {
            return new FireHistoryEvent();
        }

        public bool IsValidFor(Site site, WorldHex hex)
        {
            return !site.HasTrait(SiteTrait.Flooded);
        }
    }
}