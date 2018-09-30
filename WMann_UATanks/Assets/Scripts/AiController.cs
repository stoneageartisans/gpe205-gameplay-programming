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
        Pursue,
        Stop
    };

    public enum PatrolType
    {
        OneWay,
        BackAndForth,
        Random
    };

    public float avoidanceDuration = 2;
    public Transform collisionSensorFront;
    public Transform collisionSensorRear;
    public bool continuousPatrolling = true;
    public float fleeDistance = 10;
    public PatrolType patrolType = PatrolType.OneWay;
    public Personality personality = Personality.Cowardly;
    public float targetProximity = 3;
    public float waypointPrecision = 1;

    int avoidanceStage;
    float currentAvoidTime;
    Mode currentMode;
    int currentWaypoint;
    TankData data;
    int direction;
    TankMotor motor;
    Mode previousMode;
    bool rotating;
    Transform target;
    Transform _transform;
    List<int> waypointList;

    // Use this for initialization
    void Start()
    {
        AssignWaypoints();
        avoidanceStage = 0;
        currentMode = Mode.Patrol;
        direction = 1;
        previousMode = currentMode;
        rotating = true;
        data = gameObject.GetComponent<TankData>();
        motor = gameObject.GetComponent<TankMotor>();
        _transform = gameObject.GetComponent<Transform>();
        
        // Set sprite based on personality
        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = GameManager.instance.aiSprites[(int) personality];
    }

    // Update is called once per frame
    void Update()
    {
        if(TargetDetected())
        {
            // Determine action based on personality
            switch(personality)
            {
                case Personality.Overconfident:
                    previousMode = currentMode;
                    currentMode = Mode.Pursue;
                    break;
                case Personality.Cowardly:
                    previousMode = currentMode;
                    currentMode = Mode.Flee;
                    break;
                case Personality.Cautious:
                    // Check health
                    if(data.health > (GameManager.instance.aiStartingHealth / 2))
                    {
                        // Still good
                        previousMode = currentMode;
                        currentMode = Mode.Pursue;
                    }
                    else
                    {
                        // Hurting
                        previousMode = currentMode;
                        currentMode = Mode.Flee;
                    }
                    break;
                case Personality.Erratic:
                    // Flip a coin - 1 is Heads, 2 is Tails
                    if(Random.Range(1, 2) == 1)
                    {
                        // Heads
                        previousMode = currentMode;
                        currentMode = Mode.Pursue;
                    }
                    else
                    {
                        // Tails
                        previousMode = currentMode;
                        currentMode = Mode.Flee;
                    }
                    break;
            }
        }

        switch(currentMode)
        {
            case Mode.Avoidance:
                AvoidObstacle();
                break;
            case Mode.Flee:
                FleeTarget();
                break;
            case Mode.Patrol:
                Patrol();
                break;
            case Mode.Pursue:
                PursueTarget();
                break;
            case Mode.Stop:
                // Do nothing
                break;
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
                for(int i = GameManager.instance.waypoints.Length - 2; i > (-1); i--)
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
            motor.Rotate(data.rotateSpeed);

            // If the tank can move, change to stage 2
            if(CanMove(direction * data.moveSpeed))
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
            if(CanMove(direction * data.moveSpeed))
            {
                // Update timer and move
                currentAvoidTime -= Time.deltaTime;
                motor.Move(direction * data.moveSpeed);

                // If we have moved long enough, return to chase mode
                if(currentAvoidTime <= 0)
                {
                    avoidanceStage = 0;
                    currentMode = previousMode;
                    if(currentMode == Mode.Patrol)
                    {
                        rotating = true;
                    }
                }
            }
            else
            {
                // Otherwise, we can't move forward, so back to stage 1
                avoidanceStage = 1;
            }
        }
    }

    bool CanMove(float distance)
    {
        bool result = true;

        RaycastHit2D hit;

        // Cast a ray in the direction of movement and for the specified distance
        if(direction > 0)
        {
            hit = Physics2D.Raycast(collisionSensorFront.position, (direction * _transform.right), distance);
        }
        else
        {
            hit = Physics2D.Raycast(collisionSensorRear.position, (direction * _transform.right), distance);
        }

        // If the raycast hit something...
        if(hit.collider != null)
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

    void EnterAvoidanceMode()
    {
        if(currentMode != Mode.Avoidance)
        {
            previousMode = currentMode;
            currentMode = Mode.Avoidance;
            avoidanceStage = 1;
        }
    }

    void FleeTarget()
    {
        // Set the player's tank as the target
        target = GameManager.instance.playerTransform;

        // Rotate towards the target
        motor.RotateTowards(target.position, data.rotateSpeed);

        if(InProximityOfTarget(fleeDistance))
        {
            if(CanMove(-data.moveSpeed))
            {
                // Move backward
                motor.Move(-data.moveSpeed);
            }
            else
            {
                EnterAvoidanceMode();
            }
        }
    }

    // Determines whether or not the tank is within the specified distance to the current target
    bool InProximityOfTarget(float distance)
    {
        return (Vector2.SqrMagnitude(target.position - _transform.position) < (distance * distance));
    }

    void Patrol()
    {
        // Set the target
        target = GameManager.instance.waypoints[waypointList[currentWaypoint]];

        // If we are close to the waypoint
        if(InProximityOfTarget(waypointPrecision))
        {
            // Advance waypoint
            currentWaypoint++;

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
                    currentMode = Mode.Stop;
                    rotating = false;
                }
            }
        }

        if(rotating)
        {
            // Rotate towards the target
            rotating = motor.RotateTowards(target.position, data.rotateSpeed);
        }
        else
        {
            // If still patrolling
            if(currentMode == Mode.Patrol)
            {
                if(CanMove(data.moveSpeed))
                {
                    // Move forward
                    motor.Move(data.moveSpeed);
                }
                else
                {
                    EnterAvoidanceMode();
                }
            }
        }
    }

    void PursueTarget()
    {
        // Set the player's tank as the target
        target = GameManager.instance.playerTransform;

        // Rotate towards the target
        motor.RotateTowards(target.position, data.rotateSpeed);

        // If not too close to target
        if(!InProximityOfTarget(targetProximity))
        {
            if(CanMove(data.moveSpeed))
            {
                // Move forward
                motor.Move(data.moveSpeed);
            }
            else
            {
                EnterAvoidanceMode();
            }
        }
    }

    // Randomly shuffles the list
    void ShuffleList<T>(List<T> list)
    {
        // Copy the list to a set of available items
        List<T> availableItems = new List<T>(list);

        // Iterate through the list
        for(int i = 0; i < list.Count; i++)
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

    bool TargetDetected()
    {
        bool result = false;

        if((currentMode == Mode.Patrol) || (currentMode == Mode.Stop))
        {
            // TODO: determine if a player tank was seen
        }

        return result;
    }
}
