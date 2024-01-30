using Utility;
using WorldGeneration.Creatures;
using WorldGeneration.History.Event;
using WorldGeneration.Sites;
using WorldGeneration.Sites.Purpose;

namespace WorldGeneration.History.EventBuilder
{
    public class InvadedHistoryEventBuilder : IHistoryEventBuilder
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
            return new InvadedHistoryEvent(creature, newPurpose, settings.Random);
        }

        public bool IsValidFor(Site site, WorldHex hex)
        {
            return site.Inhabitants.Count > 0;
        }
    }
}