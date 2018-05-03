using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCMove : MonoBehaviour {

    GameObject _destination;

    NavMeshAgent _navMeshAgent;

	// Use this for initialization
	void Start () {

        _destination = GameObject.Find("Goal");

        _navMeshAgent = this.GetComponent<NavMeshAgent>();

        if (_navMeshAgent == null)
        {
            Debug.Log("The nav mesh agent is not attached to " + gameObject.name);
        }
        else
        {
            SetDestination();
        }
	}

    private void SetDestination()
    {
        if (_destination != null)
        {
            Vector3 targetVector = _destination.transform.position;
            _navMeshAgent.SetDestination(targetVector);
        }
    }
}
