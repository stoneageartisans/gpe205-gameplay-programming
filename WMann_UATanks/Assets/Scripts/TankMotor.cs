using UnityEngine;

public class TankMotor : MonoBehaviour
{
    TankData data;

    // Use this for initialization
    void Start()
    {
        data = gameObject.GetComponent<TankData>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO
    }

    // This function moves the tank
    public void Move(float speed)
    {
        // Translate the tank along the local x axis by speed and time
        data._transform.Translate(Vector3.right * speed * Time.deltaTime, Space.Self);
    }

    // This function rotates the tank
    public void Rotate(float speed)
    {
        // Rotate the tank along the local z axis by speed and time
        data._transform.Rotate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }

    // This function rotates the tank towards a target point
    // It returns true if still rotating, or false if tank is facing target 
    public bool RotateTowards(Vector3 target, float speed)
    {
        bool result = true;

        // Find the rotation that points toward the target
        Vector3 vectorToTarget = target - data._transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Check if the tank is facing the target
        if(data._transform.rotation.z == targetRotation.z)
        {
            // Done rotating
            result = false;
        }
        else
        {
            // Rotate the tank this frame
            data._transform.rotation = Quaternion.RotateTowards(data._transform.rotation, targetRotation, speed * Time.deltaTime);

            // Still rotating
            result = true;
        }

        return result;
    }
}
