using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedWaypoint : Waypoint {

    [SerializeField]
    protected float _connectivityRadius = 50f;

    // Name of waypoints you want to find 
    public string Waypoint_Tag;

    List<ConnectedWaypoint> _connections;

    public void Start()
    {
        // Grab all waypoint objects in scene.
        GameObject[] allWaypoints = GameObject.FindGameObjectsWithTag(Waypoint_Tag);

        // Create a list of waypoints I can refer to later.
        _connections = new List<ConnectedWaypoint>();

        // check if they're a connected waypoint.
        for (int i = 0; i < allWaypoints.Length; i++)
        {
            ConnectedWaypoint nextWaypoint = allWaypoints[i].GetComponent<ConnectedWaypoint>();
            
            // i.e we foind a waypoint.
            if (nextWaypoint != null)
            {
                if (Vector3.Distance(this.transform.position, nextWaypoint.transform.position) <= _connectivityRadius && nextWaypoint != this)
                {
                    _connections.Add(nextWaypoint);
                }
            }
        }
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, debugDrawRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _connectivityRadius);

    }

    public ConnectedWaypoint NextWaypoint(ConnectedWaypoint previousWaypoint)
    {
        if (_connections.Count == 0)
        {
            // No waypoints? Return null and complain.
            Debug.LogError("Insufficient waypoint count.");
            return null;
        }
        else if (_connections.Count == 1 && _connections.Contains(previousWaypoint))
        {
            // only one waypoint and it's the previous one? Just use the previous one.
            return previousWaypoint;
        }
        else // Otherwise, find a random one that isn't the previous one
        {
            ConnectedWaypoint nextWaypoint;
            int nextIndex = 0;

            do
            {
                nextIndex = UnityEngine.Random.Range(0, _connections.Count);
                nextWaypoint = _connections[nextIndex];

            } while (nextWaypoint == previousWaypoint);
                return nextWaypoint;
            
        }
    }
}
