using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
    public enum WaypointTraverseType
    {
        OneWay,
        BackAndForth,
        Random
    };

    public bool continuousTraversal = true;
    public float waypointPrecision = 1;
    public WaypointTraverseType waypointTraverseType = WaypointTraverseType.OneWay;

    private int currentWaypoint;
    private bool rotating;
    private bool patrolling;
    private TankMotor tankMotor;
    private TankData tankData;
    private List<int> waypointList;

    // Use this for initialization
    void Start()
    {
        AssignWaypoints();
        rotating = true;
        patrolling = true;
        tankMotor = gameObject.GetComponent<TankMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        // If still going
        if(patrolling)
        {
            // If we are close to the waypoint
            if(TargetReached())
            {
                // Advance to the next waypoint
                currentWaypoint ++;

                // Set the tank to rotate
                rotating = true;

                // If all waypoints have been traversed
                if(currentWaypoint == waypointList.Count)
                {
                    if(continuousTraversal)
                    {
                        // Reset back to the beginning
                        currentWaypoint = 0;

                        // Shuffle the waypoints
                        if(waypointTraverseType == WaypointTraverseType.Random)
                        {
                            ShuffleList(waypointList);
                        }
                    }
                    else // Stop
                    {
                        patrolling = false;
                        rotating = false;
                    }                
                }
            }
        
            if(rotating)
            {
                // Rotate towards target point
                rotating = tankMotor.RotateTowards(GameManager.instance.waypoints[waypointList[currentWaypoint]].position, GameManager.instance.tankTurnSpeed);
            }
            else
            {
                // Move forward
                tankMotor.Move(GameManager.instance.tankMoveSpeed);
            }
        }
    }

    void AssignWaypoints()
    {
        currentWaypoint = 0;

        waypointList = new List<int>();

        switch(waypointTraverseType)
        {
            case WaypointTraverseType.OneWay:
                // Add the waypoints in order
                for(int i = 0; i < GameManager.instance.waypoints.Length; i++)
                {
                    waypointList.Add(i);
                }
                break;
            case WaypointTraverseType.BackAndForth:
                // Add the waypoints in order
                for(int i = 0; i < GameManager.instance.waypoints.Length; i++)
                {
                    waypointList.Add(i);
                }
                // Add the waypoints in reverse order (omitting the "pivot" waypoint)
                for(int i = GameManager.instance.waypoints.Length - 2; i > -1; i --)
                {
                    waypointList.Add(i);
                }
                break;
            case WaypointTraverseType.Random:
                // Add the waypoints in order
                for(int i = 0; i < GameManager.instance.waypoints.Length; i++)
                {
                    waypointList.Add(i);
                }
                // Shuffle the list
                ShuffleList(waypointList);
                break;
        }
    }

    // Randomly shuffles the list
    void ShuffleList<T>(List<T> list)
    {
        // Copy the list to a set of available items
        List<T> availableItems = new List<T>(list);

        // Iterate through the list
        for(int i = 0; i < list.Count; i ++)
        {
            // Randomly select from available items
            int r = Random.Range(0, availableItems.Count);

            // Add the item to list
            list[i] = availableItems[r];

            // Remove the item from the set of available items
            availableItems.RemoveAt(r);

            // Resize the set of available items (this may not be necessary)
            availableItems.TrimExcess();
        }        
    }

    // Determines whether or not the tank has reached the current target (based on precision)
    bool TargetReached()
    {
        return (Vector3.SqrMagnitude(GameManager.instance.waypoints[waypointList[currentWaypoint]].position - transform.position) < (waypointPrecision * waypointPrecision));
    }
}
