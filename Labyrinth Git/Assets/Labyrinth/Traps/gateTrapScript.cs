using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gateTrapScript : MonoBehaviour {

    public bool gate_open = true;

    public float gatefalltime = 10f;

    public GameObject gateTrap;

    public GameObject keyZone;

    private int open_pos_x = 16;

    public void Start()
    {
        if (gate_open == false)
        {
            Close();
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // if the gate is open
            // close it behind him
            if (gate_open == true)
            {
                Close();
            }
            
        }
    }

    public void Open()
    {
        gateTrap.transform.Translate(16, 0, 0);
    }

    public void Close()
    {
        gateTrap.transform.Translate(-16, 0, 0);
    }
}
