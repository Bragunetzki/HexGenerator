using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldGeneration.Creatures;
using WorldGeneration.Features;
using WorldGeneration.Sites;

namespace WorldGeneration.RollTables
{
    [CreateAssetMenu(fileName = "RollTableHolder", menuName = "Custom/RollTable/RollTableHolder")]
    public class RollTableHolder : ScriptableObject
    {
        public RollTable<WorldAge> SiteAges;
        public RollTable<InhabitantType> BuilderTypes;

        private RollTable<CreatureSpecies> _preShatteringBuilders;
        private RollTable<CreatureSpecies> _ancientBuilders;
        private RollTable<CreatureSpecies> _oldBuilders;
        private RollTable<CreatureSpecies> _youngBuilders;

        private RollTable<SiteSize> _lowTierSizes;
        private RollTable<SiteSize> _mediumTierSizes;
        private RollTable<SiteSize> _highTierSizes;
        private RollTable<SiteSize> _epicTierSizes;

        private RollTable<CreatureClass> _individualClasses;
        private RollTable<CreatureClass> _groupClasses;


        public RollTable<int> TierTable;

        private Dictionary<Tuple<WorldAge, WorldTerrainType, HexFeature, WorldClimateType>, RollTable<CreatureSpecies>>
            _existingBuilderTables;
        
        private Dictionary<Tuple<InhabitantType, CreatureSpecies>, RollTable<CreatureClass>> _existingClassTables;

        private RollTable<SitePurpose> _basePurposeTable;
        private Dictionary<CreatureClass, RollTable<SitePurpose>> _existingPurposeTables;

        public void InitBaseTables(WorldGenSettings settings)
        {
            InitAgeTables();
            InitBuilderTypeTables();
            InitBuilderSpeciesTables(settings);
            InitBuilderClassTables(settings);
            InitTierTable();
            InitSizeTables();
            InitPurposeTables(settings);
        }

        private void InitPurposeTables(WorldGenSettings settings)
        {
            _existingPurposeTables = new Dictionary<CreatureClass, RollTable<SitePurpose>>();
            _basePurposeTable = new RollTable<SitePurpose>();

            foreach (var purpose in settings.possibleSitePurposes)
            {
                var weight = RarityToWeight(purpose.baseRarity);
                _basePurposeTable.AddEntry(weight, purpose);
            }
        }

        private void InitSizeTables()
        {
            _lowTierSizes = new RollTable<SiteSize>();
            _mediumTierSizes = new RollTable<SiteSize>();
            _highTierSizes = new RollTable<SiteSize>();
            _epicTierSizes = new RollTable<SiteSize>();

            _lowTierSizes.AddEntry(1, SiteSize.Small);
            _lowTierSizes.AddEntry(1, SiteSize.Average);

            _mediumTierSizes.AddEntry(2, SiteSize.Small);
            _mediumTierSizes.AddEntry(2, SiteSize.Average);
            _mediumTierSizes.AddEntry(1, SiteSize.Large);

            _highTierSizes.AddEntry(2, SiteSize.Small);
            _highTierSizes.AddEntry(2, SiteSize.Average);
            _highTierSizes.AddEntry(1, SiteSize.Large);
            _highTierSizes.AddEntry(1, SiteSize.Huge);

            _epicTierSizes.AddEntry(1, SiteSize.Small);
            _epicTierSizes.AddEntry(2, SiteSize.Average);
            _epicTierSizes.AddEntry(2, SiteSize.Large);
            _epicTierSizes.AddEntry(1, SiteSize.Huge);
        }

        private void InitTierTable()
        {
            TierTable = new RollTable<int>();
            TierTable.AddEntry(500, 1);
            TierTable.AddEntry(400, 2);
            TierTable.AddEntry(300, 3);
            TierTable.AddEntry(200, 4);
            TierTable.AddEntry(160, 5);
            TierTable.AddEntry(120, 6);
            TierTable.AddEntry(100, 7);
            TierTable.AddEntry(60, 8);
            TierTable.AddEntry(40, 9);
            TierTable.AddEntry(40, 10);
            TierTable.AddEntry(15, 11);
            TierTable.AddEntry(12, 12);
            TierTable.AddEntry(9, 13);
            TierTable.AddEntry(6, 14);
            TierTable.AddEntry(5, 15);
            TierTable.AddEntry(4, 16);
            TierTable.AddEntry(3, 17);
            TierTable.AddEntry(2, 18);
            TierTable.AddEntry(1, 19);
            TierTable.AddEntry(1, 20);
        }

        private void InitBuilderSpeciesTables(WorldGenSettings settings)
        {
            _preShatteringBuilders = new RollTable<CreatureSpecies>();
            _ancientBuilders = new RollTable<CreatureSpecies>();
            _oldBuilders = new RollTable<CreatureSpecies>();
            _youngBuilders = new RollTable<CreatureSpecies>();

            _existingBuilderTables =
                new Dictionary<Tuple<WorldAge, WorldTerrainType, HexFeature, WorldClimateType>, RollTable<CreatureSpecies>>();

            foreach (var species in settings.worldSpecies)
            {
                if (!species.isIntelligent) continue;

                var weight = RarityToWeight(species.baseRarity);

                foreach (var age in species.existsIn)
                {
                    switch (age)
                    {
                        case WorldAge.PreShattering:
                            _preShatteringBuilders.AddEntry(weight, species);
                            break;
                        case WorldAge.Ancient:
                            _ancientBuilders.AddEntry(weight, species);
                            break;
                        case WorldAge.Old:
                            _oldBuilders.AddEntry(weight, species);
                            break;
                        case WorldAge.Young:
                            _youngBuilders.AddEntry(weight, species);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private void InitBuilderClassTables(WorldGenSettings settings)
        {
            _existingClassTables = new Dictionary<Tuple<InhabitantType, CreatureSpecies>, RollTable<CreatureClass>>();
            
            foreach (var creatureClass in settings.possibleClasses)
            {
                var weight = RarityToWeight(creatureClass.baseRarity);
                if (creatureClass.canBeSolitary)
                {
                    _individualClasses.AddEntry(weight, creatureClass);
                }

                if (creatureClass.canBeGroup)
                {
                    _groupClasses.AddEntry(weight, creatureClass);
                }
            }
        }
        
        private void InitAgeTables()
        {
            SiteAges = new RollTable<WorldAge>();
            SiteAges.AddEntry(1, WorldAge.PreShattering);
            SiteAges.AddEntry(2, WorldAge.Ancient);
            SiteAges.AddEntry(2, WorldAge.Old);
            SiteAges.AddEntry(1, WorldAge.Young);
        }

        private void InitBuilderTypeTables()
        {
            BuilderTypes = new RollTable<InhabitantType>();
            BuilderTypes.AddEntry(3, InhabitantType.CreatureGroup);
            BuilderTypes.AddEntry(1, InhabitantType.PowerfulIndividual);
        }

        public RollTable<CreatureSpecies> GetBuilderTable(WorldAge age, WorldTerrainType terrain,
            HexFeature feature, WorldClimateType climate)
        {
            if (_existingBuilderTables.TryGetValue(
                    new Tuple<WorldAge, WorldTerrainType, HexFeature, WorldClimateType>(age, terrain, feature, climate),
                    out var table))
            {
                return table;
            }

            table = age switch
            {
                WorldAge.PreShattering => _preShatteringBuilders.Copy(),
                WorldAge.Ancient => _ancientBuilders.Copy(),
                WorldAge.Old => _oldBuilders.Copy(),
                WorldAge.Young => _youngBuilders.Copy(),
                _ => throw new ArgumentOutOfRangeException(nameof(age), age, null)
            };

            table.AddFilter(species => !species.excludedTerrain.Contains(terrain));
            table.AddFilter(species => !species.excludedFeatures.Contains(feature));
            table.AddFilter(species => !species.excludedClimates.Contains(climate));

            foreach (var entry in table.Entries)
            {
                var species = entry.Value;
                if (species.disfavoredTerrain.Contains(terrain) || species.disfavoredFeatures.Contains(feature) ||
                    species.disfavoredClimates.Contains(climate))
                {
                    entry.Weight = GetRarerWeight(species.baseRarity);
                }
                else if (species.favoredTerrain.Contains(terrain) || species.favoredFeatures.Contains(feature) ||
                         species.favoredClimates.Contains(climate))
                {
                    entry.Weight = GetMoreCommonWeight(species.baseRarity);
                }
            }

            _existingBuilderTables.Add(
                new Tuple<WorldAge, WorldTerrainType, HexFeature, WorldClimateType>(age, terrain, feature, climate),
                table);
            return table;
        }

        public RollTable<CreatureClass> GetClassTable(InhabitantType type, CreatureSpecies species)
        {
            if (_existingClassTables.TryGetValue(new Tuple<InhabitantType, CreatureSpecies>(type, species),
                    out var table))
            {
                return table;
            }

            table = type switch
            {
                InhabitantType.CreatureGroup => _groupClasses.Copy(),
                InhabitantType.PowerfulIndividual => _individualClasses.Copy(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            table.AddFilter(creatureClass => !species.excludedClasses.Contains(creatureClass));

            foreach (var entry in table.Entries)
            {
                var creatureClass = entry.Value;
                if (species.disfavoredClasses.Contains(creatureClass))
                {
                    entry.Weight = GetRarerWeight(creatureClass.baseRarity);
                }
                else if (species.favoredClasses.Contains(creatureClass))
                {
                    entry.Weight = GetMoreCommonWeight(creatureClass.baseRarity);
                }
            }

            _existingClassTables.Add(new Tuple<InhabitantType, CreatureSpecies>(type, species), table);
            return table;
        }
        
        public RollTable<SitePurpose> GetSitePurposeTable(CreatureClass builderClass)
        {
            if (_existingPurposeTables.TryGetValue(builderClass,
                    out var table))
            {
                return table;
            }

            table = _basePurposeTable.Copy();

            table.AddFilter(purpose => !builderClass.excludedPurposes.Contains(purpose));

            foreach (var entry in table.Entries)
            {
                var purpose = entry.Value;
                if (builderClass.disfavoredPurposes.Contains(purpose))
                {
                    entry.Weight = GetRarerWeight(purpose.baseRarity);
                }
                else if (builderClass.favoredPurposes.Contains(purpose))
                {
                    entry.Weight = GetMoreCommonWeight(purpose.baseRarity);
                }
            }

            _existingPurposeTables.Add(builderClass, table);
            return table;
        }

        public RollTable<SiteSize> GetSizeTable(TierGroup tierGroup)
        {
            return tierGroup switch
            {
                TierGroup.Low => _lowTierSizes,
                TierGroup.Medium => _mediumTierSizes,
                TierGroup.High => _highTierSizes,
                TierGroup.Epic => _epicTierSizes,
                _ => throw new ArgumentOutOfRangeException(nameof(tierGroup), tierGroup, null)
            };
        }

        private static int RarityToWeight(Rarity rarity)
        {
            var weight = rarity switch
            {
                Rarity.Common => 20,
                Rarity.Uncommon => 8,
                Rarity.Rare => 2,
                Rarity.Exotic => 1,
                _ => throw new ArgumentOutOfRangeException()
            };
            return weight;
        }

        private static int GetRarerWeight(Rarity rarity)
        {
            var newRarity = rarity switch
            {
                Rarity.Common => Rarity.Uncommon,
                Rarity.Uncommon => Rarity.Rare,
                Rarity.Rare => Rarity.Exotic,
                Rarity.Exotic => Rarity.Exotic,
                _ => throw new ArgumentOutOfRangeException()
            };


            return RarityToWeight(newRarity);
        }

        private static int GetMoreCommonWeight(Rarity rarity)
        {
            var newRarity = rarity switch
            {
                Rarity.Common => Rarity.Common,
                Rarity.Uncommon => Rarity.Common,
                Rarity.Rare => Rarity.Uncommon,
                Rarity.Exotic => Rarity.Rare,
                _ => throw new ArgumentOutOfRangeException()
            };

            return RarityToWeight(newRarity);
        }
    }
}