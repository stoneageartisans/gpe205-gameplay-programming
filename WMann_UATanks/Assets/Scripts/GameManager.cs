using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float aiAccuracyInDegrees = 15;
    public float aiFiringRange = 15;
    public float aiMoveSpeed = 2.5f;
    public int aiStartingHealth = 10;
    public float aiRotateSpeed = 120;
    public int enemiesPerRound = 4;
    public float playerMoveSpeed = 3;
    public int playerStartingHealth = 15;
    public float playerRotateSpeed = 180;
    public float rateOfFire = 3;

    public Sprite[] aiSprites;
    
    public GameObject aiTankPrefab;
    public GameObject playerTankPrefab;
    public Transform[] waypoints;

    [HideInInspector]
    public readonly string PlayerName = "Player 1";

    [HideInInspector]
    public Transform playerTransform;

    List<Vector3> aiSpawnPoints;
    Vector3 cameraOffset;
    Transform cameraTransform;
    int enemyCount;
    int playerScore;
    List<Vector3> playerSpawnPoints;
    GameObject playerTank;
    bool respawnPlayer;

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
        enemyCount = 0;
        playerScore = 0;
        respawnPlayer = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(respawnPlayer)
        {
            ClearEnemyTargets();
            CreatePlayer(PlayerName);
            InitializeCamera();
        }

        // Make the camera follow the player
        cameraTransform.position = playerTransform.position + cameraOffset;   
    }

    void ClearEnemyTargets()
    {
        foreach(AiController aiTank in GameObject.FindObjectsOfType<AiController>())
        {
            aiTank.ClearTarget();
        }
    }

    void CreateEnemies(int number)
    {
        for(int i = 0; i < number; i ++)
        {
            CreateEnemy();
        }
    }

    void CreateEnemy()
    {
        GameObject aiTank = Instantiate(aiTankPrefab);
        aiTank.GetComponent<TankData>().owner = ("Enemy Tank " + (enemyCount + 1));        

        AiController aiController = aiTank.GetComponent<AiController>();
        aiController.personality = (AiController.Personality) enemyCount;
        aiController.patrolType = (AiController.PatrolType) Random.Range(0, System.Enum.GetNames(typeof(AiController.PatrolType)).Length);
        
        switch(Random.Range(0, 2))
        {
            case 0:
                aiController.continuousPatrolling = false;
                break;
            case 1:
                aiController.continuousPatrolling = true;
                break;
        }

        int i = Random.Range(0, aiSpawnPoints.Count);
        aiTank.transform.position = aiSpawnPoints[i];
        aiSpawnPoints.RemoveAt(i);

        enemyCount++;
    }

    void CreatePlayer(string playerName)
    {
        playerTank = Instantiate(playerTankPrefab);
        playerTank.GetComponent<TankData>().owner = playerName;

        int i = Random.Range(0, playerSpawnPoints.Count);
        playerTank.transform.position = playerSpawnPoints[i];

        if(respawnPlayer)
        {
            respawnPlayer = false;
        }
    }

    public void DamageTank(Collider2D collider, int damage)
    {
        TankData tankData = collider.GetComponent<TankData>();

        // Reduce tank health by random damage between range
        tankData.health -= damage;

        // Check for tank destruction
        if(tankData.health < 1)
        {
            if(tankData.owner.ToLower().Contains("player"))
            {
                respawnPlayer = true;
                playerScore--;
            }
            else
            {
                enemyCount--;
                playerScore++;
            }

            // Destroy the tank
            Destroy(collider.gameObject);

            if(enemyCount < 1)
            {
                GetAiSpawnPoints();
            }
        }

        // Report damage
        Debug.Log(collider.GetComponent<TankData>().owner + "'s health is now " + collider.GetComponent<TankData>().health);
    }

    public void GetAiSpawnPoints()
    {
        aiSpawnPoints = MapGenerator.instance.GetAiSpawnPoints();
        CreateEnemies(enemiesPerRound);
    }

    public void GetPlayerSpawnPoints()
    {
        playerSpawnPoints = MapGenerator.instance.GetPlayerSpawnPoints();
        CreatePlayer(PlayerName);
        InitializeCamera();
    }

    void InitializeCamera()
    {
        playerTransform = playerTank.GetComponent<Transform>();
        cameraTransform = Camera.main.GetComponent<Transform>();
        cameraTransform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, cameraTransform.position.z);
        cameraOffset = cameraTransform.position - playerTransform.position;
    }
}
