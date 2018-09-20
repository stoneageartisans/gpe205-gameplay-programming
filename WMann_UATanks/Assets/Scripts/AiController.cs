using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour
{
    public enum Personality
    {
        Cautious,
        Cowardly,
        Erratic,
        Overconfident        
    };

    public enum Mode
    {
        Avoidance,
        Flee,
        Patrol,
        Pursue
    };

    public enum PatrolType
    {
        OneWay,
        BackAndForth,
        Random
    };

    public Personality personality = Personality.Cowardly;
    public float avoidanceDuration = 2;
    public bool continuousPatrolling = true;
    public float fleeDistance = 10;
    public PatrolType patrolType = PatrolType.OneWay;
    public float targetProximity = 3;
    public float waypointPrecision = 1;

    private int avoidanceStage;
    private float currentAvoidTime;
    private Mode currentMode;
    private int currentWaypoint;
    private int direction;
    private Mode previousMode;
    private bool rotating;
    private TankData tankData;
    private TankMotor tankMotor;
    private Transform target;
    private Transform _transform;
    private List<int> waypointList;

    // Use this for initialization
    void Start()
    {
        AssignWaypoints();
        avoidanceStage = 0;
        currentMode = Mode.Patrol;
        direction = 1;
        previousMode = currentMode;
        rotating = true;
        tankData = gameObject.GetComponent<TankData>();
        tankMotor = gameObject.GetComponent<TankMotor>();
        _transform = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentMode == Mode.Patrol)
        {
            // If we are close to the waypoint
            if(InProximityOfTarget(waypointPrecision))
            {
                // Advance waypoint
                currentWaypoint ++;

                // Set the tank to rotate
                rotating = true;

                // If all waypoints have been traversed
                if(currentWaypoint == waypointList.Count)
                {
                    if(continuousPatrolling)
                    {
                        // Reset back to the beginning
                        currentWaypoint = 0;

                        // Shuffle the waypoints
                        if(patrolType == PatrolType.Random)
                        {
                            ShuffleList(waypointList);
                        }
                    }
                    else // Stop
                    {
                        rotating = false;
                    }
                }

                // Set the new target
                target = GameManager.instance.waypoints[waypointList[currentWaypoint]];
            }

            if(rotating)
            {
                // Rotate towards the target
                rotating = tankMotor.RotateTowards(target.position, GameManager.instance.tankTurnSpeed);
            }
            else
            {
                if(CanMove(direction * GameManager.instance.enemyMoveSpeed))
                {
                    // Move forward
                    tankMotor.Move(GameManager.instance.enemyMoveSpeed);
                }
                else
                {
                    previousMode = currentMode;
                    currentMode = Mode.Avoidance;
                    avoidanceStage = 1;
                }
            }
        }
        else
        {
            if(currentMode == Mode.Avoidance)
            {
                AvoidObstacle();
            }
            else
            {
                // Determine action based on personality
                switch(personality)
                {
                    case Personality.Overconfident:
                        PursueTarget();
                        break;
                    case Personality.Cowardly:
                        FleeTarget();
                        break;
                    case Personality.Cautious:
                        if(tankData.health > (GameManager.instance.tankStartingHealth / 2))
                        {
                            PursueTarget();
                        }
                        else
                        {
                            FleeTarget();
                        }    
                        break;
                    case Personality.Erratic:
                        // Flip a coin - 1 is Heads, 2 is Tails
                        if(Random.Range(1, 2) == 1)
                        {
                            // Heads
                            PursueTarget();
                        }
                        else
                        {
                            // Tails
                            FleeTarget();
                        }
                        break;
                }
            }
        }
    }

    void AssignWaypoints()
    {
        currentWaypoint = 0;

        waypointList = new List<int>();

        switch(patrolType)
        {
            case PatrolType.OneWay:
                // Add the waypoints in order
                for(int i = 0; i < GameManager.instance.waypoints.Length; i++)
                {
                    waypointList.Add(i);
                }
                break;
            case PatrolType.BackAndForth:
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
            case PatrolType.Random:
                // Add the waypoints in order
                for(int i = 0; i < GameManager.instance.waypoints.Length; i++)
                {
                    waypointList.Add(i);
                }
                // Shuffle the list
                ShuffleList(waypointList);
                break;
        }

        target = GameManager.instance.waypoints[waypointList[currentWaypoint]];
    }

    void AvoidObstacle()
    {
        if(avoidanceStage == 1)
        {
            // Rotate left
            tankMotor.Rotate(-GameManager.instance.tankTurnSpeed);

            // If the tank can move, change to stage 2
            if(CanMove(direction * GameManager.instance.enemyMoveSpeed))
            {
                avoidanceStage = 2;

                // Set the number of seconds we will stay in Stage2
                currentAvoidTime = avoidanceDuration;
            }

            // Otherwise, we'll do this again next turn!
        }
        else if(avoidanceStage == 2)
        {
            // If the tank can move, do so
            if(CanMove(direction * GameManager.instance.enemyMoveSpeed))
            {
                // Update timer and move
                currentAvoidTime -= Time.deltaTime;
                tankMotor.Move(direction * GameManager.instance.enemyMoveSpeed);

                // If we have moved long enough, return to chase mode
                if(currentAvoidTime <= 0)
                {
                    currentMode = previousMode;
                    avoidanceStage = 0;
                }
            }
            else
            {
                // Otherwise, we can't move forward, so back to stage 1
                avoidanceStage = 1;
            }
        }
    }

    bool CanMove(float speed)
    {
        bool result = true;

        // Cast a ray in the direction of movement and for the specified distance
        RaycastHit hit;

        // If the raycast hit something...
        if(Physics.Raycast(_transform.position, (direction * _transform.forward), out hit, (speed * 2)))
        {
            // ...then the tank can't move.
            result = false;

            // Unless it was a player tank... 
            if(hit.collider.name.ToLower().Contains("player"))
            {
                // ...then it CAN actually move
                result = true;
            }
        }

        return result;
    }

    void FleeTarget()
    {
        // Set the player's tank as the target
        target = GameManager.instance.playerTransform;

        // Rotate towards the target
        tankMotor.RotateTowards(target.position, GameManager.instance.tankTurnSpeed);

        if(InProximityOfTarget(fleeDistance))
        {
            if(CanMove(-GameManager.instance.enemyMoveSpeed))
            {
                // Move backward
                tankMotor.Move(-GameManager.instance.enemyMoveSpeed);
            }
            else
            {
                previousMode = currentMode;
                currentMode = Mode.Avoidance;
                avoidanceStage = 1;
            }
        }
    }

    void PursueTarget()
    {
        // Set the player's tank as the target
        target = GameManager.instance.playerTransform;

        // Rotate towards the target
        tankMotor.RotateTowards(target.position, GameManager.instance.tankTurnSpeed);

        // If not too close to target
        if(!InProximityOfTarget(targetProximity))
        {
            if(CanMove(GameManager.instance.enemyMoveSpeed))
            {
                // Move forward
                tankMotor.Move(GameManager.instance.enemyMoveSpeed);
            }
            else
            {
                previousMode = currentMode;
                currentMode = Mode.Avoidance;
                avoidanceStage = 1;
            }
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

    // Determines whether or not the tank is within the specified distance to the current target
    bool InProximityOfTarget(float distance)
    {
        return (Vector3.SqrMagnitude(target.position - _transform.position) < (distance * distance));
    }
}
