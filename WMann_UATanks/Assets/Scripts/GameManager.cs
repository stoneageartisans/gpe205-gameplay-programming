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
    public int enemyCount = 4;
    public float playerMoveSpeed = 3;
    public int playerStartingHealth = 15;
    public float playerRotateSpeed = 180;
    public float rateOfFire = 3;

    public Sprite[] aiSprites;
    
    public GameObject aiTankPrefab;
    public GameObject playerTankPrefab;
    public Transform[] waypoints;    

    [HideInInspector]
    public Transform playerTransform;

    List<Vector3> aiSpawnPoints;
    List<GameObject> aiTankList;
    Vector3 cameraOffset;
    Transform cameraTransform;
    List<Vector3> playerSpawnPoints;
    GameObject playerTank;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        // Make the camera follow the player
        cameraTransform.position = playerTransform.position + cameraOffset;
    }

    void CreateEnemies(int enemyCount)
    {
        aiTankList = new List<GameObject>();

        for(int i = 0; i < enemyCount; i ++)
        {
            CreateEnemy();
        }
    }

    void CreateEnemy()
    {
        GameObject aiTank = Instantiate(aiTankPrefab);
        aiTank.GetComponent<TankData>().owner = ("Enemy Tank " + (aiTankList.Count + 1));        

        AiController aiController = aiTank.GetComponent<AiController>();
        aiController.personality = (AiController.Personality) aiTankList.Count;
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

        aiTankList.Add(aiTank);
    }

    void CreatePlayer(string playerName)
    {
        playerTank = Instantiate(playerTankPrefab);
        playerTank.GetComponent<TankData>().owner = playerName;

        int i = Random.Range(0, playerSpawnPoints.Count);
        playerTank.transform.position = playerSpawnPoints[i];
    }

    public void GetAiSpawnPoints()
    {
        aiSpawnPoints = MapGenerator.instance.GetAiSpawnPoints();
        CreateEnemies(enemyCount);
    }

    public void GetPlayerSpawnPoints()
    {
        playerSpawnPoints = MapGenerator.instance.GetPlayerSpawnPoints();
        CreatePlayer("Player 1");
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
