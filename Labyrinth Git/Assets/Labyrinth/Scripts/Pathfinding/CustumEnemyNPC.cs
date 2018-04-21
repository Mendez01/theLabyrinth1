using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustumEnemyNPC : MonoBehaviour {

    // Dictates whether the agent waits on each node
    [SerializeField]
    bool _patrolWaiting;

    // The total time we wait at each node.
    [SerializeField]
    float _totalWaitTime = 3f;

    // The probability of switching direction.
    [SerializeField]
    float _switchProbability = 0.2f;

    // Private variables for base behaviour. 
    NavMeshAgent _navMeshAgent;
    ConnectedWaypoint _currentWaypoint;
    ConnectedWaypoint _previousWaypoint;

    // Name of waypoints you want to find 
    public string Waypoint_Tag;

    // to find player 
    public Transform player;
    public Transform head;
    private Animator anim;

    bool is_persuing;

    bool _traveling;
    bool _waiting;
    float _waitTimer;
    int _waypointsVisited;

    

    public void Start()
    {
        anim = this.GetComponent<Animator>();

        _navMeshAgent = this.GetComponent<NavMeshAgent>();

        

        if (_navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent component is not attached to " + gameObject.name);
        }
        else
        {
            if (_currentWaypoint == null)
            {
                // Set it at random.
                // Grab all waypoint objects in scene.
                // GameObject[] allWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");

                GameObject[] allWaypoints = GameObject.FindGameObjectsWithTag(Waypoint_Tag);

                if (allWaypoints.Length > 0)
                {
                    while (_currentWaypoint == null)
                    {
                        int random = UnityEngine.Random.Range(0, allWaypoints.Length);
                        ConnectedWaypoint startingWaypoint = allWaypoints[random].GetComponent<ConnectedWaypoint>();

                        // i.e we found a waypoint.
                        if (startingWaypoint != null)
                        {
                            _currentWaypoint = startingWaypoint;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Failed to find any waypoints for use in the scene.");
                }
            }
            SetDestination();
        }
    }
    public void Update()
    {
        Vector3 direction = player.position - this.transform.position;
        direction.y = 0;
        float angle = Vector3.Angle(direction, head.up);

        // if we found the player 
        // follow the player 

        Debug.Log("Distance: " + Vector3.Distance(player.position, this.transform.position));

        RaycastHit hit;

        // if (((Vector3.Distance(player.position, this.transform.position) < 15) && (_traveling || _waiting)) || (Physics.Raycast(transform.position, Vector3.forward, out hit, 300.0f))) {
        
        if (Vector3.Distance(player.position, this.transform.position) < 15) 
        {
            Debug.Log("Player Nearby");

            _patrolWaiting = false;
            _traveling = false;
            is_persuing = true;
            _waiting = false;

            
            GameObject player_waypoint = GameObject.Find("PlayerWaypoint");

            Debug.Log("New Waypoints" + player_waypoint);

            ConnectedWaypoint new_Waypoint = player_waypoint.GetComponent<ConnectedWaypoint>();

            _currentWaypoint = new_Waypoint;

            SetPersue();

            // this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);

            if (direction.magnitude > 2)
            {
                // this.transform.Translate(0, 0, Time.deltaTime * speed);
                anim.SetBool("isWalking", true);
                anim.SetBool("isAttacking", false);

            }
            else if (direction.magnitude < 2)
            {
                anim.SetBool("isAttacking", true);
                anim.SetBool("isWalking", false);
            }
        }
        else if ((Vector3.Distance(player.position, this.transform.position) > 15))
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isAttacking", false);
            is_persuing = false;
            _traveling = true;

            // check if we're close to the destination.
            if (_traveling && _navMeshAgent.remainingDistance <= 1.0f)
            {
                _traveling = false;
                _waypointsVisited++;

                // If we're going to wait, then wait.
                if (_patrolWaiting)
                {
                    anim.SetBool("isWaiting", true);
                    _waiting = true;
                    _waitTimer = 0f;
                }
                else
                {
                    anim.SetBool("isWaiting", false);
                    SetDestination();
                }
            }

            // Instead if we're waiting.
            if (_waiting)
            {
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= _totalWaitTime)
                {
                    _waiting = false;

                    SetDestination();
                }
            }
            /*
            GameObject[] allWaypoints = GameObject.FindGameObjectsWithTag("Waypoint");

            int random = UnityEngine.Random.Range(0, allWaypoints.Length);
            ConnectedWaypoint startingWaypoint = allWaypoints[random].GetComponent<ConnectedWaypoint>();

            // i.e we found a waypoint.
            if (startingWaypoint != null)
            {
                _currentWaypoint = startingWaypoint;
            }

            SetDestination();
            */
        }
        
        

    }

    private void SetDestination()
    {
        if (_waypointsVisited > 0)
        {
            ConnectedWaypoint nextWaypoint = _currentWaypoint.NextWaypoint(_previousWaypoint);
            _previousWaypoint = _currentWaypoint;
            _currentWaypoint = nextWaypoint;
        }

        Vector3 targetVector = _currentWaypoint.transform.position;
        _navMeshAgent.SetDestination(targetVector);
        _traveling = true;
    }

    private void SetPersue()
    {
        

        Vector3 targetVector = _currentWaypoint.transform.position;
        _navMeshAgent.SetDestination(targetVector);
        _traveling = false;
        is_persuing = true;
    }
}
