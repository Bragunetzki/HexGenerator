using System.Linq;
using UnityEngine;
using WorldGeneration.Features;
using WorldGeneration.Sites;

namespace WorldGeneration.Creatures
{
    [CreateAssetMenu(fileName = "Species", menuName = "Custom/Creatures/CreatureSpecies")]
    public class CreatureSpecies : ScriptableObject
    {
        public bool isIntelligent;
        public WorldAge[] existsIn;
        public Rarity baseRarity = Rarity.Common;
        public bool isAlwaysSolitary;
        public bool isAlwaysGroup;
        public CreatureTrait[] traits;

        [Header("Classes")] public CreatureClass uniqueClass;
        public bool hasUniqueClass;
        public CreatureClass[] favoredClasses;
        public CreatureClass[] disfavoredClasses;
        public CreatureClass[] excludedClasses;

        [Header("Terrains")] public WorldTerrainType[] favoredTerrain;
        public WorldTerrainType[] disfavoredTerrain;
        public WorldTerrainType[] excludedTerrain = { WorldTerrainType.Freshwater, WorldTerrainType.Sea };

        [Header("Climates")] public WorldClimateType[] favoredClimates;
        public WorldClimateType[] disfavoredClimates;
        public WorldClimateType[] excludedClimates;

        [Header("Features")] public HexFeature[] favoredFeatures;
        public HexFeature[] disfavoredFeatures;
        public HexFeature[] excludedFeatures;

        [Header("Deities")] public Deity[] favoredDeities;
        public Deity[] excludedDeities;

        [Header("Tier")] public int minimumTier = 1;
        
        public bool HasTrait(CreatureTrait trait)
        {
            return traits.Contains(trait);
        }
    }
}