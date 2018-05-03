using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryLightScript : MonoBehaviour {


    private float time_limit = 13;

    float time_passed;

    float start_time; 


	// Use this for initialization
	void Start () {

        start_time = Time.time;

	}
	
	// Update is called once per frame
	void Update () {

        time_passed = (Time.time - start_time);

        Debug.Log(time_passed);
        
        if (time_passed > time_limit)
        {
            Destroy(gameObject);
        }
	}
}
