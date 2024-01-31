using UnityEngine;
using WorldGeneration;
using WorldGeneration.Features;
using WorldGeneration.RollTables;
using WorldGeneration.Sites;

namespace WorldMapDisplay
{
    [RequireComponent(typeof(PlaneHexMapDrawer))]
    [RequireComponent(typeof(MapGenerator))]
    public class HexMapCreator : MonoBehaviour
    {
        [SerializeField] private WorldGenSettings settings;
        [SerializeField] private MapGenerator mapGenerator;
        [SerializeField] private PlaneHexMapDrawer hexGridDrawer;
        
        [Header("Feature Prefabs")] [SerializeField]
        private GameObject treePrefab;
        
        private void Awake()
        {
            if (mapGenerator == null)
            {
                mapGenerator = GetComponent<MapGenerator>();
            }

            if (hexGridDrawer == null)
            {
                hexGridDrawer = GetComponent<PlaneHexMapDrawer>();
            }

            if (mapGenerator == null)
            {
                Debug.LogError("mapGenerator not assigned!");
                return;
            }

            if (hexGridDrawer == null)
            {
                Debug.LogError("hexGridDrawer not assigned!");
                return;
            }

            CreateMap();
        }

        public void CreateMap()
        {
            settings.InitRandom();
            settings.RollTables = new RollTableManager(settings);
            
            mapGenerator.SetSettings(settings);
            var grid = mapGenerator.GenerateMap();

            FeatureGenerator.GenerateFeatures(grid, settings);

            var siteGenerator = new HexSiteFiller();
            siteGenerator.GenerateSites(grid, settings);

            hexGridDrawer.DrawGrid(grid);
        }
    }
}