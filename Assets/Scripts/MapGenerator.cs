using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")] [SerializeField]
    private int mapWidth;

    [SerializeField] private int mapHeight;
    [SerializeField] private bool isFlatTopped;
    [SerializeField] private Vector2 hexSize;
    [SerializeField] private TerrainHeight[] regions;

    [Header("Noise Settings")] [SerializeField]
    private float noiseScale = 0.437f;

    [SerializeField] private int octaves = 1;
    [Range(0, 1)] [SerializeField] private float persistance;
    [SerializeField] private float lacunarity;
    [SerializeField] private Vector2 offset;
    [SerializeField] private int seed;

    [Tooltip("How many points of noise will be covered by a single hex.")] [SerializeField]
    private int hexNoiseResolution = 1;

    [Header("Feature settings")] [Range(0, 1)] [SerializeField]
    private float forestDensity = 0.2f;

    private const float HexScaleMult = 0.74f;
    private Random random;

    public HexGrid GenerateMap()
    {
        random = new Random(seed);
        if (hexNoiseResolution <= 0) hexNoiseResolution = 1;

        var grid = new HexGrid(isFlatTopped, hexSize * HexScaleMult, transform.position);


        grid.PopulateRectangle(0, mapHeight - 1, 0, mapWidth - 1);
        var noiseSize = grid.RectangularGridSize(mapWidth, mapHeight, hexSize.normalized * hexNoiseResolution);
        noiseSize *= 1.1f;
        // generate noise.
        var noiseMap =
            NoiseGenerator.GenerateNoiseMap((int)noiseSize.x, (int)noiseSize.y, seed, noiseScale, octaves,
                persistance, lacunarity, offset);

        var hexHeights = new Dictionary<HexCoordinates, float>();
        var pointsInHexes = new Dictionary<HexCoordinates, int>();
        // iterate through the noise map and map values to hexes.
        for (var y = 0; y < (int)noiseSize.y; y++)
        {
            for (var x = 0; x < (int)noiseSize.x; x++)
            {
                var noiseVal = noiseMap[y, x];

                // convert the x and y coordinates of the noise map to x and y coordinates on the hex grid.
                var gridOrigin = grid.Origin;
                var noise2DCoordinates = new Vector2(gridOrigin.x + x * hexSize.x / hexNoiseResolution,
                    gridOrigin.z + y * hexSize.y / hexNoiseResolution);
                var hexCoords = grid.HexLayout.PixelToHex(noise2DCoordinates).HexRound();

                // accumulate the heights.
                if (!hexHeights.ContainsKey(hexCoords))
                {
                    hexHeights[hexCoords] = noiseVal;
                    pointsInHexes[hexCoords] = 1;
                }
                else
                {
                    hexHeights[hexCoords] += noiseVal;
                    pointsInHexes[hexCoords] += 1;
                }
            }
        }

        // calculate average heights and set them.
        foreach (var coords in hexHeights.Keys.ToList())
        {
            hexHeights[coords] /= pointsInHexes[coords];
            var worldHex = grid.GetHex(coords);

            if (worldHex == null) continue;
            worldHex.Height = hexHeights[coords];
            worldHex.TerrainType = InterpretTerrainType(worldHex.Height);
            worldHex.Features = GenerateHexFeatures(worldHex);
            if (worldHex.TerrainType == WorldTerrainType.Sea) worldHex.Height = 0.39f;
        }

        return grid;
    }

    private List<HexFeature> GenerateHexFeatures(WorldHex worldHex)
    {
        var features = new List<HexFeature>();
        if (worldHex.TerrainType is not (WorldTerrainType.Plains or WorldTerrainType.Hills)) return features;
        var roll = random.NextDouble();
        if (roll <= forestDensity)
        {
            features.Add(HexFeature.Forest);
        }

        return features;
    }

    private WorldTerrainType InterpretTerrainType(float hexHeight)
    {
        for (var i = 0; i < regions.Length; i++)
        {
            if (hexHeight <= regions[i].height)
            {
                return regions[i].terrainType;
            }
        }

        return WorldTerrainType.Plains;
    }

    private void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }

        if (mapHeight < 1)
        {
            mapHeight = 1;
        }

        if (lacunarity < 1)
        {
            lacunarity = 1;
        }

        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}

[Serializable]
public struct TerrainHeight
{
    public float height;
    public WorldTerrainType terrainType;
}