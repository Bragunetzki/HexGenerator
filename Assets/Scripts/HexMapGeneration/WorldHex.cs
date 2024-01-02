using System.Collections.Generic;
using Hexes;

namespace HexMapGeneration
{
    public class WorldHex
    {
        public readonly HexCoordinates Coords;
        public WorldTerrainType TerrainType;
        public List<HexFeature> Features;
        public float Height;

        public WorldHex(int q, int r, WorldTerrainType terrain = WorldTerrainType.Plains, float height = 0.5f,
            List<HexFeature> features = null)
        {
            Coords = new HexCoordinates(q, r);
            TerrainType = terrain;
            Height = height;
            Features = features;
        }
    }

    public enum WorldTerrainType
    {
        Plains = 1,
        Sea = 2,
        Freshwater = 3,
        Sand = 4,
        Hills = 5,
        Mountain = 6,
        TallMountains = 7,
        Snow = 8
    }

    public enum HexFeature
    {
        Forest = 1
    }
}