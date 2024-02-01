using UnityEngine;
using UnityEngine.EventSystems;

namespace WorldMapDisplay
{
    public class HexMapEditor : MonoBehaviour
    {
        public Color[] colors;
        public PlaneHexMapDrawer hexDrawer;
        private Color _activeColor;
        private int _activeElevation;

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
            if (Physics.Raycast(inputRay, out hit))
            {
                EditCell(hexDrawer.GetCell(hit.point));
            }
        }

        private void EditCell (HexCell cell)
        {
            cell.color = _activeColor;
            cell.Elevation = _activeElevation;
            hexDrawer.Refresh();
        }

        public void SelectColor(int index)
        {
            _activeColor = colors[index];
        }
        
        public void SetElevation (float elevation) {
            _activeElevation = (int)elevation;
        }
    }
}