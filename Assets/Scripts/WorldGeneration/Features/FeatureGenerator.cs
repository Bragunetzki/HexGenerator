using System.Collections.Generic;
using System.Linq;
using Hexes;
using WorldMapDisplay.Features;

namespace WorldGeneration.Features
{
    public static class FeatureGenerator
    {
        private static Dictionary<HexFeature, IHexFeatureDrawer> _drawers = new()
        {
            { HexFeature.Forest, new ForestFeatureDrawer() }
        };

        public static void GenerateFeatures(HexGrid grid, WorldGenSettings genSettings)
        {
            foreach (var hex in grid.GetAll())
            {
                GenerateFeaturesForHex(hex, genSettings, genSettings.PossibleNaturalFeatures);
            }
        }

        private static void GenerateFeaturesForHex(WorldHex hex, WorldGenSettings worldGenSettings, IEnumerable<IHexFeatureGeneration> featureList)
        {
            foreach (var feature in featureList.Where(feature => feature.CanGenerate(hex, worldGenSettings)))
            {
                feature.Generate(hex);
            }
        }

        public static IHexFeatureDrawer GetDrawer(HexFeature feature)
        {
            return _drawers[feature];
        }
    }
}