using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum InputScheme
    {
        WASD,
        arrowKeys
    };

    public InputScheme inputScheme = InputScheme.WASD;

    TankCannon cannon;
    TankData data;
    TankMotor motor;

    // Use this for initialization
    void Start()
    {
        cannon = gameObject.GetComponentInChildren<TankCannon>();
        data = gameObject.GetComponent<TankData>();
        motor = gameObject.GetComponent<TankMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(inputScheme)
        {
            case InputScheme.WASD:
                if(Input.GetKey(KeyCode.W))
                {
                    motor.Move(data.moveSpeed);
                }
                if(Input.GetKey(KeyCode.S))
                {
                    motor.Move(-data.moveSpeed);
                }
                if(Input.GetKey(KeyCode.D))
                {
                    motor.Rotate(-data.rotateSpeed);
                }
                if(Input.GetKey(KeyCode.A))
                {
                    motor.Rotate(data.rotateSpeed);
                }
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    cannon.FireMissile(data.owner);
                }
                break;
            case InputScheme.arrowKeys:
                if(Input.GetKey(KeyCode.UpArrow))
                {
                    motor.Move(data.moveSpeed);
                }
                if(Input.GetKey(KeyCode.DownArrow))
                {
                    motor.Move(-data.moveSpeed);
                }
                if(Input.GetKey(KeyCode.RightArrow))
                {
                    motor.Rotate(-data.rotateSpeed);
                }
                if(Input.GetKey(KeyCode.LeftArrow))
                {
                    motor.Rotate(data.rotateSpeed);
                }
                if(Input.GetKeyDown(KeyCode.Return))
                {
                    cannon.FireMissile(data.owner);
                }
                break;
        }
    }
}
