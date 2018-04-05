using System.Collections;
using UnityEngine;

/// <summary>
/// Author: Hwan Kim
///
/// it will follow the player and will provide quarter view.
/// </summary>
public class CameraControllerV2 : MonoBehaviour
{
    public Transform player; //The offset of the camera to centrate the player in the X axis
    public float offsetX = -5; //The offset of the camera to centrate the player in the Z axis public
    private float offsetZ = 0; //The maximum distance permited to the camera to be far from the player, its used to make a smooth movement
    public float maximumDistance = 2; //The velocity of your player, used to determine que speed of the camera
    public float playerVelocity = 10;

    private Transform myTran;
    private float movementX;

    private float movementZ;
    private bool initialized = false;

    public void init(Transform player)
    {
        this.player = player;
        myTran = transform;
        initialized = true;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (initialized)
        {
            movementX = ((player.position.x + offsetX - myTran.position.x)) / maximumDistance; movementZ = ((player.position.z + offsetZ - myTran.position.z)) / maximumDistance;
            myTran.position += new Vector3((movementX * playerVelocity * Time.deltaTime), 0, (movementZ * playerVelocity * Time.deltaTime));
        }
    }
}