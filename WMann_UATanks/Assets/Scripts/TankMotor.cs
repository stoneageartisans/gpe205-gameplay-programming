using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(TankData))]
public class TankMotor : MonoBehaviour
{
    private CharacterController characterController;

    // Use this for initialization
    void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // This function moves our tank forward.
    public void Move(float speed)
    {
        // Create a vector to hold our speed data
        Vector3 speedVector;

        // Start with the vector pointing the same direction as the GameObject this script is on.
        speedVector = transform.forward;

        // Now, instead of our vector being 1 unit in length, apply our speed value
        speedVector *= speed;

        // Call SimpleMove() and send it our vector
        characterController.SimpleMove(speedVector);
    }

    // This function rotates our tank
    public void Rotate(float speed)
    {
        // Create a vector to hold our rotation data
        Vector3 rotateVector;

        // Start by rotating right by one degree per frame draw. Left is just "negative right".
        rotateVector = Vector3.up;

        // Adjust our rotation to be based on our speed
        rotateVector *= speed;

        // Transform.Rotate() doesn't account for speed, so we need to change our rotation to "per second" instead of "per frame."
        rotateVector *= Time.deltaTime;

        // Now, rotate our tank by this value - we want to rotate in our local space (not in world space).
        transform.Rotate(rotateVector, Space.Self);
    }
}
