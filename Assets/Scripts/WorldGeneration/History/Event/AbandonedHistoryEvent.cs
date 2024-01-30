using WorldGeneration.Sites;

namespace WorldGeneration.History.Event
{
    public class AbandonedHistoryEvent : IHistoryEvent
    {
        public void Resolve(Site site)
        {
            site.Inhabitants.Clear();
        }

        public WorldAge Age { get; set; }
        
        public string Narration()
        {
            return "In the " + Age + " times, the place was abandoned.";
        }
    }
}