using System.Collections.Generic;
using System.Linq;
using WorldGeneration.Creatures;
using WorldGeneration.Sites;

namespace WorldGeneration.History.Event
{
    public class FireHistoryEvent : IHistoryEvent
    {
        private readonly List<IInhabitant> _survivors = new();

        public void Resolve(Site site)
        {
            foreach (var inhabitant in site.Inhabitants.Where(DeathCondition))
            {
                site.DeadInhabitants.Add(inhabitant);
            }

            site.Inhabitants.RemoveAll(DeathCondition);
            site.Traits.Add(SiteTrait.Burnt);
            site.Traits.Add(SiteTrait.Wrecked);

            _survivors.AddRange(site.Inhabitants);
        }

        private static bool DeathCondition(IInhabitant inhabitant)
        {
            return !inhabitant.Species.HasTrait(CreatureTrait.Fire);
        }

        public WorldAge Age { get; set; }

        public string Narration()
        {
            var result = "In the " + Age + " times, the place caught fire, leaving it damaged.";
            if (_survivors.Count > 0)
                result += " Only ";
            for (var i = 0; i < _survivors.Count; i++)
            {
                var survivor = _survivors[i];
                result += SiteBuilder.InhabitantString(survivor);
                if (i < _survivors.Count - 1)
                    result += ", ";
            }

            if (_survivors.Count > 0)
                result += " survived.";
            return result;
        }
    }
}