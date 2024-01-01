public class WorldHex
{
    public readonly HexCoordinates Coords;
    public WorldTerrainType TerrainType;
    public float Height;

    public WorldHex(int q, int r, WorldTerrainType terrain = WorldTerrainType.Plains, float height = 1)
    {
        Coords = new HexCoordinates(q, r);
        TerrainType = terrain;
        Height = height;
    }
}

public enum WorldTerrainType
{
    Plains = 1,
    Sea = 2,
    Freshwater = 3,
    Desert = 4,
    Forest = 5,
    Mountain = 6
}