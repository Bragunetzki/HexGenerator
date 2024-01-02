using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hexes
{
    public readonly struct HexCoordinates
    {
        public readonly Vector3Int Coords;

        public HexCoordinates(int q, int r, int s)
        {
            Coords = Vector3Int.zero;
            Coords.x = q;
            Coords.y = r;
            Coords.z = s;
            if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
        }

        public HexCoordinates(Vector3Int coords)
        {
            Coords = Vector3Int.zero;
            Coords = coords;
            if (Coords.x + Coords.y + Coords.z != 0) throw new ArgumentException("q + r + s must be 0");
        }

        public HexCoordinates(int q, int r)
        {
            Coords = Vector3Int.zero;
            Coords.x = q;
            Coords.y = r;
            Coords.z = -q - r;
        }

        public override int GetHashCode() => (Coords.x, Coords.y).GetHashCode();

        public override bool Equals(object obj) => obj is HexCoordinates other && Equals(other);

        private bool Equals(HexCoordinates other) => Coords.x == other.Coords.x && Coords.y == other.Coords.y;

        public HexCoordinates Add(HexCoordinates b)
        {
            return new HexCoordinates(Coords + b.Coords);
        }

        public HexCoordinates Subtract(HexCoordinates b)
        {
            return new HexCoordinates(Coords - b.Coords);
        }

        public HexCoordinates Scale(int b)
        {
            return new HexCoordinates(Coords * b);
        }

        private int Length()
        {
            return (Mathf.Abs(Coords.x) + Mathf.Abs(Coords.y) + Mathf.Abs(Coords.z)) / 2;
        }

        public int Distance(HexCoordinates b)
        {
            return Subtract(b).Length();
        }

        private static readonly List<HexCoordinates> Directions = new()
        {
            new HexCoordinates(1, 0, -1), new HexCoordinates(1, -1, 0),
            new HexCoordinates(0, -1, 1), new HexCoordinates(-1, 0, 1),
            new HexCoordinates(-1, 1, 0), new HexCoordinates(0, 1, -1)
        };

        private static HexCoordinates Direction(int direction)
        {
            return Directions[(6 + direction % 6) % 6];
        }

        public HexCoordinates Neighbor(int direction)
        {
            return Add(Direction(direction));
        }

        public HexCoordinates RotateLeft()
        {
            return new HexCoordinates(-Coords.z, -Coords.x, -Coords.y);
        }

        public HexCoordinates RotateRight()
        {
            return new HexCoordinates(-Coords.y, -Coords.z, -Coords.x);
        }
    }

    public struct OffsetCoord
    {
        private Vector2Int _coords;
        public const int Even = 1;
        public const int Odd = -1;

        public OffsetCoord(int col, int row)
        {
            _coords = default;
            _coords.x = col;
            _coords.y = row;
        }

        public OffsetCoord(Vector2Int coords)
        {
            _coords = coords;
        }

        public static OffsetCoord QOffsetFromCube(int offset, HexCoordinates h)
        {
            var col = h.Coords.x;
            var row = h.Coords.y + (h.Coords.x + offset * (h.Coords.x & 1)) / 2;
            if (offset != Even && offset != Odd)
            {
                throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
            }

            return new OffsetCoord(col, row);
        }


        public static HexCoordinates QOffsetToCube(int offset, OffsetCoord h)
        {
            var q = h._coords.x;
            var r = h._coords.y - (h._coords.x + offset * (h._coords.x & 1)) / 2;
            var s = -q - r;
            if (offset != Even && offset != Odd)
            {
                throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
            }

            return new HexCoordinates(q, r, s);
        }


        public static OffsetCoord ROffsetFromCube(int offset, HexCoordinates h)
        {
            var col = h.Coords.x + (h.Coords.y + offset * (h.Coords.y & 1)) / 2;
            var row = h.Coords.y;
            if (offset != Even && offset != Odd)
            {
                throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
            }

            return new OffsetCoord(col, row);
        }


        public static HexCoordinates ROffsetToCube(int offset, OffsetCoord h)
        {
            var q = h._coords.x - (h._coords.y + offset * (h._coords.y & 1)) / 2;
            var r = h._coords.y;
            var s = -q - r;
            if (offset != Even && offset != Odd)
            {
                throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
            }

            return new HexCoordinates(q, r, s);
        }
    }

    public struct FractionalHex
    {
        public readonly Vector3 Coords;

        public FractionalHex(float q, float r, float s)
        {
            Coords.x = q;
            Coords.y = r;
            Coords.z = s;
            if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
        }

        public FractionalHex(Vector3 coords)
        {
            Coords = coords;
            if (Coords.x + Coords.y + Coords.z != 0) throw new ArgumentException("q + r + s must be 0");
        }

        public FractionalHex(float q, float r)
        {
            Coords.x = q;
            Coords.y = r;
            Coords.z = -q - r;
        }

        public HexCoordinates HexRound()
        {
            var qi = (int)(Mathf.Round(Coords.x));
            var ri = (int)(Mathf.Round(Coords.y));
            var si = (int)(Mathf.Round(Coords.z));
            double qDiff = Mathf.Abs(qi - Coords.x);
            double rDiff = Mathf.Abs(ri - Coords.y);
            double sDiff = Mathf.Abs(si - Coords.z);
            if (qDiff > rDiff && qDiff > sDiff)
            {
                qi = -ri - si;
            }
            else if (rDiff > sDiff)
            {
                ri = -qi - si;
            }
            else
            {
                si = -qi - ri;
            }

            return new HexCoordinates(qi, ri, si);
        }

        public FractionalHex HexLerp(FractionalHex b, float f)
        {
            return new FractionalHex(Vector3.Lerp(Coords, b.Coords, f));
        }

        public List<HexCoordinates> HexLineDraw(HexCoordinates a, HexCoordinates b)
        {
            var dist = a.Distance(b);
            var aNudge = new FractionalHex(a.Coords.x + 1e-06f, a.Coords.y + 1e-06f, a.Coords.z - 2e-06f);
            var bNudge = new FractionalHex(b.Coords.x + 1e-06f, b.Coords.y + 1e-06f, b.Coords.z - 2e-06f);
            var results = new List<HexCoordinates>();
            var step = 1f / Mathf.Max(dist, 1);
            for (var i = 0; i <= dist; i++)
            {
                results.Add(aNudge.HexLerp(bNudge, step * i).HexRound());
            }

            return results;
        }
    }

    public struct Orientation
    {
        public Orientation(float[,] forward, float[,] inverse, float startAngle)
        {
            Forward = forward;
            Inverse = inverse;
            StartAngle = startAngle;
        }

        public readonly float[,] Forward;
        public readonly float[,] Inverse;
        public readonly float StartAngle;
    }

    public struct HexLayout
    {
        public HexLayout(Orientation orientation, Vector2 size, Vector2 origin)
        {
            Orientation = orientation;
            Size = size;
            Origin = origin;
        }

        public static Orientation Pointy = new Orientation(
            new[,] { { Mathf.Sqrt(3f), Mathf.Sqrt(3f) / 2f }, { 0, 3f / 2f } },
            new[,] { { 1f / Mathf.Sqrt(3f), -1f / 3f }, { 0, 2f / 3f } },
            0);

        public static Orientation Flat = new Orientation(
            new[,] { { 3f / 2f, 0 }, { Mathf.Sqrt(3f) / 2f, Mathf.Sqrt(3) } },
            new[,] { { 2f / 3f, 0 }, { -1f / 3f, 1f / Mathf.Sqrt(3f) } },
            0.5f);

        public readonly Orientation Orientation;

        public readonly Vector2 Size;

        public readonly Vector2 Origin;

        public readonly Vector2 HexToPixel(HexCoordinates h)
        {
            var m = Orientation;
            var x = (m.Forward[0, 0] * h.Coords.x + m.Forward[0, 1] * h.Coords.y) * Size.x;
            var y = (m.Forward[1, 0] * h.Coords.x + m.Forward[1, 1] * h.Coords.y) * Size.y;
            return new Vector2(Origin.x + x, Origin.y + y);
        }

        public readonly FractionalHex PixelToHex(Vector2 p)
        {
            var m = Orientation;
            var pt = new Vector2((p.x - Origin.x) / Size.x, (p.y - Origin.y) / Size.y);
            var q = m.Inverse[0, 0] * pt.x + m.Inverse[0, 1] * pt.y;
            var r = m.Inverse[1, 0] * pt.x + m.Inverse[1, 1] * pt.y;
            return new FractionalHex(q, r);
        }

        public Vector2 HexCornerOffset(int corner)
        {
            var m = Orientation;
            var angle = 2f * Mathf.PI * (m.StartAngle - corner) / 6f;
            return new Vector2(Size.x * Mathf.Cos(angle), Size.y * Mathf.Sin(angle));
        }

        public readonly float GetHexOffsetRotation()
        {
            return Orientation.StartAngle * 60;
        }

        public List<Vector2> PolygonCorners(HexCoordinates h)
        {
            var corners = new List<Vector2>();
            var center = HexToPixel(h);
            for (var i = 0; i < 6; i++)
            {
                var offset = HexCornerOffset(i);
                corners.Add(center + offset);
            }

            return corners;
        }
    }
}