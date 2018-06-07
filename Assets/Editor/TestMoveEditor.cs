using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestObject))]
public class TestMoveEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TestObject myScript = (TestObject)target;

        if (GUILayout.Button("Test Move") == true) {
            myScript.MoveObject();
        }

        if (GUILayout.Button("Reset Object") == true) {
            myScript.ResetObject();
        }
    }
}
