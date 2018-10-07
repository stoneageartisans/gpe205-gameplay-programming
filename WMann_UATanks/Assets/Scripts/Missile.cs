using UnityEngine;

public class Missile : MonoBehaviour
{
    public GameObject explosionPrefab;
    public Transform warhead;

    public float lifetime = 3;
    public int maxDamage = 7;
    public int minDamage = 3;
    public float speed = 10;

    [HideInInspector]
    public string owner;

    // Use this for initialization
    void Start()
    {
        // Destroy the missile after x seconds
        Destroy(gameObject, lifetime);

        // Apply force to the missile
        GetComponent<Rigidbody2D>().AddForce(gameObject.transform.right * speed);
    }

    // Update is called once per frame
    void Update()
    {
        // TODO
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Create explosion
        GameObject explosion = Instantiate(explosionPrefab);
        explosion.transform.position = warhead.position;
        Destroy(explosion, 0.5f);

        // Destroy the missile
        Destroy(gameObject);

        // When the missile hits a tank
        if(collision.collider.name.ToLower().Contains("tank"))
        {
            // Report the missile's owner
            Debug.Log(owner + "'s missile hit " + collision.collider.GetComponent<TankData>().owner);

            // If the player hit an ai tank...
            if(owner.ToLower().Contains("player"))
            {
                // ...then the ai tank detects the player
                collision.collider.gameObject.GetComponent<AiController>().TargetWasDetected();
            }

            // Do damage to hit tank
            GameManager.instance.DamageTank(collision.collider, Random.Range(minDamage, maxDamage));
        }
    }
}
