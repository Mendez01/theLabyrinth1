using System.Collections;
using UnityEngine;

public class WanderController : MonoBehaviour
{
    private Transform myTran;
    private EnemyController enemy;
    public float maxStamina;
    public float currentStamina;
    private float speed = 5;
    public float directionChangeInterval = 2.5f;
    private float maxHeadingChange = 30;
    private float heading = 0;
    public bool debugOn = false;
    private bool initialized = false;
    private CapsuleCollider capCol;

    public void init(CapsuleCollider capCol)
    {
        if (maxStamina == 0)
            maxStamina = Random.Range(25, 100);
        currentStamina = maxStamina;
        enemy = GetComponent<EnemyController>();
        myTran = transform;
        // Set random initial rotation
        transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
        // look in a random direction at start of frame.
        this.capCol = capCol;
        initialized = true;
    }

    private void FixedUpdate()
    {
        if (initialized && enemy.characterInfo.isAlive)
        {
            if (!enemy.targetInitialized)
                proceeSeekingPath();
            else if (enemy.targetInitialized)
            {
                //path finding required.
                enemy.moveToTarget();
            }
        }
    }

    public float offset = -2;
    public float currentDegree = 0;
    public float minDegreeToTurn = -300;
    public float maxDegreeToTurn = 300;
    private bool tired = false;

    private void proceeSeekingPath()
    {
        if (!tired)
        {
            if (seekForThePathFoward())
            {
                enemy.moveFoward();
            }
            else
            {
                float nextDegreeToTurn = getNextDegree();
                if (nextDegreeToTurn == maxDegreeToTurn || nextDegreeToTurn == minDegreeToTurn)
                {
                    currentDegree = 0;
                    tired = true;
                    currentStamina = maxStamina / 2;
                    offset *= Random.Range(0, 2) == 0 ? -1 : 1;
                }
                else
                    enemy.rotateY(nextDegreeToTurn);
            }
            currentStamina -= 0.1f;
            if (currentStamina < 0)
            {
                tired = true;
                enemy.stop();
            }
        }
        else
        {
            currentStamina += 0.2f;
            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
                tired = false;
            }
        }
    }

    private float getNextDegree()
    {
        float nextDegree = currentDegree + offset;
        if (nextDegree < minDegreeToTurn)
        {
            offset *= -1;
            nextDegree = minDegreeToTurn;
        }
        else if (nextDegree > maxDegreeToTurn)
        {
            offset *= -1;
            nextDegree = maxDegreeToTurn;
        }
        currentDegree = nextDegree;
        return nextDegree;
    }

    private bool seekForThePathFoward()
    {
        bool pathExist = false;
        RaycastHit hit;
        Vector3 eyeLocation = myTran.position;
        eyeLocation.y += 0.5f;
        if (debugOn)
        {
            Debug.DrawRay(eyeLocation, myTran.forward * 50, Color.yellow);
        }

        if (Physics.SphereCast(eyeLocation, capCol.radius, myTran.forward, out hit, 50))
        {
            //check the distance.
            float distance = Vector3.Distance(myTran.position, hit.point);
            pathExist = distance > 3;
            if (debugOn && !pathExist)
            {
                Debug.DrawRay(eyeLocation, hit.point, Color.red);
            }
        }
        else
            pathExist = true;
        return pathExist;
    }
}