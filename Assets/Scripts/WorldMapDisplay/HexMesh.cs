using System.Collections.Generic;
using Hexes;
using UnityEngine;
using WorldGeneration;

namespace WorldMapDisplay
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexMesh : MonoBehaviour
    {
        private Mesh _hexMesh;
        private List<Vector3> _vertices;
        private List<int> _triangles;
        private MeshCollider _meshCollider;
        private List<Color> _colors;
        private const float SolidFactor = 0.75f;
        private const float BlendFactor = 1f - SolidFactor;

        private void Awake()
        {
            GetComponent<MeshFilter>().mesh = _hexMesh = new Mesh();
            _meshCollider = gameObject.AddComponent<MeshCollider>();
            _hexMesh.name = "Hex Mesh";
            _vertices = new List<Vector3>();
            _triangles = new List<int>();
            _colors = new List<Color>();
        }

        public void Triangulate(HexGrid grid, Dictionary<HexCoordinates, HexCell> hexCells)
        {
            _hexMesh.Clear();
            _vertices.Clear();
            _triangles.Clear();
            _colors.Clear();
            foreach (var t in grid.AllHexes())
            {
                Triangulate(grid, t.CoordHolder, hexCells);
            }

            _hexMesh.vertices = _vertices.ToArray();
            _hexMesh.colors = _colors.ToArray();
            _hexMesh.triangles = _triangles.ToArray();
            _hexMesh.RecalculateNormals();
            _meshCollider.sharedMesh = _hexMesh;
        }

        private void Triangulate(HexGrid hexGrid, HexCoordinates hexCoords,
            IReadOnlyDictionary<HexCoordinates, HexCell> hexCells)
        {
            var corners = hexGrid.HexLayout.PolygonCorners(hexCoords, SolidFactor);
            var center = hexGrid.HexToLocalOffset(hexCoords);
            center.y = 0;
            for (var dir = 0; dir < 6; dir++)
            {
                var v1 = corners[dir];
                var v2 = corners[(dir + 1) % 6];
                AddTriangle(center, v1, v2);

                var cell = hexCells[hexCoords];
                AddTriangleColor(cell.color);

                if (dir < 3)
                    TriangulateConnection(hexGrid, hexCoords, hexCells, center, v1, v2, dir);
            }
        }

        private void TriangulateConnection(HexGrid grid, HexCoordinates hexCoords,
            IReadOnlyDictionary<HexCoordinates, HexCell> hexCells, Vector3 center, Vector3 v1, Vector3 v2, int dir)
        {
            var blendCorners = grid.HexLayout.PolygonCorners(hexCoords);
            var bridgeVec = (blendCorners[dir] - center + blendCorners[(dir + 1) % 6] - center) * BlendFactor;
            var bridgeVec2 = (blendCorners[(dir + 1) % 6] - center + blendCorners[(dir + 2) % 6] - center) *
                             BlendFactor;
            var neighborCoords = hexCoords.Neighbor(dir);
            hexCells.TryGetValue(neighborCoords, out var neighborCell);
            if (neighborCell is null) return;

            var cell = hexCells[hexCoords];
            var v3 = v1 + bridgeVec;
            var v4 = v2 + bridgeVec;
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(cell.color, neighborCell.color);

            var nextNeighbor = hexCoords.Neighbor((dir + 1) % 6);
            hexCells.TryGetValue(nextNeighbor, out var nextNeighborCell);
            if (nextNeighborCell is null) return;

            if (dir >= 2) return;
            AddTriangle(v2, v4, v2 + bridgeVec2);
            AddTriangleColor(cell.color, neighborCell.color, nextNeighborCell.color);
        }

        private void AddTriangleColor(Color c1, Color c2, Color c3)
        {
            _colors.Add(c1);
            _colors.Add(c2);
            _colors.Add(c3);
        }

        private void AddTriangleColor(Color color)
        {
            _colors.Add(color);
            _colors.Add(color);
            _colors.Add(color);
        }

        private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            var vertexIndex = _vertices.Count;
            _vertices.Add(v1);
            _vertices.Add(v2);
            _vertices.Add(v3);
            _triangles.Add(vertexIndex);
            _triangles.Add(vertexIndex + 1);
            _triangles.Add(vertexIndex + 2);
        }

        private void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            var vertexIndex = _vertices.Count;
            _vertices.Add(v1);
            _vertices.Add(v2);
            _vertices.Add(v3);
            _vertices.Add(v4);
            _triangles.Add(vertexIndex);
            _triangles.Add(vertexIndex + 2);
            _triangles.Add(vertexIndex + 1);
            _triangles.Add(vertexIndex + 1);
            _triangles.Add(vertexIndex + 2);
            _triangles.Add(vertexIndex + 3);
        }

        private void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
        {
            _colors.Add(c1);
            _colors.Add(c2);
            _colors.Add(c3);
            _colors.Add(c4);
        }

        private void AddQuadColor(Color c1, Color c2)
        {
            _colors.Add(c1);
            _colors.Add(c1);
            _colors.Add(c2);
            _colors.Add(c2);
        }
    }
}