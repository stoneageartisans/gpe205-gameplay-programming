using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCannon : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FireMissile()
    {
        // Spawns a missile that moves in the direction the tank is pointing
        Instantiate(GameManager.instance.missile, gameObject.transform.position, gameObject.transform.rotation);
    }
}
