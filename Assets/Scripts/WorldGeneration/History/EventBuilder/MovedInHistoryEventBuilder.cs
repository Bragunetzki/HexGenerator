using System;
using System.Linq;
using Utility;
using WorldGeneration.Creatures;
using WorldGeneration.History.Event;
using WorldGeneration.RollTables;
using WorldGeneration.Sites;
using WorldGeneration.Sites.Purpose;

namespace WorldGeneration.History.EventBuilder
{
    public class MovedInHistoryEventBuilder : IHistoryEventBuilder
    {
        public IHistoryEvent Make(Site site, WorldHex hex, WorldAge age, WorldGenSettings settings)
        {
            var creature = site.HasTrait(SiteTrait.Flooded)
                ? CreatureBuilder.GenerateCreature(hex, age, settings, ConditionOption.DontCare, new[]
                {
                    CreatureTrait.Aquatic
                })
                : CreatureBuilder.GenerateCreature(hex, age, settings, ConditionOption.DontCare);

            var newPurpose = PurposeBuilder.GeneratePurposeForCreature(creature, hex, age, settings);

            return new MovedInHistoryEvent(creature, newPurpose);
        }

        public bool IsValidFor(Site site, WorldHex hex)
        {
            var inhabitantSlots = site.Size switch
            {
                SiteSize.Small => 1,
                SiteSize.Average => 1,
                SiteSize.Large => 2,
                SiteSize.Huge => 4,
                _ => throw new ArgumentOutOfRangeException()
            };

            var takenSlots = site.Inhabitants.Select(inhabitant => RollTableManager.TierToTierGroup(inhabitant.Tier))
                .Select(group => group switch
                {
                    TierGroup.Low => 1,
                    TierGroup.Medium => 1,
                    TierGroup.High => 2,
                    TierGroup.Epic => 3,
                    _ => throw new ArgumentOutOfRangeException()
                })
                .Sum();

            return takenSlots < inhabitantSlots;
        }
    }
}