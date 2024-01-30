using System;
using System.Collections.Generic;
using System.Linq;
using Utility;
using WorldGeneration.Creatures;
using WorldGeneration.Features;
using WorldGeneration.History.Event;
using WorldGeneration.History.EventBuilder;
using WorldGeneration.Sites;
using WorldGeneration.Sites.Purpose;

namespace WorldGeneration.RollTables
{
    public class RollTableManager
    {
        public RollTable<WorldAge> SiteAges;
        public RollTable<InhabitantType> BuilderTypes;
        public RollTable<Mineral> Minerals;
        public RollTable<IHistoryEventBuilder> HistoryEventBuilders;

        private RollTable<CreatureSpecies> _preShatteringCreatures;
        private RollTable<CreatureSpecies> _ancientCreatures;
        private RollTable<CreatureSpecies> _oldAgeCreatures;
        private RollTable<CreatureSpecies> _youngAgeCreatures;

        private RollTable<SiteSize> _lowTierSizes;
        private RollTable<SiteSize> _mediumTierSizes;
        private RollTable<SiteSize> _highTierSizes;
        private RollTable<SiteSize> _epicTierSizes;

        private RollTable<CreatureClass> _individualClasses;
        private RollTable<CreatureClass> _groupClasses;

        private RollTable<Deity> _deities;
        public RollTable<int> TierTable;
        
        private RollTable<SitePurpose> _basePurposeTable;
        private RollTable<SitePurpose> _naturalPurposeTable;

        private Dictionary<Tuple<WorldAge, WorldTerrainType, HexFeature, WorldClimateType, ConditionOption>, RollTable<CreatureSpecies>>
            _existingCreatureTables;
        private Dictionary<Tuple<InhabitantType, CreatureSpecies>, RollTable<CreatureClass>> _existingClassTables;
        private Dictionary<CreatureClass, RollTable<SitePurpose>> _existingPurposeTables;
        private Dictionary<Tuple<CreatureSpecies, CreatureClass>, RollTable<Deity>> _existingDeityTables;

        public RollTableManager(WorldGenSettings settings)
        {
            InitAgeTables();
            InitBuilderTypeTables();
            InitSpeciesTables(settings);
            InitBuilderClassTables(settings);
            InitTierTable();
            InitSizeTables();
            InitPurposeTables(settings);
            InitDeityTable(settings);
            InitMineralTable();
            InitHistoryEventBuilders();
        }

        private void InitHistoryEventBuilders()
        {
            HistoryEventBuilders = new RollTable<IHistoryEventBuilder>();
            HistoryEventBuilders.AddEntry(1, new MovedInHistoryEventBuilder());
            HistoryEventBuilders.AddEntry(1, new AbandonedHistoryEventBuilder());
            HistoryEventBuilders.AddEntry(1, new FireHistoryEventBuilder());
            HistoryEventBuilders.AddEntry(1, new FloodHistoryEventBuilder());
            HistoryEventBuilders.AddEntry(1, new InvadedHistoryEventBuilder());
            HistoryEventBuilders.AddEntry(1, new MagicalExperimentHistoryEventBuilder());
            HistoryEventBuilders.AddEntry(1, new PlagueHistoryEventBuilder());
            HistoryEventBuilders.AddEntry(1, new StarvationHistoryEventBuilder());
            HistoryEventBuilders.AddEntry(1, new UndeadRiseHistoryEventBuilder());
        }

        private void InitMineralTable()
        {
            Minerals = new RollTable<Mineral>();
            Minerals.AddEntry(4, Mineral.Coal);
            Minerals.AddEntry(4, Mineral.Copper);
            Minerals.AddEntry(4, Mineral.Tin);
            Minerals.AddEntry(4, Mineral.Iron);
            Minerals.AddEntry(2, Mineral.Silver);
            Minerals.AddEntry(1, Mineral.Platinum);
            Minerals.AddEntry(1, Mineral.Gold);
        }

        private void InitDeityTable(WorldGenSettings settings)
        {
            _deities = new RollTable<Deity>();
            _existingDeityTables = new Dictionary<Tuple<CreatureSpecies, CreatureClass>, RollTable<Deity>>();

            foreach (var deity in settings.worldDeities)
            {
                var weight = RarityToWeight(deity.baseRarity);
                _deities.AddEntry(weight, deity);
            }
        }

        private void InitPurposeTables(WorldGenSettings settings)
        {
            _existingPurposeTables = new Dictionary<CreatureClass, RollTable<SitePurpose>>();
            _basePurposeTable = new RollTable<SitePurpose>();
            _naturalPurposeTable = new RollTable<SitePurpose>();

            foreach (var purpose in settings.possibleSitePurposes)
            {
                if (purpose.isNatural)
                {
                    _naturalPurposeTable.AddEntry(RarityToWeight(purpose.baseRarity), purpose);
                }
                else
                {
                    var weight = RarityToWeight(purpose.baseRarity);
                    _basePurposeTable.AddEntry(weight, purpose);
                }
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

            _highTierSizes.AddEntry(1, SiteSize.Small);
            _highTierSizes.AddEntry(2, SiteSize.Average);
            _highTierSizes.AddEntry(2, SiteSize.Large);
            _highTierSizes.AddEntry(1, SiteSize.Huge);

            _epicTierSizes.AddEntry(1, SiteSize.Small);
            _epicTierSizes.AddEntry(1, SiteSize.Average);
            _epicTierSizes.AddEntry(2, SiteSize.Large);
            _epicTierSizes.AddEntry(2, SiteSize.Huge);
        }

        private void InitTierTable()
        {
            TierTable = new RollTable<int>();
            TierTable.AddEntry(12, 1);
            TierTable.AddEntry(11, 2);
            TierTable.AddEntry(10, 3);
            TierTable.AddEntry(9, 4);
            TierTable.AddEntry(8, 5);
            TierTable.AddEntry(7, 6);
            TierTable.AddEntry(6, 7);
            TierTable.AddEntry(5, 8);
            TierTable.AddEntry(4, 9);
            TierTable.AddEntry(3, 10);
            TierTable.AddEntry(3, 11);
            TierTable.AddEntry(3, 12);
            TierTable.AddEntry(3, 13);
            TierTable.AddEntry(3, 14);
            TierTable.AddEntry(3, 15);
            TierTable.AddEntry(3, 16);
            TierTable.AddEntry(3, 17);
            TierTable.AddEntry(2, 18);
            TierTable.AddEntry(1, 19);
            TierTable.AddEntry(1, 20);
        }

        private void InitSpeciesTables(WorldGenSettings settings)
        {
            _preShatteringCreatures = new RollTable<CreatureSpecies>();
            _ancientCreatures = new RollTable<CreatureSpecies>();
            _oldAgeCreatures = new RollTable<CreatureSpecies>();
            _youngAgeCreatures = new RollTable<CreatureSpecies>();

            _existingCreatureTables =
                new Dictionary<Tuple<WorldAge, WorldTerrainType, HexFeature, WorldClimateType, ConditionOption>, RollTable<CreatureSpecies>>();

            foreach (var species in settings.rollableSpecies)
            {

                var weight = RarityToWeight(species.baseRarity);

                foreach (var age in species.existsIn)
                {
                    switch (age)
                    {
                        case WorldAge.PreShattering:
                            _preShatteringCreatures.AddEntry(weight, species);
                            break;
                        case WorldAge.Ancient:
                            _ancientCreatures.AddEntry(weight, species);
                            break;
                        case WorldAge.Old:
                            _oldAgeCreatures.AddEntry(weight, species);
                            break;
                        case WorldAge.Young:
                            _youngAgeCreatures.AddEntry(weight, species);
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
            _individualClasses = new RollTable<CreatureClass>();
            _groupClasses = new RollTable<CreatureClass>();
            
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

        public RollTable<CreatureSpecies> GetSpeciesTable(WorldAge age, WorldTerrainType terrain,
            HexFeature feature, WorldClimateType climate, ConditionOption requireIntelligent)
        {
            if (_existingCreatureTables.TryGetValue(
                    new Tuple<WorldAge, WorldTerrainType, HexFeature, WorldClimateType, ConditionOption>(age, terrain, feature, climate, requireIntelligent),
                    out var table))
            {
                return table;
            }

            table = age switch
            {
                WorldAge.PreShattering => _preShatteringCreatures.Copy(),
                WorldAge.Ancient => _ancientCreatures.Copy(),
                WorldAge.Old => _oldAgeCreatures.Copy(),
                WorldAge.Young => _youngAgeCreatures.Copy(),
                _ => throw new ArgumentOutOfRangeException(nameof(age), age, null)
            };

            table.AddFilter(species => !species.excludedTerrain.Contains(terrain));
            table.AddFilter(species => !species.excludedFeatures.Contains(feature));
            table.AddFilter(species => !species.excludedClimates.Contains(climate));
            switch (requireIntelligent)
            {
                case ConditionOption.RequireTrue:
                    table.AddFilter(species => species.isIntelligent);
                    break;
                case ConditionOption.RequireFalse:
                    table.AddFilter(species => !species.isIntelligent);
                    break;
                case ConditionOption.DontCare:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(requireIntelligent), requireIntelligent, null);
            }

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

            _existingCreatureTables.Add(
                new Tuple<WorldAge, WorldTerrainType, HexFeature, WorldClimateType, ConditionOption>(age, terrain, feature, climate, requireIntelligent),
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
        
        public RollTable<Deity> GetDeityTable(CreatureSpecies species, CreatureClass creatureClass)
        {
            if (_existingDeityTables.TryGetValue(new Tuple<CreatureSpecies, CreatureClass>(species, creatureClass), out var table))
            {
                return table;
            }
            
            table = _deities.Copy();
            table.AddFilter(deity => !species.excludedDeities.Contains(deity));
            table.AddFilter(deity => !creatureClass.excludedDeities.Contains(deity));
            foreach (var entry in table.Entries)
            {
                var deity = entry.Value;
                if (species.favoredDeities.Contains(deity) || creatureClass.favoredDeities.Contains(deity))
                {
                    entry.Weight = GetMoreCommonWeight(deity.baseRarity);
                }
            }

            _existingDeityTables.Add(new Tuple<CreatureSpecies, CreatureClass>(species, creatureClass), table);
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
                Rarity.VeryCommon => 30,
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
                Rarity.Common => Rarity.VeryCommon,
                Rarity.Uncommon => Rarity.Common,
                Rarity.Rare => Rarity.Uncommon,
                Rarity.Exotic => Rarity.Rare,
                _ => throw new ArgumentOutOfRangeException()
            };

            return RarityToWeight(newRarity);
        }

        public RollTable<SitePurpose> GetNaturalSitePurposeTable(WorldHex hex)
        {
            var table = _naturalPurposeTable;
            if (!hex.IsAquatic()) return table;
            table = table.Copy();
            table.AddFilter(purpose => purpose.canBeAquatic);

            return table;
        }

        public static TierGroup TierToTierGroup(int tier)
        {
            var tierGroup = tier switch
            {
                <= 5 => TierGroup.Low,
                <= 10 => TierGroup.Medium,
                <= 15 => TierGroup.High,
                <= 20 => TierGroup.Epic,
                _ => throw new ArgumentOutOfRangeException()
            };
            return tierGroup;
        }
    }
}