using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [HideInInspector]
    public string ownerName;

    // Use this for initialization
    void Start()
    {
        // Destroy the missile after x seconds
        Destroy(gameObject, GameManager.instance.missileLife);

        // Apply force to the missile
        GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * GameManager.instance.missileSpeed);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        // Destroy the missile
        Destroy(gameObject);

        // When the missile hits a tank
        if(collider.name.ToLower().Contains("tank"))
        {
            // Report the missile's owner
            Debug.Log(ownerName + "'s missile hit " + collider.name);

            // Do damage to hit tank
            DamageTank(collider);
        }
    }

    void DamageTank(Collider collider)
    {
        // Reduce tank health by random damage between range
        collider.GetComponent<TankData>().health -= Random.Range(GameManager.instance.missileMinDamage, GameManager.instance.missileMaxDamage);

        // Report damage
        Debug.Log(collider.name + "'s health is now " + collider.GetComponent<TankData>().health);
    }
}
