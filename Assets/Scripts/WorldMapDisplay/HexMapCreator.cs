using HexMapGeneration;
using UnityEngine;

namespace WorldMapDisplay
{
    [RequireComponent(typeof(HexGridDrawer))]
    [RequireComponent(typeof(MapGenerator))]
    public class HexMapCreator : MonoBehaviour
    {
        [SerializeField] private MapGenerator mapGenerator;
        [SerializeField] private HexGridDrawer hexGridDrawer;

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
                hexGridDrawer = GetComponent<HexGridDrawer>();
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
            var grid = mapGenerator.GenerateMap();
            hexGridDrawer.Grid = grid;
            hexGridDrawer.FeaturePrefabs["tree"] = treePrefab;
            hexGridDrawer.DrawGrid();
        }
    }
}