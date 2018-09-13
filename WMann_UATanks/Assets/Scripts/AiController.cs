using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
    public float waypointPrecision = 1;

    private int currentWaypoint;
    private bool rotating;
    private TankMotor tankMotor;
    private TankData tankData;

    // Use this for initialization
    void Start()
    {
        currentWaypoint = 0;
        rotating = true;
        tankMotor = gameObject.GetComponent<TankMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        if(rotating)
        {
            // Rotate towards target point
            rotating = tankMotor.RotateTowards(GameManager.instance.waypoints[currentWaypoint].position, GameManager.instance.tankTurnSpeed);
        }
        else
        {
            // Move forward
            tankMotor.Move(GameManager.instance.tankMoveSpeed);
        }

        // If we are close to the waypoint
        if(TargetReached())
        {
            // Advance to the next waypoint
            currentWaypoint ++;

            if(currentWaypoint >= GameManager.instance.waypoints.Length)
            {
                currentWaypoint = 0;
            }

            rotating = true;
        }
    }

    bool TargetReached()
    {
        return (Vector3.Distance(transform.position, GameManager.instance.waypoints[currentWaypoint].position) < waypointPrecision);
    }
}
