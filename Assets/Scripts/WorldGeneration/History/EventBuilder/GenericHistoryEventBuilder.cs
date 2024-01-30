using WorldGeneration.History.Event;
using WorldGeneration.Sites;

namespace WorldGeneration.History.EventBuilder
{
    public class GenericHistoryEventBuilder<T> : IHistoryEventBuilder where T : IHistoryEvent, new()
    {
        public IHistoryEvent Make(Site site, WorldHex hex, WorldAge age, WorldGenSettings settings)
        {
            return new T();
        }

        public bool IsValidFor(Site site, WorldHex hex)
        {
            return true;
        }
    }
}