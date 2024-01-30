using WorldGeneration.Sites;

namespace WorldGeneration.History.Event
{
    public interface IHistoryEvent
    {
        public void Resolve(Site site);
        public WorldAge Age { get; set; }
        public string Narration();
    }
}