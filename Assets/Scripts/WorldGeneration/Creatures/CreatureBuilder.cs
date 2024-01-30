using System;
using System.Linq;
using UnityEngine;
using Utility;
using WorldGeneration.Sites;

namespace WorldGeneration.Creatures
{
    public static class CreatureBuilder
    {
        public static IInhabitant GenerateCreatureOfSpecies(CreatureSpecies species, WorldGenSettings settings)
        {
            var random = settings.Random;
            var tables = settings.RollTables;
            
            InhabitantType builderType;
            if (species.isAlwaysSolitary)
                builderType = InhabitantType.PowerfulIndividual;
            else if (species.isAlwaysGroup)
                builderType = InhabitantType.CreatureGroup;
            else
                builderType = tables.BuilderTypes.Roll(random);

            var builderClass = species.hasUniqueClass
                ? species.uniqueClass
                : tables.GetClassTable(builderType, species).Roll(random);

            var tier = tables.TierTable.Roll(random);
            var minTier = Mathf.Max(species.minimumTier, builderClass.minimumTier);
            tier += minTier - 1;
            if (tier > 20) tier = 20;

            IInhabitant inhabitant = builderType switch
            {
                InhabitantType.PowerfulIndividual => new Inhabitant(tier, species, builderClass, builderType),
                InhabitantType.CreatureGroup => new InhabitantGroup(tier, species, builderClass, builderType),
                _ => throw new ArgumentOutOfRangeException()
            };
            return inhabitant; 
        }
        
        public static IInhabitant GenerateCreature(WorldHex hex, WorldAge age, WorldGenSettings settings,
            ConditionOption requireIntelligent, CreatureTrait[] requiredTraits = null)
        {
            var tables = settings.RollTables;
            var random = settings.Random;
            var speciesTable =
                tables.GetSpeciesTable(age, hex.TerrainType, hex.Feature, hex.Climate, requireIntelligent);
            
            if (requiredTraits != null)
            {
                speciesTable = speciesTable.Copy();
                speciesTable.AddFilter(species => requiredTraits.All(species.traits.Contains));
            }
            var species = speciesTable.Roll(random);

            InhabitantType builderType;
            if (species.isAlwaysSolitary)
                builderType = InhabitantType.PowerfulIndividual;
            else if (species.isAlwaysGroup)
                builderType = InhabitantType.CreatureGroup;
            else
                builderType = tables.BuilderTypes.Roll(random);

            var builderClass = species.hasUniqueClass
                ? species.uniqueClass
                : tables.GetClassTable(builderType, species).Roll(random);

            var tier = tables.TierTable.Roll(random);
            var minTier = Mathf.Max(species.minimumTier, builderClass.minimumTier);
            tier += minTier - 1;
            if (tier > 20) tier = 20;

            IInhabitant inhabitant = builderType switch
            {
                InhabitantType.PowerfulIndividual => new Inhabitant(tier, species, builderClass, builderType),
                InhabitantType.CreatureGroup => new InhabitantGroup(tier, species, builderClass, builderType),
                _ => throw new ArgumentOutOfRangeException()
            };
            return inhabitant;
        }
    }
}