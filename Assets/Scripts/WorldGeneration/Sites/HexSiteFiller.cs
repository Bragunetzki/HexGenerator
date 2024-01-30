using Hexes;

namespace WorldGeneration.Sites
{
    public class HexSiteFiller
    {
        private SiteBuilder _builder = new SiteBuilder();

        public void GenerateSites(HexGrid grid, WorldGenSettings genSettings)
        {
            _builder = new SiteBuilder();
            foreach (var hex in grid.GetAll())
            {
                GenerateSitesForHex(hex, genSettings);
            }
        }

        private void GenerateSitesForHex(WorldHex hex, WorldGenSettings settings)
        {
            var random = settings.Random;
            if (random.NextDouble() >= settings.siteDensity)
                return;

            _builder.BuildSite(hex, settings);
        }

        
    }
}