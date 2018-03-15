using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControl : MonoBehaviour {

    public static ShipControl sharedInstance;
    public float maxSpeed = 5.0f;
    public float startSpeed=3.0f;
    public float speedIncrement = 0.1f;
    public float accelerationInterval = 0.2f;
    //remove in final version 
    public bool decelerate = true;
    float currentSpeed = 0.0f;
    float accelerationTime = 0.0f;
    //get input from the move joystick, aka the direction the joystick is pointing towards
    public VirtualJoystick moveJoystick;
    //the rigidbody2d component of the ship
    private Rigidbody2D controller;
    //firerate interval, less means faster
    public float shotInterval = 0.5f;
    float nextShot = 0.0f;
    //trigger shooting from the UI shoot button
    bool allowShot = false;
    TurretSystem attachTurretSystem;

    // Use this for initialization
    void Start () {
        //lastPosition = Vector3.zero;
        //speed = 0.0f;
        //get the ship's rigidbody2d component, setting it in the controller variable
        attachTurretSystem = this.GetComponent<TurretSystem>();
        controller = GetComponent<Rigidbody2D>();
        sharedInstance = this;
        currentSpeed = startSpeed;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Debug Shoot code, can't test on pc using the thumbstick and buttons so we need keyboard controls
        if (Input.GetKey(KeyCode.Space) && Time.time > nextShot)
        {
            nextShot = Time.time + shotInterval;
            attachTurretSystem.ShootingRoutine();
        }
        if (allowShot && Time.time > nextShot)
        {
            nextShot = Time.time + shotInterval;
            attachTurretSystem.ShootingRoutine();
        }
    }

    //movement should be tied to framerate, called at exact points in the games FPS
    private void FixedUpdate()
    {
        if (moveJoystick.inputDirection != Vector3.zero)
        {
            //get the joystick direction, and move in that direction, Time.deltaTime used to tie the calculation to framerate
            Vector3 movement = moveJoystick.inputDirection.normalized * currentSpeed * Time.deltaTime;
            //check if the future position is within bounds
            controller.MovePosition(controller.transform.position + movement);
            if(Time.time>accelerationTime&& currentSpeed<maxSpeed)
            {
                accelerationTime = Time.time + accelerationInterval;
                currentSpeed = currentSpeed + speedIncrement;
                if (currentSpeed > maxSpeed)
                    currentSpeed = maxSpeed;
                //Debug.Log("Accelerated speed: " + currentSpeed);
            }
        }
        else
        {
            if (decelerate)
            {
                    if (Time.time > accelerationTime && currentSpeed > startSpeed)
                    {
                        accelerationTime = Time.time + accelerationInterval;
                        currentSpeed = currentSpeed - speedIncrement;
                        if (currentSpeed <startSpeed)
                            currentSpeed = startSpeed;
                        Debug.Log("Deceleration speed: " + currentSpeed);
                    }
            }
            else
            {
                currentSpeed = startSpeed;
            }
        }
    }

    public void Shoot()
    {

    }

    public void AllowShooting()
    {
        allowShot = true;
    }

    public void DisableShooting()
    {
        allowShot = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="EnemyBullet")
        {
            collision.gameObject.SetActive(false);
        }
    }
}
