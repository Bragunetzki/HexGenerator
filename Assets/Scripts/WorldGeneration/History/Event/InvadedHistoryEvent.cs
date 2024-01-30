using System;
using System.Linq;
using WorldGeneration.Creatures;
using WorldGeneration.Sites;
using WorldGeneration.Sites.Purpose;

namespace WorldGeneration.History.Event
{
    public class InvadedHistoryEvent : IHistoryEvent
    {
        private readonly IInhabitant _invaders;
        private readonly Random _random;
        private readonly SitePurpose _newPurpose;
        private bool _victory;

        public InvadedHistoryEvent(IInhabitant invaders, SitePurpose newPurpose, Random random)
        {
            _invaders = invaders;
            _random = random;
            _newPurpose = newPurpose;
        }

        public void Resolve(Site site)
        {
            var roll = _random.Next(1, 21) + _invaders.Tier;
            var victory = true;
            foreach (var _ in site.Inhabitants.Where(inhabitant => roll < 10 + inhabitant.Tier * 2))
            {
                victory = false;
            }

            _victory = victory;

            if (!victory) return;

            site.DeadInhabitants.AddRange(site.Inhabitants);
            site.Inhabitants.Clear();
            site.Inhabitants.Add(_invaders);
            site.CurrentPurpose = _newPurpose;
        }

        public WorldAge Age { get; set; }

        public string Narration()
        {
            var result = "In the " + Age + " times, the place was invaded by " +
                         SiteBuilder.InhabitantString(_invaders);
            result += _victory switch
            {
                true => ". They were victorious and now used the site as a " + SiteBuilder.PurposeString(_newPurpose),
                false => ". The invasion was repelled."
            };

            return result;
        }
    }
}