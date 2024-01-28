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
        public bool isSolitary;
        public Rarity baseRarity;

        [Header("Classes")]
        public CreatureClass[] favoredClasses;
        public CreatureClass[] disfavoredClasses;
        public CreatureClass[] excludedClasses;

        [Header("Terrains")]
        public WorldTerrainType[] favoredTerrain;
        public WorldTerrainType[] disfavoredTerrain;
        public WorldTerrainType[] excludedTerrain;

        [Header("Climates")]
        public WorldClimateType[] favoredClimates;
        public WorldClimateType[] disfavoredClimates;
        public WorldClimateType[] excludedClimates;
        
        [Header("Features")]
        public HexFeature[] favoredFeatures;
        public HexFeature[] disfavoredFeatures;
        public HexFeature[] excludedFeatures;

        [Header("Tier")]
        public int minimumTier;
    }
}