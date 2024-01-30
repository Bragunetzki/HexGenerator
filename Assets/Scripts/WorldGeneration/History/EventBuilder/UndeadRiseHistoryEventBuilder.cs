using Utility;
using WorldGeneration.Creatures;
using WorldGeneration.History.Event;
using WorldGeneration.Sites;
using WorldGeneration.Sites.Purpose;

namespace WorldGeneration.History.EventBuilder
{
    public class UndeadRiseHistoryEventBuilder : IHistoryEventBuilder
    {
        public IHistoryEvent Make(Site site, WorldHex hex, WorldAge age, WorldGenSettings settings)
        {
            var undead = CreatureBuilder.GenerateCreature(hex, age, settings, ConditionOption.DontCare,
                new[] { CreatureTrait.Undead });
            
            var newPurpose = PurposeBuilder.GeneratePurposeForCreature(undead, hex, age, settings);
            return new UndeadRiseHistoryEvent(undead, newPurpose, settings.Random);
        }

        public bool IsValidFor(Site site, WorldHex hex)
        {
            return site.DeadInhabitants.Count > 0;
        }
    }
}