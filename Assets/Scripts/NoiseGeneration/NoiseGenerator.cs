using UnityEngine;
using WorldGeneration;
using Random = System.Random;

namespace NoiseGeneration
{
    public static class NoiseGenerator
    {
        public static float[,] GenerateNoiseMap(NoiseGenSettings settings, int seed)
        {
            return GenerateNoiseMap(settings.noiseWidth, settings.noiseHeight, seed, settings.noiseScale, settings.octaves,
                settings.persistance, settings.lacunarity, settings.offset);
        }

        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves,
            float persistance,
            float lacunarity, Vector2 offset)
        {
            var prng = new Random(seed);
            var octaveOffsets = new Vector2[octaves];
            for (var i = 0; i < octaves; i++)
            {
                var offsetX = prng.Next(-100000, 100000) + offset.x;
                var offsetY = prng.Next(-100000, 100000) + offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            if (scale <= 0)
            {
                scale = 0.0001f;
            }

            var noiseMap = new float [mapHeight, mapWidth];
            var maxNoise = float.MinValue;
            var minNoise = float.MaxValue;

            var halfWidth = mapWidth / 2f;
            var halfHeight = mapHeight / 2f;

            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    for (var i = 0; i < octaves; i++)
                    {
                        var sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                        var sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;
                        var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;
                        noiseMap[y, x] = perlinValue;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    if (noiseHeight > maxNoise)
                    {
                        maxNoise = noiseHeight;
                    }
                    else if (noiseHeight < minNoise)
                    {
                        minNoise = noiseHeight;
                    }

                    noiseMap[y, x] = noiseHeight;
                }
            }

            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {
                    noiseMap[y, x] = Mathf.InverseLerp(minNoise, maxNoise, noiseMap[y, x]);
                }
            }

            return noiseMap;
        }
    }
}