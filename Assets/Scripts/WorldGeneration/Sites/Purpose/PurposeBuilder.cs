using Utility;
using WorldGeneration.Creatures;

namespace WorldGeneration.Sites.Purpose
{
    public static class PurposeBuilder
    {
        public static SitePurpose GeneratePurposeForCreature(IInhabitant inhabitant, WorldHex hex, WorldAge age,
            WorldGenSettings settings)
        {
            if (!inhabitant.Species.isIntelligent)
                return settings.purposeDictionary["den"];
            
            var tableHolder = settings.RollTables;
            var random = settings.Random;
            var creatureClass = inhabitant.Class;
            var creatureSpecies = inhabitant.Species;
            var purpose = tableHolder.GetSitePurposeTable(creatureClass).Roll(random);
            switch (purpose)
            {
                case TemplePurpose templePurpose:
                {
                    var worshippedDeity = tableHolder.GetDeityTable(creatureSpecies, creatureClass).Roll(random);
                    templePurpose.worshippedDeity = worshippedDeity;
                    break;
                }
                case MinePurpose minePurpose:
                {
                    var minedMineral = tableHolder.Minerals.Roll(random);
                    minePurpose.minedMineral = minedMineral;
                    break;
                }
                case PrisonPurpose prisonPurpose:
                {
                    var prisoner = CreatureBuilder.GenerateCreature(hex, age, settings,
                        ConditionOption.RequireTrue);
                    prisonPurpose.Prisoner = prisoner;
                    break;
                }
            }

            return purpose;
        }
    }
}