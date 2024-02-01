using System;
using System.Collections.Generic;
using System.Linq;
using Hexes;
using NoiseGeneration;
using UnityEngine;

namespace WorldGeneration
{
    public class MapGenerator : MonoBehaviour
    {
        private WorldGenSettings _settings;

        public void SetSettings(WorldGenSettings settings)
        {
            _settings = settings;
        }


        public HexGrid GenerateMap()
        {
            if (_settings is null)
            {
                return null;
            }
            
            var grid = new HexGrid(_settings.isFlatTopped, _settings.hexSize, transform.position);

            grid.PopulateRectangle(0, _settings.mapHeight - 1, 0, _settings.mapWidth - 1);

            grid = GenerateTerrain(grid);

            return grid;
        }

        private HexGrid GenerateTerrain(HexGrid grid)
        {
            var mapWidth = _settings.mapWidth;
            var mapHeight = _settings.mapHeight;
            var hexSize = _settings.hexSize;
            var hexNoiseResolution = _settings.noiseGenSettings.hexNoiseResolution;

            var noiseSize = grid.RectangularGridSize(mapWidth, mapHeight, hexSize.normalized * hexNoiseResolution);
            noiseSize *= 1.1f;
            _settings.noiseGenSettings.noiseWidth = (int)noiseSize.x;
            _settings.noiseGenSettings.noiseHeight = (int)noiseSize.y;

            // generate noise.
            var noiseMap =
                NoiseGenerator.GenerateNoiseMap(_settings.noiseGenSettings, _settings.seed);

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
                    var hexCoords = grid.HexLayout.CoordsToHex(new Vector3(noise2DCoordinates.x, 0, noise2DCoordinates.y)).HexRound();

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
                if (worldHex.TerrainType == WorldTerrainType.Freshwater) worldHex.Height = 0.39f;
            }

            return grid;
        }

        private WorldTerrainType InterpretTerrainType(float hexHeight)
        {
            for (var i = 0; i < _settings.regions.Length; i++)
            {
                if (hexHeight <= _settings.regions[i].height)
                {
                    return _settings.regions[i].terrainType;
                }
            }

            return WorldTerrainType.Plains;
        }
    }

    [Serializable]
    public struct TerrainHeight
    {
        public float height;
        public WorldTerrainType terrainType;
    }
}