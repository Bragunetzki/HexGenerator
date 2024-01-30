using System;
using WorldGeneration.Sites;

namespace WorldGeneration.History.Event
{
    public class MagicalExperimentHistoryEvent : IHistoryEvent
    {
        private readonly Random _random;
        private SiteTrait _addedTrait;

        public MagicalExperimentHistoryEvent(Random random)
        {
            _random = random;
        }

        public void Resolve(Site site)
        {
            site.DeadInhabitants.AddRange(site.Inhabitants);
            site.Inhabitants.Clear();
            var choice = _random.Next(1, 3);
            site.Traits.Add(choice == 0 ? SiteTrait.UnstableAether : SiteTrait.Burnt);
            site.Traits.Add(SiteTrait.Wrecked);
        }

        public WorldAge Age { get; set; }

        public string Narration()
        {
            var result = "In the " + Age +
                         " times, the place was ravaged by a magical experiment gone wrong, leaving it " + _addedTrait +
                         " and wrecked";
            return result;
        }
    }
}