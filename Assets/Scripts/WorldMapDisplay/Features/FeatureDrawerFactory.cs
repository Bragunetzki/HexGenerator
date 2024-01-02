using HexMapGeneration;

namespace WorldMapDisplay.Features
{
    public static class FeatureDrawerFactory
    {
        public static IHexFeatureDrawer GetFeatureDrawer(HexFeature feature)
        {
            return feature switch
            {
                HexFeature.Forest => new ForestFeatureDrawer(),
                _ => null
            };
        }
    }
}