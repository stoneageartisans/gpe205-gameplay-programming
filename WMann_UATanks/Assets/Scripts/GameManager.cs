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
    public float playerMoveSpeed = 3;
    public float enemyMoveSpeed = 2.5f;
    public float tankCannonDelay = 2;
    public int tankStartingHealth = 10;
    public float tankTurnSpeed = 180;

    public GameObject aiTankPrefab;
    public GameObject missilePrefab;
    public GameObject playerTankPrefab;

    public Transform[] waypoints;

    [HideInInspector]
    public GameObject playerTank;

    [HideInInspector]
    public Transform playerTransform;

    private Vector3 cameraOffset;
    private Transform cameraTransform;

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
        // Create the player's tank
        playerTank = Instantiate(playerTankPrefab);
        playerTank.GetComponent<TankData>().ownerName = "Player";
        playerTransform = playerTank.GetComponent<Transform>();

        // Initialize some values for the camera to follow the player
        cameraTransform = Camera.main.GetComponent<Transform>();
        cameraOffset = cameraTransform.position - playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Set the position of the camera to be the same as the player's plus offset
        cameraTransform.position = playerTransform.position + cameraOffset;
    }

    /*
    void CreateEnemy(string name, float x, float z, AiController.PatrolType _patrolType, bool _continuousPatrolling)
    {
        GameObject aiTank = Instantiate(aiTankPrefab);
        aiTank.transform.position = new Vector3(x, aiTank.transform.position.y, z);
        aiTank.GetComponent<TankData>().ownerName = name;

        AiController aiController = aiTank.GetComponent<AiController>();
        aiController.patrolType = _patrolType;
        aiController.continuousPatrolling = _continuousPatrolling;
        aiController.attackMode = AiController.AttackMode.None;
    }

    void CreateEnemy(string name, float x, float z, AiController.AttackMode _attackMode)
    {
        GameObject aiTank = Instantiate(aiTankPrefab);
        aiTank.transform.position = new Vector3(x, aiTank.transform.position.y, z);
        aiTank.GetComponent<TankData>().ownerName = name;

        AiController aiController = aiTank.GetComponent<AiController>();
        aiController.patrolType = AiController.PatrolType.None;
        aiController.continuousPatrolling = false;
        aiController.attackMode = _attackMode;
    }
    */
}
