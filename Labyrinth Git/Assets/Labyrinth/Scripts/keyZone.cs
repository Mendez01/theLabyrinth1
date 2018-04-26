using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class keyZone : MonoBehaviour {

    public GameObject Trap_Collider;

    public Text Instructions;

    private void Start()
    {
        AudioSource Open_Sound = GetComponent<AudioSource>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // if the gate is closed
            // tell the player that they should use the key
            // to open the door and press "q"
            if (Trap_Collider.GetComponent<gateTrapScript>().gate_open == false)
            {
                // prompt on screen that they need key and press "q"

                Instructions.GetComponent<Text>().text = "Find a key and press q to proceed";

                if (Input.GetKey("q"))
                {
                    // if they have enough keys use one
                    // open the door, if not tell them they need to find more keys
                    Trap_Collider.GetComponent<gateTrapScript>().Open();
                    Trap_Collider.GetComponent<gateTrapScript>().gate_open = true;

                    AudioSource audio = GetComponent<AudioSource>();

                    if (audio != null)
                    {
                        audio.Play();
                    }
                  
                    Debug.Log("Entered");

                    /*
                    if (Open_Sound != null)
                    {
                        Open_Sound.Play();
                    }
                    */
                    // else if there are not enough keys play a incorrect sound
                    // tell them to find a key 
                    // Instructions.text = "You do not have a key. Find a key"

                }
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        Instructions.GetComponent<Text>().text = "";
    }
}
