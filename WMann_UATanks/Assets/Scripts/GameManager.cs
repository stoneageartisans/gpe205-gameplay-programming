using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float missileLife = 3;
    public int missileMinDamage = 3;
    public int missileMaxDamage = 7;
    public float missileSpeed = 10;
    public float tankMoveSpeed = 3;
    public float tankCannonDelay = 2;
    public int tankStartingHealth = 10;
    public float tankTurnSpeed = 180;

    public GameObject aiTankPrefab;
    public GameObject missilePrefab;
    public GameObject playerTankPrefab;    

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
        GameObject playerTank = Instantiate(playerTankPrefab);
        playerTank.GetComponent<TankData>().ownerName = "Player";

        // Spawn an AI tank
        GameObject aiTank = Instantiate(aiTankPrefab);
        aiTank.transform.position = new Vector3(10, aiTank.transform.position.y, 10);

        // Spawn an AI tank
        GameObject aiTank2 = Instantiate(aiTankPrefab);
        aiTank2.transform.position = new Vector3(-5, aiTank2.transform.position.y, -10);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
