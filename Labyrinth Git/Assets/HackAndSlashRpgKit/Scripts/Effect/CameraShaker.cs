using System.Collections;
using UnityEngine;

/// <summary>
/// Author: Hwam Kim
/// Converted from Java script
/// Source : http://answers.unity3d.com/questions/212189/camera-shake.html
/// </summary>
public class CameraShaker : MonoBehaviour
{
    private Vector3 originPosition;
    private Quaternion originRotation;
    public float shake_decay;
    public float shake_intensity;
    private static Transform myTran;

    private void Awake()
    {
        myTran = transform;
    }

    private void Update()
    {
        if (shake_intensity > 0)
        {
            myTran.position = originPosition + Random.insideUnitSphere * shake_intensity;
            myTran.rotation = new Quaternion(
            originRotation.x + Random.Range(-shake_intensity, shake_intensity) * .2f,
            originRotation.y + Random.Range(-shake_intensity, shake_intensity) * .2f,
            originRotation.z + Random.Range(-shake_intensity, shake_intensity) * .2f,
            originRotation.w + Random.Range(-shake_intensity, shake_intensity) * .2f);
            shake_intensity -= shake_decay;
        }
    }

    public float intensity = .05f;
    public float decay = 0.002f;

    public void Shake()
    {
        shake_intensity = intensity;
        shake_decay = decay;
        originPosition = myTran.position;
        originRotation = myTran.rotation;
    }
}