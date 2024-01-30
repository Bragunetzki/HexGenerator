using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using WorldGeneration.Creatures;
using WorldGeneration.Sites;

namespace WorldGeneration.History.Event
{
    public class PlagueHistoryEvent : IHistoryEvent
    {
        private readonly List<IInhabitant> _survivors = new();

        public void Resolve(Site site)
        {
            site.DeadInhabitants.AddRange(site.Inhabitants.FindAll(DeathCondition));
            site.Inhabitants.RemoveAll(DeathCondition);
            
            _survivors.AddRange(site.Inhabitants);
        }

        public WorldAge Age { get; set; }

        private static bool DeathCondition(IInhabitant inhabitant)
        {
            return !inhabitant.Species.traits.Contains(CreatureTrait.Inorganic) &&
                   !inhabitant.Species.traits.Contains(CreatureTrait.Undead);
        }

        public string Narration()
        {
            var result = "In the " + Age + " times, the place was ravaged by plague.";
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