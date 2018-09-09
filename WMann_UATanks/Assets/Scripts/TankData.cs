using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankData : MonoBehaviour
{
    [HideInInspector]
    public float moveSpeed;

    [HideInInspector]
    public float rateOfFire;

    [HideInInspector]
    public  float turnSpeed;

    // Use this for initialization
    void Start()
    {
        moveSpeed = GameManager.instance.tankMoveSpeed;
        rateOfFire = GameManager.instance.tankRateOfFire;
        turnSpeed = GameManager.instance.tankTurnSpeed;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
