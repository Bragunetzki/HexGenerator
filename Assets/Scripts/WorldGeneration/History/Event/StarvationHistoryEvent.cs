using System.Collections.Generic;
using System.Linq;
using WorldGeneration.Creatures;
using WorldGeneration.Sites;

namespace WorldGeneration.History.Event
{
    public class StarvationHistoryEvent : IHistoryEvent
    {
        private readonly List<IInhabitant> _survivors = new();

        public void Resolve(Site site)
        {
            site.DeadInhabitants.AddRange(site.Inhabitants.FindAll(DeathCondition));
            site.Inhabitants.RemoveAll(DeathCondition);
            _survivors.AddRange(site.Inhabitants);
        }

        private static bool DeathCondition(IInhabitant inhabitant)
        {
            return !inhabitant.Species.traits.Contains(CreatureTrait.Inorganic) &&
                   !inhabitant.Species.traits.Contains(CreatureTrait.Undead);
        }

        public WorldAge Age { get; set; }
        public string Narration()
        {
            var result = "In the " + Age + " times, the inhabitants faced starvation.";
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