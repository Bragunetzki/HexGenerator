using System;
using NoiseGeneration;
using UnityEngine;

namespace NoiseMapDisplay
{
    public class PlaneMapCreator : MonoBehaviour
    {
        public enum DrawMode
        {
            NoiseMap,
            ColourMap
        }
        public DrawMode drawMode;

        [SerializeField] private int mapWidth;
        [SerializeField] private int mapHeight;
        [SerializeField] private float noiseScale = 0.437f;
        [SerializeField] private int octaves = 1;
        [Range(0, 1)]
        [SerializeField] private float persistance;
        [SerializeField] private float lacunarity;
        [SerializeField] private Vector2 offset;
        [SerializeField] private int seed;
        public bool autoUpdate = true;
        [SerializeField] private TerrainType[] regions;

        public void GenerateMap()
        {
            var noiseMap = NoiseGenerator.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance,
                lacunarity, offset);

            var colourMap = new Color[mapWidth * mapHeight];
        
            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {
                    var currentHeight = noiseMap[y, x];
                    for (var i = 0; i < regions.Length; i++)
                    {
                        if (!(currentHeight <= regions[i].height)) continue;
                        colourMap[y * mapWidth + x] = regions[i].colour;
                        break;
                    }
                }
            }

            var display = FindObjectOfType<NoiseMapDrawer>();
            switch (drawMode)
            {
                case DrawMode.NoiseMap:
                    display.DrawTexture(NoiseTextureGenerator.TextureFromHeightMap(noiseMap));
                    break;
                case DrawMode.ColourMap:
                    display.DrawTexture(NoiseTextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color colour;
    }
}