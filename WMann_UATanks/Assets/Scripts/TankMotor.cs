using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(TankData))]
public class TankMotor : MonoBehaviour
{
    private CharacterController characterController;
    private Transform _transform;

    // Use this for initialization
    void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();
        _transform = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // This function moves our tank
    public void Move(float speed)
    {
        // Call SimpleMove() with the tank's vector and speed
        characterController.SimpleMove(_transform.forward * speed);
    }

    // This function rotates our tank
    public void Rotate(float speed)
    {
        // Rotate our tank's vector in local space by speed and time
        _transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.Self);
    }

    // This function rotates the tank towards a target point
    // It returns true if still rotating, or false if tank is facing target 
    public bool RotateTowards(Vector3 target, float speed)
    {
        // Find the rotation that points toward the target 
        Quaternion targetRotation = Quaternion.LookRotation(target - _transform.position);

        // Check if the tank is facing the target
        if(targetRotation == _transform.rotation)
        {
            // Done rotating
            return false;
        }
        else
        {
            // Rotate the tank this frame
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation, targetRotation, speed * Time.deltaTime);

            // Still rotating
            return true;
        }
    }
}
