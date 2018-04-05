using UnityEngine;

/// <summary>
/// Author : _USSR http://answers.unity3d.com/users/615244/-ussr.html
/// Source: http://answers.unity3d.com/questions/41931/how-to-randomly-change-the-intensity-of-a-point-li.html
/// </summary>
[RequireComponent(typeof(Light))]
public class SoftFlicker : MonoBehaviour
{
    public float minIntensity = 0.25f;
    public float maxIntensity = 0.5f;
    public float maxRange;
    private float minRange;
    private Light thisLight;
    private float random;

    private void Start()
    {
        random = Random.Range(0.0f, 65535.0f);
        thisLight = GetComponent<Light>();
        minRange = thisLight.range;
    }

    private void FixedUpdate()
    {
        float noise = Mathf.PerlinNoise(random, Time.time);
        thisLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        thisLight.range = Mathf.Lerp(minRange, maxRange, noise);
    }
}