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

    public float avoidanceDuration = 2.5f;
    public Transform collisionSensorFront;
    public Transform collisionSensorRear;
    public bool continuousPatrolling = true;
    public float fleeDistance = 10;
    public PatrolType patrolType = PatrolType.OneWay;
    public Personality personality = Personality.Cowardly;
    public float targetProximity = 3;
    public float waypointPrecision = 1;

    int avoidanceDirection;
    int avoidanceStage;
    TankCannon cannon;
    float currentAvoidTime;
    Mode currentMode;
    int currentWaypoint;
    TankData data;
    int direction;
    TankMotor motor;
    Mode previousMode;
    bool rotating;
    Transform target;
    bool targetDetected;
    List<int> waypointList;

    // Use this for initialization
    void Start()
    {
        AssignWaypoints();

        avoidanceDirection = 0;
        avoidanceStage = 0;
        currentMode = Mode.Patrol;
        direction = 1;
        previousMode = currentMode;
        rotating = true;
        targetDetected = false;

        data = gameObject.GetComponent<TankData>();
        data.health = GameManager.instance.aiStartingHealth;
        data.moveSpeed = GameManager.instance.aiMoveSpeed;
        data.rateOfFire = GameManager.instance.rateOfFire;
        data.rotateSpeed = GameManager.instance.aiRotateSpeed;

        cannon = gameObject.GetComponentInChildren<TankCannon>();
        motor = gameObject.GetComponent<TankMotor>();

        // Set sprite based on personality
        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = GameManager.instance.aiSprites[(int) personality];
    }

    // Update is called once per frame
    void Update()
    {
        if(targetDetected)
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
                    if(Random.Range(1, 3) == 1)
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

            targetDetected = false;
        }

        switch(currentMode)
        {
            case Mode.Avoidance:
                AvoidObstacle();
                break;
            case Mode.Flee:
                direction = (-1);
                FleeTarget();
                break;
            case Mode.Patrol:
                Patrol();
                break;
            case Mode.Pursue:
                direction = 1;
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
            motor.PauseMoveSound();
            motor.Rotate(avoidanceDirection * data.rotateSpeed);
            motor.PlayRotateSound();

            // If the tank can move, change to stage 2
            if(CanMove(direction * data.moveSpeed))
            {
                motor.PauseRotateSound();
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
                motor.PlayMoveSound();

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
        // By default the tank can move
        bool result = true;

        RaycastHit2D hit;

        // Cast a ray in the direction of movement and for the specified distance
        if(direction > 0)
        {
            hit = Physics2D.Raycast(collisionSensorFront.position, (direction * data._transform.right), distance);
        }
        else
        {
            hit = Physics2D.Raycast(collisionSensorRear.position, (direction * data._transform.right), distance);
        }

        // If the raycast hit something...
        if(hit.collider != null)
        {
            // ...and it's a wall
            if(hit.collider.name.ToLower().Contains("tile"))
            {
                // ...then it can't move
                result = false;
            }
        }

        return result;
    }

    public void ClearTarget()
    {
        currentMode = Mode.Patrol;
    }

    void EnterAvoidanceMode()
    {
        if(currentMode != Mode.Avoidance)
        {
            previousMode = currentMode;
            currentMode = Mode.Avoidance;
            avoidanceStage = 1;

            // If this is the first time avoiding an obstacle
            if(avoidanceDirection == 0)
            {
                // Randomly determine starting rotation direction
                switch(Random.Range(1, 3))
                {
                    case 1:
                        // Left
                        avoidanceDirection = 1;
                        break;
                    case 2:
                        // Right
                        avoidanceDirection = (-1);
                        break;
                }
            }
            else
            {
                // Switch the direction of rotation
                avoidanceDirection *= (-1);
            }
        }
    }

    void ExecuteFiringSolution()
    {
        // If the target is withing firing range
        if(Vector2.Distance(data._transform.position, target.position) <= GameManager.instance.aiFiringRange)
        {
            // Find the rotation that points toward the target
            Vector3 vectorToTarget = target.position - data._transform.position;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Check if the tank's facing is within it's aim accuracy
            if(Quaternion.Angle(data._transform.rotation, targetRotation) <= GameManager.instance.aiAccuracyInDegrees)
            {
                // FIRE!
                cannon.FireMissile(data.owner);
            }
        }
    }

    void FleeTarget()
    {
        // Set the player's tank as the target
        target = GameManager.instance.playerTransform;        

        // Rotate towards the target
        motor.RotateTowards(target.position, data.rotateSpeed);

        ExecuteFiringSolution();

        if(InProximityOfTarget(fleeDistance))
        {
            if(CanMove(-data.moveSpeed))
            {
                motor.PauseRotateSound();

                // Move backward
                motor.Move(-data.moveSpeed);
                motor.PlayMoveSound();
            }
            else
            {
                EnterAvoidanceMode();
                motor.PauseMoveSound();
            }
        }
    }

    // Determines whether or not the tank is within the specified distance to the current target
    bool InProximityOfTarget(float distance)
    {
        bool result = false;

        if(Vector2.Distance(data._transform.position, target.position) < distance)
        {
            result = true;
        }

        return result;
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
            motor.PauseMoveSound();

            // Rotate towards the target
            rotating = motor.RotateTowards(target.position, data.rotateSpeed);
        }
        else
        {
            motor.PauseRotateSound();

            // If still patrolling
            if(currentMode == Mode.Patrol)
            {
                if(CanMove(data.moveSpeed))
                {
                    // Make any necessary course corrections
                    motor.RotateTowards(target.position, data.rotateSpeed);

                    // Move forward
                    motor.Move(data.moveSpeed);
                    motor.PlayMoveSound();
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

        ExecuteFiringSolution();

        // If not too close to target
        if(!InProximityOfTarget(targetProximity))
        {
            if(CanMove(data.moveSpeed))
            {
                motor.PauseRotateSound();

                // Move forward
                motor.Move(data.moveSpeed);
                motor.PlayMoveSound();
            }
            else
            {
                motor.PauseMoveSound();
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

    public void TargetWasDetected()
    {
        if((currentMode == Mode.Patrol) || (currentMode == Mode.Stop))
        {
            if(targetDetected == false)
            {
                targetDetected = true;
            }
        }
    }
}
