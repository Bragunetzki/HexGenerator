using System;
using UnityEngine;
using Random = System.Random;

namespace WorldGeneration.Creatures
{
    [CreateAssetMenu(fileName = "InhabitantGroup", menuName = "Custom/SiteGen/InhabitantGroup")]
    public class InhabitantPreset : ScriptableObject
    {
        public CreatureSpecies species;
        public int minTier;
        public int maxTier;
        public bool isIntelligent;

        public int GenerateTier(Random random)
        {
            var u1 = 1.0 - random.NextDouble();
            var u2 = 1.0 - random.NextDouble();
            var standardNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            var mean = (minTier + maxTier) / 2.0;
            var standardDeviation = (maxTier - minTier) / 4.0;

            var randomValue = mean + standardNormal * standardDeviation;

            // Ensure the result is within the specified range
            var result = (int)Math.Round(Math.Clamp(randomValue, minTier, maxTier));

            return result;
        } 
    }
}