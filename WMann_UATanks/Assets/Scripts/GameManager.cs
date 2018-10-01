using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public float aiMoveSpeed = 2.5f;
    public int aiStartingHealth = 10;
    public float aiRotateSpeed = 120;
    public float playerMoveSpeed = 3;
    public int playerStartingHealth = 15;
    public float playerRotateSpeed = 180;
    public float rateOfFire = 3;

    public Sprite[] aiSprites;
    public GameObject aiTankPrefab;
    public GameObject playerTank;
    public Transform[] waypoints;    

    [HideInInspector]
    public Transform playerTransform;

    Vector3 cameraOffset;
    Transform cameraTransform;

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
        playerTransform = playerTank.GetComponent<Transform>();
        cameraTransform = Camera.main.GetComponent<Transform>();
        cameraTransform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, cameraTransform.position.z);
        cameraOffset = cameraTransform.position - playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Make the camera follow the player
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

    void CreatePlayer(string playerName)
    {
        playerTank = Instantiate(playerTankPrefab);        

        TankData playerData = playerTank.GetComponent<TankData>();

        playerData.health = playerStartingHealth;
        playerData.moveSpeed = playerMoveSpeed;
        playerData.owner = playerName;
        playerData.rateOfFire = rateOfFire;
        playerData.rotateSpeed = playerRotateSpeed;
    }
    */
}
