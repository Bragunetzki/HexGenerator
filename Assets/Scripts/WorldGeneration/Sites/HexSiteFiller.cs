using Hexes;

namespace WorldGeneration.Sites
{
    public class HexSiteFiller
    {

        public void GenerateSites(HexGrid grid, WorldGenSettings genSettings)
        {
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

            var builder = new SiteBuilder();
            builder.BuildSite(hex, settings);
        }

        
    }
}