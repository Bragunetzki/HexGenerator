using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexMapCreator))]
public class MapCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var mapGenerator = (HexMapCreator) target;
        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            mapGenerator.CreateMap();
        }
        base.OnInspectorGUI();
    }
}
