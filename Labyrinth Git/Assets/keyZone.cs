using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyZone : MonoBehaviour {

    public GameObject Trap_Collider;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // if the gate is closed
            // tell the player that they should use the key
            // to open the door and press "Q"
            if (Trap_Collider.GetComponent<gateTrapScript>().gate_open == false)
            {
                // prompt on screen that they need key and press "Q"

                if (Input.GetButtonDown("Q"))
                {
                    // if they have enough keys use one
                    // open the door, if not tell them they need to find more keys
                    Trap_Collider.GetComponent<gateTrapScript>().Open();
                }
            }

        }
    }
}
