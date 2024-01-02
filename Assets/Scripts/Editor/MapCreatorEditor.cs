using UnityEditor;
using UnityEngine;
using WorldMapDisplay;

namespace Editor
{
    [CustomEditor(typeof(HexMapCreator))]
    public class MapCreatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var mapGenerator = (HexMapCreator) target;

            DrawDefaultInspector();
            if (GUILayout.Button("Generate"))
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                mapGenerator.CreateMap();
            }
        }
    }
}
