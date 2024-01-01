using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private float noiseScale = 0.437f;
    [SerializeField] private bool isFlatTopped;
    [SerializeField] private Vector2 hexSize;
    private const float HexScaleMult = 0.74f;

    [Tooltip("How many points of noise will be covered by a single hex.")] [SerializeField]
    private int hexNoiseResolution = 1;

    public HexGrid GenerateMap()
    {
        if (hexNoiseResolution <= 0) hexNoiseResolution = 1;

        var grid = new HexGrid(isFlatTopped, hexSize * HexScaleMult, transform.position);


        grid.PopulateRectangle(0, mapHeight - 1, 0, mapWidth - 1);
        var noiseSize = grid.RectangularGridSize(mapWidth, mapHeight, hexSize.normalized * hexNoiseResolution);
        noiseSize *= hexNoiseResolution * 1.1f;

        // generate noise.
        var noiseMap =
            NoiseGenerator.GenerateNoiseMap((int)noiseSize.x, (int)noiseSize.y, noiseScale);

        var hexHeights = new Dictionary<HexCoordinates, float>();
        var pointsInHexes = new Dictionary<HexCoordinates, int>();
        // iterate through the noise map and map values to hexes.
        for (var y = 0; y < (int)noiseSize.y; y++)
        {
            for (var x = 0; x < (int)noiseSize.x; x++)
            {
                var noiseVal = noiseMap[y, x] * 2.5f;

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
            if (worldHex.TerrainType == WorldTerrainType.Sea) worldHex.Height = 0.9f;
        }

        return grid;
    }

    private static WorldTerrainType InterpretTerrainType(float hexHeight)
    {
        return hexHeight switch
        {
            < 1 => WorldTerrainType.Sea,
            < 1.5f => WorldTerrainType.Plains,
            _ => WorldTerrainType.Plains
        };
    }
}