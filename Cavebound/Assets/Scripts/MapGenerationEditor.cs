using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(MapGenerator))]
public class MapGenerationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapGenerator myScript = (MapGenerator)target;

        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("Generate Map"))
            {
                myScript.generateMap();
            }

            if (GUILayout.Button("Clear Map"))
            {
                myScript.clearMap();
            }
        }
    } 
}