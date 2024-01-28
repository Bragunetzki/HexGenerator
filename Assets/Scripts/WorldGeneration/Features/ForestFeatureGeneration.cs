using WorldMapDisplay.Features;

namespace WorldGeneration.Features
{
    public class ForestFeatureGeneration : IHexFeatureGeneration
    {
        public bool CanGenerate(WorldHex hex, WorldGenSettings genSettings)
        {
            if (hex.TerrainType is not (WorldTerrainType.Hills or WorldTerrainType.Plains))
                return false;
            var roll = genSettings.Random.NextDouble();
            return roll <= genSettings.forestation;
        }

        public void Generate(WorldHex hex)
        {
            hex.Feature = HexFeature.Forest;
        }

        public IHexFeatureDrawer GetDrawer()
        {
            return new ForestFeatureDrawer();
        }
    }
}