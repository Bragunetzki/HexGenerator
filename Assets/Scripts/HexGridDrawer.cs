using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(HexMaterialProvider))]
public class HexGridDrawer : MonoBehaviour
{
    public HexGrid grid { get; set; }
    [SerializeField] private GameObject hexPrefab;
    [SerializeField] private HexMaterialProvider materialProvider;

    public void DrawGrid()
    {
        if (grid == null)
        {
            Debug.LogError("HexGrid not assigned!");
            return;
        }

        // delete any existing children.
        DeleteChildren(transform);

        foreach (var hex in grid.GetAll())
        {
            var hexPosition = grid.HexToWorld(hex);
            var hexEulerRot = hexPrefab.transform.localRotation.eulerAngles;
            var hexRotation = Quaternion.Euler(hexEulerRot.x, hexEulerRot.y + grid.GetHexOffsetRotation(),
                hexEulerRot.z);

            // Instantiate hex parent object.
            var hexParent = new GameObject("HexParent")
            {
                transform =
                {
                    position = hexPosition
                }
            };
            hexParent.transform.SetParent(transform);
            var localScale = hexParent.transform.localScale;

            // Instantiate hex object.
            var hexObject = Instantiate(hexPrefab, hexPosition, hexRotation);
            hexObject.transform.rotation = hexRotation;
            hexObject.transform.SetParent(hexParent.transform);

            // Change parent object scale to account for hex size and height. 
            var hexSize = grid.GetHexSize();
            var height = hex.Height;
            localScale = new Vector3(localScale.x * hexSize.x, localScale.y * height,
                localScale.z * hexSize.y);
            hexParent.transform.localScale = localScale;

            // Shift position to account for height.
            var transformPosition = hexParent.transform.position;
            transformPosition.y += hexObject.transform.localScale.y * localScale.y * 0.37f / 2f;
            hexParent.transform.position = transformPosition;


            // Set meshRenderer material depending on hex type.
            var meshRenderer = hexObject.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer == null) continue;
            var material = materialProvider.GetMaterialOfHex(hex);
            meshRenderer.material = material;
        }
    }

    private void DeleteChildren(Transform parent)
    {
        if (!EditorApplication.isPlaying)
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(parent.GetChild(i).gameObject);
            }
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager
                .GetActiveScene());
            return;
        }

        for (var i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}