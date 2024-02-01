using System.Collections.Generic;
using Hexes;
using UnityEngine;

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
        private const int TerracesPerSlope = 2;
        private const int TerraceSteps = TerracesPerSlope * 2 + 1;
        private const float HorizontalTerraceStepSize = 1f / TerraceSteps;
        private const float VerticalTerraceStepSize = 1f / (TerracesPerSlope + 1);

        private enum HexConnectionType
        {
            Flat,
            Slope,
            Cliff
        }

        private void Awake()
        {
            GetComponent<MeshFilter>().mesh = _hexMesh = new Mesh();
            _meshCollider = gameObject.AddComponent<MeshCollider>();
            _hexMesh.name = "Hex Mesh";
            _vertices = new List<Vector3>();
            _triangles = new List<int>();
            _colors = new List<Color>();
        }

        public void Triangulate(HexLayout layout, Dictionary<HexCoordinates, HexCell> hexCells)
        {
            _hexMesh.Clear();
            _vertices.Clear();
            _triangles.Clear();
            _colors.Clear();
            foreach (var t in hexCells.Values)
            {
                Triangulate(layout, t.coordinates, hexCells);
            }

            _hexMesh.vertices = _vertices.ToArray();
            _hexMesh.colors = _colors.ToArray();
            _hexMesh.triangles = _triangles.ToArray();
            _hexMesh.RecalculateNormals();
            _meshCollider.sharedMesh = _hexMesh;
        }

        private void Triangulate(HexLayout layout, HexCoordinates hexCoords,
            IReadOnlyDictionary<HexCoordinates, HexCell> hexCells)
        {
            //add height option
            var cell = hexCells[hexCoords];
            var position = cell.transform.localPosition;
            var corners = layout.PolygonCorners(hexCoords, SolidFactor, position.y);
            //add height option
            var center = layout.HexToLocalCoords(hexCoords, position.y);

            for (var dir = 0; dir < 6; dir++)
            {
                var v1 = corners[dir];
                var v2 = corners[(dir + 1) % 6];
                AddTriangle(center, v1, v2);

                AddTriangleColor(cell.color);

                if (dir < 3)
                    TriangulateConnection(layout, hexCoords, hexCells, center, v1, v2, dir);
            }
        }

        private void TriangulateConnection(HexLayout layout, HexCoordinates hexCoords,
            IReadOnlyDictionary<HexCoordinates, HexCell> hexCells, Vector3 center, Vector3 v1, Vector3 v2, int dir)
        {
            var blendCorners = layout.PolygonCorners(hexCoords, 1, hexCells[hexCoords].transform.localPosition.y);

            var bridgeVec = (blendCorners[dir] - center + blendCorners[(dir + 1) % 6] - center) * BlendFactor;
            var bridgeVec2 = (blendCorners[(dir + 1) % 6] - center + blendCorners[(dir + 2) % 6] - center) *
                             BlendFactor;

            var neighborCoords = hexCoords.Neighbor(dir);
            hexCells.TryGetValue(neighborCoords, out var neighborCell);
            if (neighborCell is null) return;

            var cell = hexCells[hexCoords];
            var v3 = v1 + bridgeVec;
            var v4 = v2 + bridgeVec;
            v3.y = v4.y = neighborCell.transform.localPosition.y;

            if (GetConnectionType(cell, neighborCell) == HexConnectionType.Slope)
                TriangulateTerraceEdges(v1, v2, hexCells[hexCoords], v3, v4, neighborCell);
            else
            {
                AddQuad(v1, v2, v3, v4);
                AddQuadColor(cell.color, neighborCell.color);
            }

            var nextNeighbor = hexCoords.Neighbor((dir + 1) % 6);
            hexCells.TryGetValue(nextNeighbor, out var nextNeighborCell);

            if (nextNeighborCell is null) return;
            if (dir >= 2) return;
            var v5 = v2 + bridgeVec2;
            v5.y = nextNeighborCell.transform.localPosition.y;

            if (cell.Elevation <= neighborCell.Elevation)
            {
                if (cell.Elevation <= nextNeighborCell.Elevation)
                {
                    TriangulateCorner(v2, cell, v4, neighborCell, v5, nextNeighborCell);
                }
                else
                {
                    TriangulateCorner(v5, nextNeighborCell, v2, cell, v4, neighborCell);
                }
            }
            else if (neighborCell.Elevation <= nextNeighborCell.Elevation)
            {
                TriangulateCorner(v4, neighborCell, v5, nextNeighborCell, v2, cell);
            }
            else
            {
                TriangulateCorner(v5, nextNeighborCell, v2, cell, v4, neighborCell);
            }
        }

        private void TriangulateCorner(
            Vector3 bottom, HexCell bottomCell,
            Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell)
        {
            var leftEdgeType = GetConnectionType(bottomCell, leftCell);
            var rightEdgeType = GetConnectionType(bottomCell, rightCell);

            if (leftEdgeType == HexConnectionType.Slope)
            {
                switch (rightEdgeType)
                {
                    case HexConnectionType.Slope:
                        TriangulateCornerTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
                        break;
                    case HexConnectionType.Flat:
                        TriangulateCornerTerraces(left, leftCell, right, rightCell, bottom, bottomCell);
                        break;
                    case HexConnectionType.Cliff:
                    default:
                        TriangulateCornerTerracesCliff(bottom, bottomCell, left, leftCell, right, rightCell);
                        break;
                }
            }
            else if (rightEdgeType == HexConnectionType.Slope)
            {
                if (leftEdgeType == HexConnectionType.Flat)
                {
                    TriangulateCornerTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
                }
                else
                {
                    TriangulateCornerCliffTerraces(
                        bottom, bottomCell, left, leftCell, right, rightCell
                    );
                }
            }
            else if (GetConnectionType(leftCell, rightCell) == HexConnectionType.Slope)
            {
                if (leftCell.Elevation < rightCell.Elevation)
                {
                    TriangulateCornerCliffTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
                }
                else
                {
                    TriangulateCornerTerracesCliff(left, leftCell, right, rightCell, bottom, bottomCell);
                }
            }
            else
            {
                AddTriangle(bottom, left, right);
                AddTriangleColor(bottomCell.color, leftCell.color, rightCell.color);
            }
        }

        private void TriangulateCornerTerracesCliff(
            Vector3 start, HexCell startCell,
            Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell
        )
        {
            var b = 1f / (rightCell.Elevation - startCell.Elevation);
            if (b < 0)
                b = -b;
            var boundary = Vector3.Lerp(start, right, b);
            var boundaryColor = Color.Lerp(startCell.color, rightCell.color, b);

            TriangulateBoundaryTriangle(
                start, startCell, left, leftCell, boundary, boundaryColor
            );

            if (GetConnectionType(leftCell, rightCell) == HexConnectionType.Slope)
            {
                TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
            }
            else
            {
                AddTriangle(left, right, boundary);
                AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
            }
        }

        private void TriangulateCornerCliffTerraces(
            Vector3 start, HexCell startCell,
            Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell
        )
        {
            var b = 1f / (leftCell.Elevation - startCell.Elevation);
            if (b < 0)
                b = -b;
            var boundary = Vector3.Lerp(start, left, b);
            var boundaryColor = Color.Lerp(startCell.color, leftCell.color, b);

            TriangulateBoundaryTriangle(
                right, rightCell, start, startCell, boundary, boundaryColor
            );

            if (GetConnectionType(leftCell, rightCell) == HexConnectionType.Slope)
            {
                TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
            }
            else
            {
                AddTriangle(left, right, boundary);
                AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
            }
        }

        private void TriangulateBoundaryTriangle(Vector3 start, HexCell startCell, Vector3 left, HexCell leftCell,
            Vector3 boundary, Color boundaryColor)
        {
            var v2 = TerraceLerp(start, left, 1);
            var c2 = TerraceLerp(startCell.color, leftCell.color, 1);

            AddTriangle(start, v2, boundary);
            AddTriangleColor(startCell.color, c2, boundaryColor);

            for (var i = 2; i < TerraceSteps; i++)
            {
                var v1 = v2;
                var c1 = c2;
                v2 = TerraceLerp(start, left, i);
                c2 = TerraceLerp(startCell.color, leftCell.color, i);
                AddTriangle(v1, v2, boundary);
                AddTriangleColor(c1, c2, boundaryColor);
            }

            AddTriangle(v2, left, boundary);
            AddTriangleColor(c2, leftCell.color, boundaryColor);
        }

        private void TriangulateCornerTerraces(Vector3 start, HexCell startCell, Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell)
        {
            var v3 = TerraceLerp(start, left, 1);
            var v4 = TerraceLerp(start, right, 1);
            var c3 = TerraceLerp(startCell.color, leftCell.color, 1);
            var c4 = TerraceLerp(startCell.color, rightCell.color, 1);

            AddTriangle(start, v3, v4);
            AddTriangleColor(startCell.color, c3, c4);

            for (var i = 2; i < TerraceSteps; i++)
            {
                var v1 = v3;
                var v2 = v4;
                var c1 = c3;
                var c2 = c4;
                v3 = TerraceLerp(start, left, i);
                v4 = TerraceLerp(start, right, i);
                c3 = TerraceLerp(startCell.color, leftCell.color, i);
                c4 = TerraceLerp(startCell.color, rightCell.color, i);
                AddQuad(v1, v2, v3, v4);
                AddQuadColor(c1, c2, c3, c4);
            }

            AddQuad(v3, v4, left, right);
            AddQuadColor(c3, c4, leftCell.color, rightCell.color);
        }

        private void TriangulateTerraceEdges(Vector3 startLeft, Vector3 startRight, HexCell startCell, Vector3 endLeft,
            Vector3 endRight, HexCell endCell)
        {
            var v3 = TerraceLerp(startLeft, endLeft, 1);
            var v4 = TerraceLerp(startRight, endRight, 1);
            var c2 = TerraceLerp(startCell.color, endCell.color, 1);
            AddQuad(startLeft, startRight, v3, v4);
            AddQuadColor(startCell.color, c2);

            for (var i = 2; i < TerraceSteps; i++)
            {
                var v1 = v3;
                var v2 = v4;
                var c1 = c2;
                v3 = TerraceLerp(startLeft, endLeft, i);
                v4 = TerraceLerp(startRight, endRight, i);
                c2 = TerraceLerp(startCell.color, endCell.color, i);
                AddQuad(v1, v2, v3, v4);
                AddQuadColor(c1, c2);
            }

            AddQuad(v3, v4, endLeft, endRight);
            AddQuadColor(c2, endCell.color);
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

        private static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
        {
            var h = step * HorizontalTerraceStepSize;
            a.x += (b.x - a.x) * h;
            a.z += (b.z - a.z) * h;
            var yStep = (step + 1) / 2;
            var v = yStep * VerticalTerraceStepSize;
            a.y += (b.y - a.y) * v;
            return a;
        }

        private static Color TerraceLerp(Color a, Color b, int step)
        {
            var h = step * HorizontalTerraceStepSize;
            return Color.Lerp(a, b, h);
        }

        private static HexConnectionType GetConnectionType(int elevation1, int elevation2)
        {
            if (elevation1 == elevation2)
            {
                return HexConnectionType.Flat;
            }

            var delta = elevation2 - elevation1;
            return delta is -1 or 1 ? HexConnectionType.Slope : HexConnectionType.Cliff;
        }

        private static HexConnectionType GetConnectionType(HexCell cell, HexCell neighbor)
        {
            return GetConnectionType(
                neighbor.Elevation, cell.Elevation
            );
        }
    }
}