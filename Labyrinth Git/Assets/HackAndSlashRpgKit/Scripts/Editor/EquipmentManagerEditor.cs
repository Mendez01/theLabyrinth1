using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Author: Hwan Kim
/// </summary>
[CustomEditor(typeof(EquipmentManagerV2))]
public class EquipmentManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EquipmentManagerV2 myScript = (EquipmentManagerV2)target;
        if (!myScript.initialized)
        {
            if (GUILayout.Button("Initialize EquipmentManager"))
            {
                myScript.init();
            }
        }
        else
        {
            if (GUILayout.Button("Random Character"))
            {
                myScript.randomEquipment(false);
            }

            if (GUILayout.Button("Spawn Character"))
            {
                GameObject createGo = myScript.spawnTrimmedCharacterWithCrrentEquipments(myScript.spawnCharacterPos);
                createGo.name = RandomName.randomFullName;
            }

            if (GUILayout.Button("Save Equipment to Prefabs"))
            {
                myScript.startSavingPrefabs();
            }
        }
    }
}