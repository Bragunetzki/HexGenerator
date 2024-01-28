using UnityEngine;

namespace WorldGeneration
{
    [CreateAssetMenu(fileName = "NoiseGenSettings", menuName = "Custom/WorldGen/NoiseGenSettings")]
    public class NoiseGenSettings : ScriptableObject
    {
        public int noiseWidth;
        public int noiseHeight;
        public float noiseScale;
        public int octaves;
        [Range(0, 1)] public float persistance;
        public float lacunarity;
        public Vector2 offset;

        [Tooltip("How many points of noise will be covered by a single hex.")]
        public int hexNoiseResolution;
    }
}