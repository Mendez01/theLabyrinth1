using System.Collections;
using UnityEngine;

/// <summary>
/// Author : Hwan Kim
/// </summary>
[ExecuteInEditMode]
public class MeshFixer : MonoBehaviour
{
    public SkinnedMeshRenderer myMesh;

    private void Awake()
    {
        myMesh = GetComponent<SkinnedMeshRenderer>();
        if (myMesh.sharedMesh == null || !gameObject.name.Equals(myMesh.sharedMesh.name))
        {
            if (myMesh.sharedMesh != null)
                Debug.Log(" unmatched mesh (" + gameObject.name + "): as " + myMesh.sharedMesh.name);
            foreach (SkinnedMeshRenderer mesh in (SkinnedMeshRenderer[])Resources.FindObjectsOfTypeAll(typeof(SkinnedMeshRenderer)))
            {
                if (mesh.sharedMesh != null && gameObject.name.Equals(mesh.sharedMesh.name))
                {
                    Debug.Log("Fixing unmatched mesh : " + mesh.sharedMesh.name);
                    myMesh.sharedMesh = mesh.sharedMesh;
                }
            }
        }
    }
}