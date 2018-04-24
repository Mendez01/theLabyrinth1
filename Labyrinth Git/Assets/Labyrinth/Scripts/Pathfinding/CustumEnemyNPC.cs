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

    // health

    public float health = 100f;

    // Name of waypoints you want to find 
    public string Waypoint_Tag;

    // to find player 
    public Transform player;
    public Transform head;

    // for other stuff

    public bool _dead = false;

    public GameObject[] SpawnOnDeadItems;

    private Animator anim;

    bool is_persuing;

    bool _traveling;
    bool _waiting;
    float _waitTimer;
    int _waypointsVisited;

    private GameObject[] allWaypoints;

    private float count;

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

                allWaypoints = GameObject.FindGameObjectsWithTag(Waypoint_Tag);

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

        if (health == 0)
        {
            _dead = true;
        }

        if (_dead == true)
        {
            count++;

            _currentWaypoint = null;
            
            anim.SetBool("isWalking", false);
            anim.SetBool("isAttacking", false);
            anim.SetBool("isPersuing", false);
            anim.SetBool("isHit", false);
            anim.SetBool("isWaiting", false);
            anim.SetBool("isDead", true);

            if ((SpawnOnDeadItems != null) && count == 1)
            {
                // Vector3 current_pos = this.transform.position;
                Invoke("SpawnItem", 4);
                
            }

            Destroy(gameObject, 4);
       
        }

        // if (((Vector3.Distance(player.position, this.transform.position) < 15) && (_traveling || _waiting)) || (Physics.Raycast(transform.position, Vector3.forward, out hit, 300.0f))) {
        
        else if ((Vector3.Distance(player.position, this.transform.position) < 15) && _dead == false)
        {
            Debug.Log("Player Nearby");

            _patrolWaiting = false;
            _traveling = false;
            is_persuing = true;
            _waiting = false;

            // speed up a little 
            _navMeshAgent.speed = 4f;

            
            GameObject player_waypoint = GameObject.Find("PlayerWaypoint");

            Debug.Log("New Waypoints" + player_waypoint);

            ConnectedWaypoint new_Waypoint = player_waypoint.GetComponent<ConnectedWaypoint>();

            _currentWaypoint = new_Waypoint;

            SetPersue();

            //this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), rotSpeed * Time.deltaTime);

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 3 * Time.deltaTime);

            if ((direction.magnitude > 2) && _dead == false)
            {
                // too far to attack 

                // this.transform.Translate(0, 0, Time.deltaTime * speed);
                anim.SetBool("isWalking", true);
                anim.SetBool("isAttacking", false);

            }
            else if ((direction.magnitude < 2) && _dead == false)
            {
                // within attacking range

                anim.SetBool("isAttacking", true);
                anim.SetBool("isWalking", false);
            }
        }
        else if (((Vector3.Distance(player.position, this.transform.position) > 15)) && _dead == false)
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isAttacking", false);
            is_persuing = false;
            _traveling = true;

            // normal patrol speed
            _navMeshAgent.speed = 3.5f;

            

            // if you have not found the player
            // if you don't already have a set destination 


            while (_currentWaypoint == null)
            {
                // set waypoints visited to 0 if you went through all of them and start 
                // patrolling all nearby waypoints again
                _waypointsVisited = 0;

                int random = UnityEngine.Random.Range(0, allWaypoints.Length);
                ConnectedWaypoint startingWaypoint = allWaypoints[random].GetComponent<ConnectedWaypoint>();

                _currentWaypoint = startingWaypoint;

                SetDestination();
            }

            // check if we're close to the destination.
            if ((_traveling && _navMeshAgent.remainingDistance <= 1.0f) && _dead == false)
            {
                _traveling = false;
                _waypointsVisited++;

                // If we're going to wait, then wait.
                if (_patrolWaiting)
                {
                    
                    // anim.SetBool("isWaiting", true);
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
            if ((_waiting) && _dead == false)
            {
                // anim.SetBool("isWaiting", true);
                _waitTimer += Time.deltaTime;
                if (_waitTimer >= _totalWaitTime)
                {
                    anim.SetBool("isWaiting", false);
                    _waiting = false;
                    _patrolWaiting = false;
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
        is_persuing = false;
        anim.SetBool("isPersuing", false);
        _traveling = true;
        

        /*
        // chance of waiting while patrol after destination

        float wait_chance = Random.Range(0, 100);

        if (wait_chance > 70)
        {
            _patrolWaiting = true;
            
        }
        */
    }

    private void SetPersue()
    {
       
        Vector3 targetVector = _currentWaypoint.transform.position;
        _navMeshAgent.SetDestination(targetVector);
        _traveling = false;
        is_persuing = true;
        anim.SetBool("isPersuing", true);
    }

    private void SpawnItem()
    {
        for (int i = 0; i < SpawnOnDeadItems.Length; i++)
        {
            int rand_y = Random.Range(1, 4);
            Vector3 pos = Vector3.up * rand_y;
            Instantiate(SpawnOnDeadItems[i], (pos + transform.position), transform.rotation);
        }
        
    }
}
