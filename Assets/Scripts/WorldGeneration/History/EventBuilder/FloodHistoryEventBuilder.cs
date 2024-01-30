using WorldGeneration.History.Event;
using WorldGeneration.Sites;

namespace WorldGeneration.History.EventBuilder
{
    public class FloodHistoryEventBuilder : IHistoryEventBuilder
    {
        public IHistoryEvent Make(Site site, WorldHex hex, WorldAge age, WorldGenSettings settings)
        {
            return new FloodHistoryEvent();
        }

        public bool IsValidFor(Site site, WorldHex hex)
        {
            if (site.HasTrait(SiteTrait.Flooded)) return false;
            return hex.TerrainType is WorldTerrainType.Plains or WorldTerrainType.Beach or WorldTerrainType.Freshwater
                or WorldTerrainType.Sea;
        }
    }
}