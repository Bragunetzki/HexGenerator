using UnityEngine;

[RequireComponent(typeof(HexGridDrawer))]
[RequireComponent(typeof(MapGenerator))]
public class HexMapCreator : MonoBehaviour
{
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private HexGridDrawer hexGridDrawer;

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

        var grid = mapGenerator.GenerateMap();
        hexGridDrawer.grid = grid;
        CreateMap();
    }

    public void CreateMap()
    {
        var grid = mapGenerator.GenerateMap();
        hexGridDrawer.grid = grid;
        hexGridDrawer.DrawGrid();
    }
}