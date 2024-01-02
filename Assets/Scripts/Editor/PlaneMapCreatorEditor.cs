using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(PlaneMapCreator))]
    public class PlaneMapCreatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var mapGenerator = (PlaneMapCreator) target;
            
            if (DrawDefaultInspector())
            {
                if (mapGenerator.autoUpdate)
                {
                    mapGenerator.GenerateMap();
                }
            }

            if (GUILayout.Button("Generate"))
            {
                mapGenerator.GenerateMap();
            }
        }
    }
}
