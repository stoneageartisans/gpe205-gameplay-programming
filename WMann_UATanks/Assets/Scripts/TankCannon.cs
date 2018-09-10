﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankCannon : MonoBehaviour
{
    private bool ready;
    private float nextReadyTime;

    // Use this for initialization
    void Start()
    {
        ready = true;
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

    public void FireMissile(string ownerName)
    {
        if(ready)
        {
            // Spawns a missile that moves in the direction the tank is pointing
            GameObject missile = Instantiate(GameManager.instance.missilePrefab, gameObject.transform.position, gameObject.transform.rotation);

            // Set the missile's owner
            missile.GetComponent<Missile>().ownerName = ownerName;

            // Set cannon as "unready"
            ready = false;

            // Set the next "ready" time
            nextReadyTime = Time.time + GameManager.instance.tankCannonDelay;
        }
    }
}
