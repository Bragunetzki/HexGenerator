using Hexes;
using UnityEngine;

namespace WorldMapDisplay
{
    public class HexCell : MonoBehaviour
    {
        public HexCoordinates coordinates;
        public RectTransform uiRect;
        private const float ElevationStep = 0.5f;
        private int _elevation;
        public int Elevation {
            set {
                _elevation = value;
                var transform1 = transform;
                var position = transform1.localPosition;
                position.y = value * ElevationStep;
                transform1.localPosition = position;
                
                var uiPosition = uiRect.localPosition;
                uiPosition.z = _elevation * -ElevationStep;
                uiRect.localPosition = uiPosition;
            }
            get => _elevation;
        }
        public Color color;
    }
}