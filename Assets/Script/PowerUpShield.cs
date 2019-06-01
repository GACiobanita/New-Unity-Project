using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpShield : PowerUp {

    public GameObject shieldObj;

    private void Awake()
    {
        ComponentSetup();
    }

    private void Start()
    {
        StartCoroutine(Countdown());
    }

    private void FixedUpdate()
    {
        if (canMove)
            Movement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            shieldObj.GetComponent<Shield>().SetArmor(ShipControl.sharedInstance.GetShieldArmor());
            GameObject go=Instantiate(shieldObj) as GameObject;
            go.transform.position = collision.transform.position;
            go.transform.parent = collision.transform;
            RemovePowerUp();
        }
    }
}
