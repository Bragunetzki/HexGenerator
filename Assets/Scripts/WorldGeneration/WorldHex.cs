using System.Collections.Generic;
using Hexes;
using WorldGeneration.Features;
using WorldGeneration.Sites;

namespace WorldGeneration
{
    public class WorldHex
    {
        public readonly HexCoordinates CoordHolder;
        public WorldTerrainType TerrainType;
        public HexFeature Feature;
        public readonly WorldClimateType Climate;
        public readonly List<Site> Sites;
        public float Height;
        public WorldHex(int q, int r, WorldTerrainType terrain = WorldTerrainType.Plains, WorldClimateType climate = WorldClimateType.Temperate, float height = 0.5f)
        {
            CoordHolder = new HexCoordinates(q, r);
            TerrainType = terrain;
            Height = height;
            Feature = HexFeature.None;
            Climate = climate;
            Sites = new List<Site>();
        }

        public bool IsAquatic()
        {
            return TerrainType is WorldTerrainType.Freshwater or WorldTerrainType.Sea;
        }
    }

    public enum WorldTerrainType
    {
        Plains,
        Desert,
        Sea,
        Freshwater,
        Beach,
        Hills,
        Mountain,
        TallMountains,
        SnowyPeaks
    }

    public enum WorldClimateType
    {
        Temperate,
        Arctic,
        Tropical
    }
}