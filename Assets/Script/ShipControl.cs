using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControl : MonoBehaviour {

    public static ShipControl sharedInstance;
    public float maxSpeed = 5.0f;
    public float startSpeed=3.0f;
    public float speedIncrement = 0.1f;
    public float accelerationInterval = 0.2f;
    //x is width, y is height
    Vector2 shipSizes;
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
    public int shipLives=3;
    Animate animComponent;
    public float shipImmunity = 1.0f;
    public Transform originPos;
    public Transform entryPos;

    // Use this for initialization
    void Awake () {
        //lastPosition = Vector3.zero;
        //speed = 0.0f;
        //get the ship's rigidbody2d component, setting it in the controller variable
        attachTurretSystem = this.GetComponent<TurretSystem>();
        controller = GetComponent<Rigidbody2D>();
        shipSizes = new Vector2(this.GetComponent<BoxCollider2D>().size.x, this.GetComponent<BoxCollider2D>().size.y);
        sharedInstance = this;
        currentSpeed = startSpeed;
        animComponent = this.GetComponent<Animate>();
        animComponent.moveJoystick = moveJoystick;
        animComponent.go = this.gameObject;
        this.transform.GetChild(0).GetComponent<Animate>().moveJoystick = moveJoystick;
        this.transform.GetChild(0).GetComponent<Animate>().go=this.transform.GetChild(0).gameObject;

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
        if (!animComponent.IsAnimating())
        {
            if (moveJoystick.inputDirection != Vector3.zero)
            {
                //get the joystick direction, and move in that direction, Time.deltaTime used to tie the calculation to framerate
                Vector3 movement = moveJoystick.inputDirection.normalized * currentSpeed * Time.deltaTime;
                //check if the future position is within bounds
                if (CameraControl.sharedInstance.IsObjectInView(controller.transform.position + movement, shipSizes.x, shipSizes.y))
                {
                    controller.MovePosition(controller.transform.position + movement);
                    if (Time.time > accelerationTime && currentSpeed < maxSpeed)
                    {
                        accelerationTime = Time.time + accelerationInterval;
                        currentSpeed = currentSpeed + speedIncrement;
                        if (currentSpeed > maxSpeed)
                            currentSpeed = maxSpeed;
                    }
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
                        if (currentSpeed < startSpeed)
                            currentSpeed = startSpeed;
                    }
                }
                else
                {
                    currentSpeed = startSpeed;
                }
            }
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

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (!animComponent.IsAnimating())
        {
            if(collision.tag=="EnemyBullet" || collision.tag=="Enemy")
            {
                shipLives -= 1;
                if(shipLives>0)
                {
                    Respawn();
                }
                else
                {
                    this.gameObject.SetActive(false);
                    GameArea.sharedInstance.GameOver();
                }
            }
        }
    }

    IEnumerator StopEnemyCollision()
    {
        this.GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(animComponent.SpriteFlicker(shipImmunity));
        yield return new WaitForSeconds(shipImmunity);
        this.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void EnterScene()
    {
        this.GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(animComponent.HeadToPos(entryPos.transform.position, 0.5f));
        this.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void ExitScene()
    {
        Vector3 exitPos = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight + Camera.main.pixelHeight/2));
        exitPos = new Vector3(this.transform.position.x, exitPos.y, 0.0f);
        this.GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(animComponent.HeadToPos(exitPos, 0.5f));
        this.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void Respawn()
    {
        this.transform.position = originPos.transform.position;
        StartCoroutine(StopEnemyCollision());
        StartCoroutine(animComponent.HeadToPos(entryPos.transform.position, 0.5f));
    }

    public void SetOrigin(Transform go)
    {
        originPos = go;
    }

    public void SetEntry(Transform go)
    {
        entryPos = go;
    }
}
