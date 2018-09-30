using UnityEngine;

public class TankCannon : MonoBehaviour
{
    public GameObject missilePrefab;

    bool ready;
    float nextReadyTime;    
    TankData data;
    Transform _transform;

    // Use this for initialization
    void Start()
    {
        ready = true;
        data = gameObject.GetComponentInParent<TankData>();
        _transform = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!ready)
        {
            // Check timer
            if(Time.time >= nextReadyTime)
            {
                // Set cannon as "unready"
                ready = true;
            }
        }
    }

    public void FireMissile(string owner)
    {
        if(ready)
        {
            // Spawns a missile that moves in the direction the tank is pointing
            GameObject missile = Instantiate(missilePrefab, _transform.position, _transform.rotation);

            // Set the missile's owner
            missile.GetComponent<Missile>().owner = data.owner;

            // Set cannon as "unready"
            ready = false;

            // Set the next "ready" time
            nextReadyTime = Time.time + data.rateOfFire;
        }
    }
}
