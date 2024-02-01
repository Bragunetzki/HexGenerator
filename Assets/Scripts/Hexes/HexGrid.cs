using System.Collections.Generic;
using UnityEngine;
using WorldGeneration;

namespace Hexes
{
    public class HexGrid
    {
        private readonly Dictionary<HexCoordinates, WorldHex> _grid;
        public readonly HexLayout HexLayout;
        private readonly bool _isFlatTopped;
        public readonly Vector3 Origin;

        public HexGrid(bool isFlatTopped, Vector2 hexSize, Vector3 origin)
        {
            _grid = new Dictionary<HexCoordinates, WorldHex>();
            var orientation = isFlatTopped ? HexLayout.Flat : HexLayout.Pointy;
            _isFlatTopped = isFlatTopped;
            HexLayout = new HexLayout(orientation, hexSize, origin);
            Origin = origin;
        }

        public Vector2 GetHexSize()
        {
            return HexLayout.Size;
        }

        public void AddHex(WorldHex hex)
        {
            _grid[hex.CoordHolder] = hex;
        }

        public WorldHex GetHex(HexCoordinates hexCoordinates)
        {
            return _grid.TryGetValue(hexCoordinates, out var worldHex) ? worldHex : null;
        }

        public WorldHex NeighborOf(WorldHex hex, int direction)
        {
            var neighborCoords = hex.CoordHolder.Neighbor(direction);
            _grid.TryGetValue(neighborCoords, out var result);
            return result;
        }

        public List<WorldHex> NeighborsOf(WorldHex hexCoordinates)
        {
            var neighbors = new List<WorldHex>();
            for (var direction = 0; direction < 6; direction++)
            {
                neighbors.Add(NeighborOf(hexCoordinates, direction));
            }
            return neighbors;
        }

        public List<WorldHex> AllHexes()
        {
            var valuesList = new List<WorldHex>(_grid.Values);
            return valuesList;
        }

        // Returns the global coordinates of the hex.
        public Vector3 HexToWorld(WorldHex hex)
        {
            return HexLayout.HexToCoords(hex.CoordHolder);
        }

        public WorldHex WorldToHex(Vector3 v)
        {
            var hexCoords = HexLayout.CoordsToHex(v).HexRound();
            return GetHex(hexCoords);
        }
        
        // Returns the local coordinates of the hex.
        public Vector3 HexToLocalOffset(WorldHex hex)
        {
            return HexLayout.HexToLocalCoords(hex.CoordHolder);
        }
        
        public Vector3 HexToLocalOffset(HexCoordinates coords)
        {
            return HexLayout.HexToLocalCoords(coords);
        }

        public Vector2 RectangularGridSize(int width, int height, Vector2 hexRes)
        {
            if (_isFlatTopped)
            {
                var w = hexRes.x * (width * 1.5f + 0.5f) * 2;
                var h = hexRes.y * (height * Mathf.Sqrt(3) + 0.5f) * 2;
                return new Vector2(w, h);
            }
            else
            {
                var w = hexRes.x * (width * Mathf.Sqrt(3) + 0.5f);
                var h = hexRes.y * (height * 1.5f + 0.5f);
                return new Vector2(w, h);
            }
        }

        public float GetHexOffsetRotation()
        {
            return HexLayout.GetHexOffsetRotation();
        }

        public void PopulateRectangle(int top, int bottom, int left, int right)
        {
            if (_isFlatTopped)
            {
                for (var q = left; q <= right; q++)
                {
                    var qOffset = q >> 1;
                    for (var r = top - qOffset; r <= bottom - qOffset; r++)
                    {
                        AddHex(new WorldHex(q, r));
                    }
                }
            }
            else
            {
                for (var r = top; r <= bottom; r++)
                {
                    var rOffset = r >> 1;
                    for (var q = left - rOffset; q <= right - rOffset; q++)
                    {
                        AddHex(new WorldHex(q, r));
                    }
                }
            }
        }
    }
}