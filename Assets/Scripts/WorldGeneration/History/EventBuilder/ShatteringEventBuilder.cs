using WorldGeneration.Creatures;
using WorldGeneration.History.Event;
using WorldGeneration.Sites;

namespace WorldGeneration.History.EventBuilder
{
    public class ShatteringEventBuilder : IHistoryEventBuilder
    {
        public IHistoryEvent Make(Site site, WorldHex hex, WorldAge age, WorldGenSettings settings)
        {
            var automatons = CreatureBuilder.GenerateCreatureOfSpecies( settings.speciesDictionary["automaton"], settings);
            return new ShatteringHistoryEvent(automatons);
        }

        public bool IsValidFor(Site site, WorldHex hex)
        {
            return site.Age == WorldAge.PreShattering;
        }
    }
}