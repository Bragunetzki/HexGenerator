using System;
using WorldGeneration.Creatures;
using WorldGeneration.History.Event;
using WorldGeneration.History.EventBuilder;

namespace WorldGeneration.Sites
{
    public class SiteHistoryGenerator
    {
        private readonly WorldGenSettings _settings;

        public SiteHistoryGenerator(WorldGenSettings settings)
        {
            _settings = settings;
        }

        public void Generate(Site site, WorldHex hex)
        {
            if (site.InitialPurpose.isNatural)
            {
                var moved = new MovedInHistoryEventBuilder().Make(site, hex, site.Age, _settings);
                moved.Resolve(site);
                moved.Age = site.Age;
                site.History.Add(moved);
            }

            if (site.Age == WorldAge.PreShattering)
            {
                
            }

            var totalEvents = site.Age switch
            {
                WorldAge.PreShattering => 4,
                WorldAge.Ancient => 3,
                WorldAge.Old => 2,
                WorldAge.Young => 1,
                _ => throw new ArgumentOutOfRangeException()
            };

            for (var i = 0; i < totalEvents; i++)
            {
                var age = i switch
                {
                    0 => site.Age,
                    1 => site.Age + 1,
                    2 => site.Age + 2,
                    3 => WorldAge.Young,
                    _ => throw new ArgumentOutOfRangeException()
                };
                var hEvent = GetNewEvent(site, hex, age);
                hEvent.Resolve(site);
                hEvent.Age = age;
                site.History.Add(hEvent);
                
                if (i == totalEvents - 1) continue;

                if (age == WorldAge.PreShattering)
                {
                    var shatteringEvent = new ShatteringEventBuilder().Make(site, hex, site.Age, _settings);
                    shatteringEvent.Resolve(site);
                    shatteringEvent.Age = site.Age;
                    site.History.Add(shatteringEvent);
                }
                
                // wanderer classes don't stay for more than one age.
                site.Inhabitants.RemoveAll(inhabitant => inhabitant.Class.HasTrait(CreatureTrait.Wanderer));
                // individual creatures die after one age, unless they are long lived. 
                site.Inhabitants.RemoveAll(inhabitant =>
                    inhabitant.Type == InhabitantType.PowerfulIndividual &&
                    inhabitant.Species.HasTrait(CreatureTrait.LongLived));
            }
        }

        private IHistoryEvent GetNewEvent(Site site, WorldHex hex, WorldAge age)
        {
            var table = _settings.RollTables.HistoryEventBuilders.Copy();
            table.AddFilter(builder => builder.IsValidFor(site, hex));
            return table.Roll(_settings.Random).Make(site, hex, age, _settings);
        }
    }
}