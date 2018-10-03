using UnityEngine;

public class Missile : MonoBehaviour
{
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
        // Destroy the missile
        Destroy(gameObject);

        // When the missile hits a tank
        if(collision.collider.name.ToLower().Contains("tank"))
        {
            // Report the missile's owner
            Debug.Log(owner + "'s missile hit " + collision.collider.GetComponent<TankData>().owner);

            // Do damage to hit tank
            DamageTank(collision.collider);
        }
    }

    void DamageTank(Collider2D collider)
    {
        // Reduce tank health by random damage between range
        collider.GetComponent<TankData>().health -= Random.Range(minDamage, maxDamage);

        // Report damage
        Debug.Log(collider.GetComponent<TankData>().owner + "'s health is now " + collider.GetComponent<TankData>().health);
    }
}
