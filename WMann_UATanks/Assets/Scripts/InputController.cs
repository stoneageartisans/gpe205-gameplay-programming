using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public enum InputScheme
    {
        WASD,
        arrowKeys
    };

    public InputScheme inputScheme = InputScheme.WASD;

    private TankCannon tankCannon;
    private TankData tankData;
    private TankMotor tankMotor;    

    // Use this for initialization
    void Start()
    {
        tankCannon = gameObject.GetComponent<TankCannon>();
        tankData = gameObject.GetComponent<TankData>();
        tankMotor = gameObject.GetComponent<TankMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        switch(inputScheme)
        {
            case InputScheme.WASD:
                if(Input.GetKey(KeyCode.W))
                {
                    tankMotor.Move(tankData.moveSpeed);
                }
                if(Input.GetKey(KeyCode.S))
                {
                    tankMotor.Move(-tankData.moveSpeed);
                }
                if(Input.GetKey(KeyCode.D))
                {
                    tankMotor.Rotate(tankData.turnSpeed);
                }
                if(Input.GetKey(KeyCode.A))
                {
                    tankMotor.Rotate(-tankData.turnSpeed);
                }
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    tankCannon.FireMissile();
                }
                break;
            case InputScheme.arrowKeys:
                if(Input.GetKey(KeyCode.UpArrow))
                {
                    tankMotor.Move(tankData.moveSpeed);
                }
                if(Input.GetKey(KeyCode.DownArrow))
                {
                    tankMotor.Move(-tankData.moveSpeed);
                }
                if(Input.GetKey(KeyCode.RightArrow))
                {
                    tankMotor.Rotate(tankData.turnSpeed);
                }
                if(Input.GetKey(KeyCode.LeftArrow))
                {
                    tankMotor.Rotate(-tankData.turnSpeed);
                }
                if(Input.GetKeyDown(KeyCode.Return))
                {
                    tankCannon.FireMissile();
                }
                break;
        }
    }
}
