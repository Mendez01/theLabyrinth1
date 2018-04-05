using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Author: Hwan Kim
/// </summary>
public class Eye : MonoBehaviour
{
    public Material myMat;
    public SkinnedMeshRenderer render;

    // Use this for initialization
    private void Awake()
    {
        render = GetComponent<SkinnedMeshRenderer>();
        myMat = render.sharedMaterial;
    }

    public void setEyeMat(Material mat)
    {
        render.sharedMaterial = mat;
    }

    public void randomEyeColorChange()
    {
        render.sharedMaterial.color = ColorHSV.GetRandomColor(Random.Range(0.0f, 360f), 1, 1);
    }

    public void eyeColorChange(Color newColor)
    {
        render.sharedMaterial.color = newColor;
    }

    public Color getEyeColor()
    {
        return render.sharedMaterial.color;
    }
}