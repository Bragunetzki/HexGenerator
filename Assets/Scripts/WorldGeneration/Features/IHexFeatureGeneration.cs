using WorldMapDisplay.Features;

namespace WorldGeneration.Features
{
    public interface IHexFeatureGeneration
    {
        public bool CanGenerate(WorldHex hex, WorldGenSettings genSettings);

        public void Generate(WorldHex hex);

        public IHexFeatureDrawer GetDrawer();
    }
}