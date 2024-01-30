using System;
using System.Linq;
using WorldGeneration.Creatures;
using WorldGeneration.Sites;
using WorldGeneration.Sites.Purpose;

namespace WorldGeneration.History.Event
{
    public class UndeadRiseHistoryEvent : IHistoryEvent
    {
        private readonly IInhabitant _undead;
        private readonly Random _random;
        private readonly SitePurpose _newPurpose;
        private bool _victory;
        

        public UndeadRiseHistoryEvent(IInhabitant undead, SitePurpose newPurpose, Random random)
        {
            _undead = undead;
            _random = random;
            _newPurpose = newPurpose;
        }
        
        public void Resolve(Site site)
        {
            var roll = _random.Next(1, 21) + _undead.Tier;
            var victory = true;
            foreach (var _ in site.Inhabitants.Where(inhabitant => roll < 10 + inhabitant.Tier * 2))
            {
                victory = false;
            }

            _victory = victory;

            if (!victory) return;
            
            site.Inhabitants.Clear();
            site.Inhabitants.Add(_undead);
            site.DeadInhabitants.Clear();
            site.CurrentPurpose = _newPurpose;
        }

        public WorldAge Age { get; set; }
        public string Narration()
        {
            var result = "In the " + Age + " times, the dead " + "of the place arose as undead.";
            result += " They were " + SiteBuilder.InhabitantString(_undead);
            result += _victory switch
            {
                true => ". They killed the inhabitants and now used the site as a " + SiteBuilder.PurposeString(_newPurpose),
                false => ". The undead invasion was repelled."
            };

            return result;
        }
    }
}