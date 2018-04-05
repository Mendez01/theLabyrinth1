using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Author: Hwan Kim
/// </summary>
///

[CustomEditor(typeof(EquipmentV2))]
[CanEditMultipleObjects]
public class EquipmentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.

        DrawDefaultInspector();
        if (GUILayout.Button("Initialize Equipment"))
        {
            for (int i = 0; i < targets.Length; i++)
            {
                EquipmentV2 eq = (targets[i] as EquipmentV2);
                eq.init();
                Debug.Log(eq.name + " got Initialized as " + eq.equipmentDetails.id);
            }
        }
    }
}