using WorldGeneration.Creatures;
using WorldGeneration.Sites;
using WorldGeneration.Sites.Purpose;

namespace WorldGeneration.History.Event
{
    public class MovedInHistoryEvent : IHistoryEvent
    {
        private readonly IInhabitant _creature;
        private readonly SitePurpose _newPurpose;
        private bool _repairs;
        
        public MovedInHistoryEvent(IInhabitant whoMovedIn, SitePurpose newPurpose)
        {
            _creature = whoMovedIn;
            _newPurpose = newPurpose;
        }
        
        public void Resolve(Site site)
        {
            site.Inhabitants.Add(_creature);
            if (site.Inhabitants.Count > 1)
                site.CurrentPurpose = _newPurpose;
            
            if (!_creature.Species.isIntelligent) return;
            
            site.Traits.Remove(SiteTrait.Burnt);
            site.Traits.Remove(SiteTrait.Wrecked);
            _repairs = true;
        }

        public WorldAge Age { get; set; }
        public string Narration()
        {
            var result = "In the " + Age + " times, " + SiteBuilder.InhabitantString(_creature) +
                         " moved in. ";
            if (_repairs)
            {
                result += "They did repairs. ";
            }
            result += "They used the site as a " + SiteBuilder.PurposeString(_newPurpose);
            return result;
        }
    }
}