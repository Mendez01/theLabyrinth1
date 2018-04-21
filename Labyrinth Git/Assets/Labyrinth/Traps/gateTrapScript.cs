using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gateTrapScript : MonoBehaviour {

    public float gatefalltime = 10f;

    public GameObject gateTrap;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gateTrap.transform.Translate(-16, 0, 0);
        }
    }
}
