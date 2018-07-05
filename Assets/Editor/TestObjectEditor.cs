using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestObject))]
public class TestObjectEditor : Editor {

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

        if (GUILayout.Button("Kill Object") == true) {
            myScript.TestDie();
        }

        if (GUILayout.Button("Reset Scene") == true) {
            myScript.RestartScene();
        }

        if (GUILayout.Button("Damage Object") == true) {
            myScript.DamageObjet();
        }
    }
}
