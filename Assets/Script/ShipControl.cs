using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControl : MonoBehaviour {

    public static ShipControl sharedInstance;
    public float moveSpeed=5.0f;
    //real drag?
    public float drag=0.5f;
    //get input from the move joystick, aka the direction the joystick is pointing towards
    public VirtualJoystick moveJoystick;
    //the "gatling gun" from which the bullets are launched from
    public GameObject bulletSpawnPoint;
    //the rigidbody2d component of the ship
    private Rigidbody2D controller;
    //firerate interval, less means faster
    public float shotInterval = 0.5f;
    float nextShot = 0.0f;
    //trigger shooting from the UI shoot button
    bool allowShot = false;

    // Use this for initialization
    void Start () {
        //lastPosition = Vector3.zero;
        //speed = 0.0f;
        //get the ship's rigidbody2d component, setting it in the controller variable
        controller = GetComponent<Rigidbody2D>();
        sharedInstance = this;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Debug.Log(allowShot);
        //Debug Shoot code, can't test on pc using the thumbstick and buttons so we need keyboard controls
        if(Input.GetKey(KeyCode.Space)&& allowShot && Time.time > nextShot)
        {
            nextShot = Time.time + shotInterval;
            Shoot();
        }
        if(allowShot && Time.time>nextShot)
        {
            nextShot = Time.time + shotInterval;
            Shoot();
        }
    }

    //movement should be tied to framerate, called at exact points in the games FPS
    private void FixedUpdate()
    {
        if (moveJoystick.inputDirection != Vector3.zero)
        {
            //get the joystick direction, and move in that direction, Time.deltaTime used to tie the calculation to framerate
            Vector3 movement = moveJoystick.inputDirection.normalized * moveSpeed * Time.deltaTime;
            //check if the future position is within bounds
            controller.MovePosition(controller.transform.position + movement);
        }
    }

    public void AllowShooting()
    {
        allowShot = true;
    }

    public void DisableShooting()
    {
        allowShot = false;
    }

    public void Shoot()
    {
        //get a bullet from the pool, search by tag set in the unity inspector, on the prefab
        GameObject bullet = ObjectPooler.sharedInstance.GetPooledObject("PlayerBullet");
        //just to be sure we aren't firing blanks
        if (bullet != null)
        {
            //set the bullet's start position at the location of the bullet spawn point
            //bullet spawn point is a child object used only for the purpose of spawning bullets, think of it as a gatling gun
            bullet.transform.position = bulletSpawnPoint.transform.position;
            //will go in the direction the gatling gun is pointing towards
            bullet.transform.rotation = bulletSpawnPoint.transform.rotation;
            bullet.SetActive(true);
        }
    }
}
