using UnityEngine;

public class TankData : MonoBehaviour
{
    [HideInInspector]
    public int health;

    [HideInInspector]
    public float moveSpeed;

    [HideInInspector]
    public string owner;

    [HideInInspector]
    public float rateOfFire;

    [HideInInspector]
    public float rotateSpeed;

    [HideInInspector]
    public Transform _transform;

    // Use this for initialization
    void Start()
    {
        _transform = gameObject.GetComponent<Transform>();
        owner = gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        // TODO
    }
}
