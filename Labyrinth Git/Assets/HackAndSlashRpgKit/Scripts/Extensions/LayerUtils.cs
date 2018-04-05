using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
#endif
public class LayerUtils
{
    /// <summary>
    /// This will set up automatically all the layers that we need in V2.0.
    /// This layers will get used for physical collision map.
    /// </summary>
    static LayerUtils()
    {
        createLayer();
    }

    private static List<string> requiredLayers = new List<string>() { "Player", "Enemy", "Item", "DeadEnemy" };

    static void createLayer()
    {
#if UNITY_EDITOR
        Debug.Log("LayerUtils.createLayer get called");
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        SerializedProperty layers = tagManager.FindProperty("layers");
        if (layers == null || !layers.isArray)
        {
            Debug.LogWarning("Can't set up the layers.  It's possible the format of the layers and tags data has changed in this version of Unity.");
            Debug.LogWarning("Layers is null: " + (layers == null));
            return;
        }
        foreach (string layerName in requiredLayers)
        {
            Debug.Log("Trying to setting up " + layerName + " layer");
            //till 8 seems reserved.
            for (int i = 8; i < layers.arraySize; i++)
            {
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
                if (layerSP.stringValue != layerName && !requiredLayers.Contains(layerSP.stringValue))
                {
                    Debug.Log("Setting up layers.  Layer " + i + " is now called " + layerName);
                    layerSP.stringValue = layerName;
                    break;
                }
            }
        }
        tagManager.ApplyModifiedProperties();
#endif
    }
}