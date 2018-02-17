using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControl : MonoBehaviour {

    public float moveSpeed=5.0f;
    //real drag?
    public float drag=0.5f;
    //get input from the move joystick, aka the direction the joystick is pointing towards
    public VirtualJoystick moveJoystick;
    //the "gatling gun" from which the bullets are launched from
    public GameObject bulletSpawnPoint;
    //the rigidbody2d component of the ship
    private Rigidbody2D controller;
    //ship sprite size
    float shipWidth;
    //Vector3 lastPosition;//Debug, remove later
    //float speed;//Debug, remove later


    // Use this for initialization
    void Start () {
        //lastPosition = Vector3.zero;
        //speed = 0.0f;
        //get the ship's rigidbody2d component, setting it in the controller variable
        controller = GetComponent<Rigidbody2D>();
        //get the width of the ship size divided by 100(the sprite pixel per unit), we can do this better but how
        shipWidth = GetComponent<SpriteRenderer>().bounds.size.x;
	}
	
	// Update is called once per frame
	void Update () {
        //if there is joystick input
        if (moveJoystick.inputDirection!=Vector3.zero)
        {
            //get the joystick direction, and move in that direction, Time.deltaTime used to tie the calculation to framerate
            Vector3 movement = moveJoystick.inputDirection.normalized * moveSpeed * Time.deltaTime;
            //check if the future position is within bounds
            controller.MovePosition(GameArea.sharedInstance.CheckPosition(controller.transform.position + movement, moveJoystick.inputDirection.normalized, shipWidth));
        }
        //Debug Shoot code, can't test on pc using the thumbstick and buttons so we need keyboard controls
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
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
