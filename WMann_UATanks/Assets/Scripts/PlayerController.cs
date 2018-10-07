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
        data = gameObject.GetComponent<TankData>();
        data.health = GameManager.instance.playerStartingHealth;
        data.moveSpeed = GameManager.instance.playerMoveSpeed;
        data.rateOfFire = GameManager.instance.rateOfFire;
        data.rotateSpeed = GameManager.instance.playerRotateSpeed;

        cannon = gameObject.GetComponentInChildren<TankCannon>();
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
                    motor.PlayMoveSound();
                }
                if(Input.GetKey(KeyCode.S))
                {
                    motor.Move(-data.moveSpeed);
                    motor.PlayMoveSound();
                }
                if(Input.GetKey(KeyCode.D))
                {
                    motor.Rotate(-data.rotateSpeed);
                    motor.PlayRotateSound();
                }
                if(Input.GetKey(KeyCode.A))
                {
                    motor.Rotate(data.rotateSpeed);
                    motor.PlayRotateSound();
                }
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    cannon.FireMissile(data.owner);
                }
                if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
                {
                    motor.PauseMoveSound();
                }
                if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
                {
                    motor.PauseRotateSound();
                }
                break;
            case InputScheme.arrowKeys:
                if(Input.GetKey(KeyCode.UpArrow))
                {
                    motor.Move(data.moveSpeed);
                    motor.PlayMoveSound();
                }
                if(Input.GetKey(KeyCode.DownArrow))
                {
                    motor.Move(-data.moveSpeed);
                    motor.PlayMoveSound();
                }
                if(Input.GetKey(KeyCode.RightArrow))
                {
                    motor.PlayRotateSound();
                    motor.Rotate(-data.rotateSpeed);
                }
                if(Input.GetKey(KeyCode.LeftArrow))
                {
                    motor.Rotate(data.rotateSpeed);
                    motor.PlayRotateSound();
                }
                if(Input.GetKeyDown(KeyCode.Return))
                {
                    cannon.FireMissile(data.owner);
                }
                if(Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
                {
                    motor.PauseMoveSound();
                }
                if(Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    motor.PauseRotateSound();
                }
                break;
        }
    }
}
