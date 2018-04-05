using System.Collections;
using UnityEngine;

public class SkinnedMeshToMeshV2 : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    // Use this for initialization
    public void process()
    {
        SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
        meshFilter =
            (UnityEngine.MeshFilter)
                gameObject.AddComponent(typeof(MeshFilter));
        meshFilter.mesh = skinnedMeshRenderer.sharedMesh;
        meshRenderer =
            (UnityEngine.MeshRenderer)
                gameObject.AddComponent(typeof(MeshRenderer));
        meshRenderer.materials =
            new Material[1];
        meshRenderer.materials[0] = skinnedMeshRenderer.sharedMaterial;
        meshRenderer.material = skinnedMeshRenderer.sharedMaterial;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        meshRenderer.receiveShadows = true;
        Destroy(skinnedMeshRenderer);
    }
}