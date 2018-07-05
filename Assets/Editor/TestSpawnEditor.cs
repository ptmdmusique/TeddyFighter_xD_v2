using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestSpawn))]
public class TestSpawnEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TestSpawn myScript = (TestSpawn)target;

        if (GUILayout.Button("Test Spawn") == true) {
            myScript.SpawnObject(myScript.spawnIndx);
        }

    }
}
