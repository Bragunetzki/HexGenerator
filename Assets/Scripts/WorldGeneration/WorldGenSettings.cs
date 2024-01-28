using System.Collections.Generic;
using UnityEngine;
using WorldGeneration.Creatures;
using WorldGeneration.Features;
using WorldGeneration.RollTables;
using WorldGeneration.Sites;
using Random = System.Random;

namespace WorldGeneration
{
    [CreateAssetMenu(fileName = "WorldGenSettings", menuName = "Custom/WorldGen/WorldGenSettings")]
    public class WorldGenSettings : ScriptableObject
    {
        public int mapWidth;
        public int mapHeight;
        [SerializeField] [Range(0, 1)] public float forestation;
        public int seed;
        public Random Random;
        [SerializeField] [Range(0, 1)] public float siteDensity;
        public bool isFlatTopped;
        public Vector2 hexSize;
        public NoiseGenSettings noiseGenSettings;
        public TerrainHeight[] regions;
        public RollTableHolder rollTables;

        public CreatureSpecies[] worldSpecies;
        public CreatureClass[] possibleClasses;
        public SitePurpose[] possibleSitePurposes;

        public void InitRandom()
        {
            Random = new Random(seed);
        }
        
        public readonly List<IHexFeatureGeneration> PossibleNaturalFeatures = new()
        {
            new ForestFeatureGeneration()
        };
    }
}