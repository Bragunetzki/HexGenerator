using UnityEditor;
using UnityEngine;
using WorldGeneration.RollTables;

namespace Editor
{
    [CustomEditor(typeof(RollTable<>), true)]
    public class RollTableEditor : UnityEditor.Editor
    {
        private SerializedProperty _entries;

        private void OnEnable()
        {
            _entries = serializedObject.FindProperty("entries");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();
            
            EditorGUILayout.Space();

            if (GUILayout.Button("Add Entry"))
            {
                //AddEntry();
            }

            serializedObject.ApplyModifiedProperties();
        }

        // private void AddEntry()
        // {
        //     var rollTable = (RollTable<Object>)target;
        //     rollTable.AddEntry(0, null);
        // }
    }
}