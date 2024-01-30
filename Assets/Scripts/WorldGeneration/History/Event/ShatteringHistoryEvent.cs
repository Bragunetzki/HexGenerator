using WorldGeneration.Creatures;
using WorldGeneration.Sites;

namespace WorldGeneration.History.Event
{
    public class ShatteringHistoryEvent : IHistoryEvent
    {
        private readonly IInhabitant _automatons;

        public ShatteringHistoryEvent(IInhabitant automatons)
        {
            _automatons = automatons;
        }

        public void Resolve(Site site)
        {
            site.DeadInhabitants.AddRange(site.Inhabitants);
            site.Inhabitants.Clear();
            if (site.HasTrait(SiteTrait.AncientDarokkian))
                site.Inhabitants.Add(_automatons);
            site.Traits.Add(SiteTrait.UnstableAether);
            site.Traits.Add(SiteTrait.Wrecked);
        }

        public WorldAge Age { get; set; }
        public string Narration()
        {
            var result = "In the " + Age + " times, the Shattering took place. The place was wrecked.";
            return result;
        }
    }
}