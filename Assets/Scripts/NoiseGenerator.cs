using UnityEngine;

public static class NoiseGenerator
{
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale)
        {
                if (scale <= 0)
                {
                        scale = 0.0001f;
                }
                
                var noiseMap = new float [mapHeight, mapWidth];

                for (var y = 0; y < mapHeight; y++)
                {
                        for (var x = 0; x < mapWidth; x++)
                        {
                                var sampleX = x / scale;
                                var sampleY = y / scale;
                                var perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                                noiseMap[y, x] = perlinValue;
                        }
                }

                return noiseMap;
        }
}