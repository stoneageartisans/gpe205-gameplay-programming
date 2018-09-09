using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float missileLife = 3; // in seconds
    public float missileSpeed = 100; // in meters per second
    public float tankMoveSpeed = 3; // in meters per second
    public float tankRateOfFire = 0.25F; // in shots per second
    public float tankTurnSpeed = 180; // in degrees per second

    public GameObject missile;
    public GameObject playerTank;    

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("ERROR: An instance of " + gameObject.name + " already exists.");
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        // Spawns the player's tank
        Instantiate(playerTank);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
