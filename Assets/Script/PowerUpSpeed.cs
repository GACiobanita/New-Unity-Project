using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpeed : PowerUp {

    public float additionalSpeed;

    private void Awake()
    {
        ComponentSetup();
    }

    private void Start()
    {
        StartCoroutine(Countdown());
    }

    void FixedUpdate()
    {
        if (canMove)
            Movement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Player")
        {
            collision.GetComponent<ShipControl>().IncreaseMaxSpeed(additionalSpeed);
            RemovePowerUp();
        }
    }
}
