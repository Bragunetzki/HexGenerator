using UnityEngine;
using UnityEngine.EventSystems;

namespace WorldMapDisplay
{
    public class HexMapEditor : MonoBehaviour
    {
        public Color[] colors;
        public PlaneHexMapDrawer hexDrawer;
        private Color _activeColor;

        private void Awake () {
            SelectColor(0);
        }
        
        private void Update () {
            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) {
                HandleInput();
            }
        }

        private void HandleInput ()
        {
            if (UnityEngine.Camera.main is null) return;
            var inputRay = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit)) {
                hexDrawer.ColorCell(hit.point, _activeColor);
            }
        }

        public void SelectColor(int index)
        {
            _activeColor = colors[index];
        }
    }
}