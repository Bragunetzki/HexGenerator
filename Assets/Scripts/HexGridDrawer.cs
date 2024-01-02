using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(HexMaterialProvider))]
public class HexGridDrawer : MonoBehaviour
{
    public HexGrid Grid { get; set; }
    [SerializeField] private GameObject hexPrefab;
    [SerializeField] private HexMaterialProvider materialProvider;
    [SerializeField] public float heightDiffMultiplier = 2f;
    public readonly Dictionary<string, GameObject> FeaturePrefabs = new();

    public void DrawGrid()
    {
        if (Grid == null)
        {
            Debug.LogError("HexGrid not assigned!");
            return;
        }

        // delete any existing children.
        DeleteChildren(transform);

        foreach (var hex in Grid.GetAll())
        {
            DrawHex(hex);
        }
    }

    private void DrawHex(WorldHex hex)
    {
        var hexPosition = Grid.HexToWorld(hex);
        var hexEulerRot = hexPrefab.transform.localRotation.eulerAngles;
        var hexRotation = Quaternion.Euler(hexEulerRot.x, hexEulerRot.y + Grid.GetHexOffsetRotation(),
            hexEulerRot.z);

        // Instantiate parent object and set it as child of grid.
        var parentObj = new GameObject("HexParent")
        {
            transform =
            {
                position = hexPosition,
                parent = transform
            }
        };

        // Instantiate hex holder object and set as child of parent.
        var hexHolder = new GameObject("HexMeshHolder");
        hexHolder.transform.SetParent(parentObj.transform, false);

        // Instantiate hex object and set as child of hexHolder.
        var hexObject = Instantiate(hexPrefab, Vector3.zero, hexRotation);
        hexObject.transform.SetParent(hexHolder.transform, false);
        hexObject.transform.rotation = hexRotation;

        // Change hexHolder object scale to account for hex size and height. 
        var hexHolderLocalScale = hexHolder.transform.localScale;
        var hexSize = Grid.GetHexSize();
        var height = hex.Height * heightDiffMultiplier;
        hexHolderLocalScale = new Vector3(hexHolderLocalScale.x * hexSize.x, hexHolderLocalScale.y * height,
            hexHolderLocalScale.z * hexSize.y);
        hexHolder.transform.localScale = hexHolderLocalScale;

        // Shift hex holder position to account for height.
        var transformPosition = hexHolder.transform.position;
        // 0.37 - default height of this particular hex mesh, should probably adjust to be 1.
        var hexSurfaceOrigin = transformPosition;
        transformPosition.y += height * 0.37f / 2f;
        hexSurfaceOrigin.y += height * 0.37f;
        hexHolder.transform.position = transformPosition;
        
        // Set meshRenderer material depending on hex type.
        var meshRenderer = hexObject.GetComponentInChildren<MeshRenderer>();
        if (meshRenderer is null) return;
        var material = materialProvider.GetMaterialOfHex(hex);
        meshRenderer.material = material;

        // Draw the hex features.
        DrawFeatures(hex, parentObj, hexSurfaceOrigin, hexSize);
    }

    private static void DeleteChildren(Transform parent)
    {
        if (!EditorApplication.isPlaying)
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(parent.GetChild(i).gameObject);
            }

            EditorSceneManager.MarkSceneDirty(SceneManager
                .GetActiveScene());
            return;
        }

        for (var i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    private void DrawFeatures(WorldHex hex, GameObject parentObject, Vector3 origin, Vector2 hexSize)
    {
        foreach (var featureDrawer in hex.Features.Select(FeatureDrawerFactory.GetFeatureDrawer))
        {
            var drawDatalist = featureDrawer.GetDrawFeatureList(hex, origin, hexSize, FeaturePrefabs);
            foreach (var data in drawDatalist)
            {
                if (data.Prefab is null) continue;
                var featureObj = Instantiate(data.Prefab, data.Position, data.Rotation, parentObject.transform);
                featureObj.transform.localScale = data.Scale;
            }
        }
    }
}