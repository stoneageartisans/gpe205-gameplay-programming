using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankData : MonoBehaviour
{
    [HideInInspector]
    public string ownerName;

    [HideInInspector]
    public int health;

    // Use this for initialization
    void Start()
    {
        health = GameManager.instance.tankStartingHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
