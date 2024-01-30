using WorldGeneration.History.Event;
using WorldGeneration.Sites;

namespace WorldGeneration.History.EventBuilder
{
    public class AbandonedHistoryEventBuilder : IHistoryEventBuilder
    {
        public IHistoryEvent Make(Site site, WorldHex hex, WorldAge age, WorldGenSettings settings)
        {
            return new AbandonedHistoryEvent();
        }

        public bool IsValidFor(Site site, WorldHex hex)
        {
            return site.Inhabitants.Count > 0;
        }
    }
}