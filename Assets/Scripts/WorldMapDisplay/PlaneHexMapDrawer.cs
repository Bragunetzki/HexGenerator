using System.Collections.Generic;
using Hexes;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using WorldGeneration;

namespace WorldMapDisplay
{
    public class PlaneHexMapDrawer : MonoBehaviour, IHexGridDrawer
    {
        public HexCell hexPrefab;
        private HexGrid _grid;
        private HexMesh _hexMesh;
        
        [SerializeField] private TextMeshProUGUI cellLabelPrefab;
        [SerializeField] private Canvas gridCanvas;

        private readonly Dictionary<HexCoordinates, HexCell> _hexCells = new();
        private void Awake()
        {
            gridCanvas = GetComponentInChildren<Canvas>();
            _hexMesh = GetComponentInChildren<HexMesh>();
        }

        private void Start()
        {
            _hexMesh.Triangulate(_grid, _hexCells);
        }

        public void DrawGrid(HexGrid grid)
        {
            if (grid == null)
            {
                return;
            }

            _grid = grid;

            // delete any existing children.
            DeleteChildren(transform);

            foreach (var hex in grid.AllHexes())
            {
                DrawHex(hex);
            }
        }

        private void DrawHex(WorldHex hex)
        {
            var hexPosition = _grid.HexToLocalOffset(hex);
            
            // Instantiate parent object and set it as child of grid.
            var hexObj = Instantiate(hexPrefab, transform, false);
            var transform1 = hexObj.transform;
            transform1.position = hexPosition;

            var hexLocalScale = transform1.localScale;
            var hexSize = _grid.GetHexSize();
            hexLocalScale = new Vector3(hexLocalScale.x * hexSize.x, hexLocalScale.y,
                hexLocalScale.z * hexSize.y);
            transform1.localScale = hexLocalScale;
            hexObj.color = Color.white;
            _hexCells.Add(hex.CoordHolder, hexObj);

            var label = Instantiate(cellLabelPrefab, gridCanvas.transform, false);
            label.rectTransform.anchoredPosition =
                new Vector2(hexPosition.x, hexPosition.z);
            label.text = hex.CoordHolder.Coords.x + "\n" + hex.CoordHolder.Coords.y + "\n" + hex.CoordHolder.Coords.z;
        }

        public void ColorCell (Vector3 position, Color color) {
            position = transform.InverseTransformPoint(position);
            var hex = _grid.WorldToHex(position);
            var cell = _hexCells[hex.CoordHolder];
            cell.color = color;
            _hexMesh.Triangulate(_grid, _hexCells);
        }

        private void DeleteChildren(Transform parent)
        {
            if (!EditorApplication.isPlaying)
            {
                for (var i = parent.childCount - 1; i >= 0; i--)
                {
                    var child = parent.GetChild(i);

                    if (child.GetComponent<HexCell>() is not null)
                    {
                        DestroyImmediate(child.gameObject);
                    }
                }
                for (var i = gridCanvas.transform.childCount - 1; i >= 0; i--)
                {
                    var child = gridCanvas.transform.GetChild(i).gameObject;
                    DestroyImmediate(child);
                }

                EditorSceneManager.MarkSceneDirty(SceneManager
                    .GetActiveScene());
                return;
            }

            for (var i = gridCanvas.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(gridCanvas.transform.GetChild(i).gameObject);
            }

            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i);
                if (child.GetComponent<HexCell>() is not null)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }
}