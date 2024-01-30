using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using WorldGeneration.Creatures;
using WorldGeneration.Features;
using WorldGeneration.RollTables;
using WorldGeneration.Sites;
using WorldGeneration.Sites.Purpose;
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
        public RollTableManager RollTables;

        public CreatureSpecies[] rollableSpecies = {};
        public SerializableSpeciesDictionary speciesDictionary;
        public SerializablePurposeDictionary purposeDictionary;
        public CreatureClass[] possibleClasses = {};
        public SitePurpose[] possibleSitePurposes = {};
        public Deity[] worldDeities = { };


        public void InitRandom()
        {
            Random = new Random(seed);
        }

        public readonly List<IHexFeatureGeneration> PossibleNaturalFeatures = new()
        {
            new ForestFeatureGeneration()
        };
    }
    
    [Serializable]
    public class SerializableSpeciesDictionary : SerializableDictionary<string, CreatureSpecies>
    {
    }
    
    [Serializable]
    public class SerializablePurposeDictionary : SerializableDictionary<string, SitePurpose>
    {
    }
}