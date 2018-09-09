using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        // Unspawn the missile after 3 seconds
        Destroy(gameObject, GameManager.instance.missileLife);

        // Apply force to the missile
        GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * GameManager.instance.missileSpeed);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
