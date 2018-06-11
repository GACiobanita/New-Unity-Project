using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a base(parent) class for all enemies that will be used in a shmup game, maybe other games
[System.Serializable]
public class Enemy: MonoBehaviour {

    //in the shmup game enemies will have 3 different speeds
    //base speed to be used when out of a predetermined path or in basic movement 
    public float baseSpeed;
    //speed used while the enemy travels in a predetermined path
    public float loopSpeed;
    //speed used if the enemy exits with a different speed from a predetermined path
    public float outOfLoopSpeed;
    //this value stores the current speed used by the enemy, mainly needed for a return speed function
    public float currentSpeed;
    //if the enemy has a predetermined path
    public bool hasPath;
    //if the enemy has a path, is that path a loop?
    public bool isLooping;
    //path points
    public List<Vector2> loopPoints;
    //the rigidbody component of the enemy, used for movement methods
    public Rigidbody2D controller;
    //what node in the path is the enemy currently at
    public int currentLoopLocation;

    //default constructor
    public Enemy()
    {
        baseSpeed = 0.0f;
        loopSpeed = 0.0f;
        outOfLoopSpeed = 0.0f;
        currentSpeed = 0.0f;
        hasPath = false;
        isLooping = false;
        loopPoints = new List<Vector2>();
        currentLoopLocation = 0;
    }

    //default constructor for basic enemies with a single speed
    public Enemy(float speed)
    {
        baseSpeed = speed;
        loopSpeed = speed;
        outOfLoopSpeed = speed;
        currentSpeed = speed;
        hasPath = false;
        isLooping = false;
        loopPoints = new List<Vector2>();
        currentLoopLocation = 0;
    }

    //constructor with all 3 speeds
    public Enemy(float bSpeed, float lSpeed, float oolSpeed)
    {
        baseSpeed = bSpeed;
        loopSpeed = lSpeed;
        outOfLoopSpeed = oolSpeed;
        hasPath = false;
        isLooping = false;
        loopPoints = new List<Vector2>();
        currentLoopLocation = 0;
    }

    //set/get the base speed
    public float BaseSpeed
    {
        get
        {
            return baseSpeed;
        }
        set
        {
            baseSpeed = value;
        }
    }

    //set/get the loop speed
    public float LoopSpeed
    {
        get
        {
            return loopSpeed;
        }
        set
        {
            loopSpeed = value;
        }
    }

    //set/get the ool speed
    public float OutOfLoopSpeed
    {
        get
        {
            return outOfLoopSpeed;
        }
        set
        {
            outOfLoopSpeed = value;
        }
    }

    //pass a list of vector2's that the enemy will travels through, also see if the enemy should loop
    public void SetupPathing(List<Vector2> pointsPath,bool looping)
    {
        loopPoints = pointsPath;
        //checks if the list is composed of just one point or multiple points
        if(loopPoints.Count>1)
        {
            hasPath = true;
        }
        else
        {
            //is it has only one point then this means it is just an origin point
            //and the enemy should follow it's basic predetermined path, e.g. just moving downwards
            hasPath = false;
        }
        isLooping = looping;
    }

    //method that handles the enemy movement depending if the enemy has a path and if this path is loopable
    //also handles the speed value the enemy uses depending on enemy movement
    public void ActivateBehaviour()
    {
        if(hasPath)
        {
            //change the current speed variable
            //variable is also used in the movement methods
            currentSpeed = loopSpeed;
            if (isLooping)
            {
                //loop movement
                LoopThroughPoints();
            }
            else
            {
                //doing the path only once
                TravelThroughPoints();
            }
        }
        else
        {
            //once the enemy exits the path or if there is no path
            currentSpeed = baseSpeed;
            NormalRoutine(currentSpeed);
        }
    }

    //normal movement is that the enemy only goes towards the bottom of the screen
    public virtual void NormalRoutine(float speed)
    {
        Vector3 movement = Vector3.down * speed * Time.deltaTime;
        //check if the future position is within bounds
        controller.MovePosition(controller.transform.position + movement);
    }

    public void LoopThroughPoints()
    {
        //get the current pos of the enemy
        Vector2 currPos = this.transform.position;
        //if the current loop location hasn't reached the end of the loop
        //default value is 0 and will start at 0 when moving
        if (currentLoopLocation < loopPoints.Count)
        {
            //if the current pos of the enemy is not the current position of the next loop point
            //use the currentLoopLocation value to determine what point the enemy will go towards
            if (currPos != loopPoints[currentLoopLocation])
            {
                //move the enemy towards the point
                //https://docs.unity3d.com/ScriptReference/Vector2.MoveTowards.html
                Vector2 pos = Vector2.MoveTowards(this.transform.position, loopPoints[currentLoopLocation], currentSpeed * Time.deltaTime);
                controller.MovePosition(pos);
            }
            else
            {
                //else update the current loop location so the method will proceed to the next loop point
                currentLoopLocation += 1;
            }
        }
        else
        {
            //else, reset the current loop location so that the enemy will now go towards the initial point
            //in the loop
            currentLoopLocation = 0;
        }
    }

    public void TravelThroughPoints()
    {
        //get the current pos of the enemy
        Vector2 currPos = this.transform.position;
        //check if the enemy has reached the final loop point
        if (currentLoopLocation < loopPoints.Count)
        {
            //if the current pos of the enemy is not the current pos of the next loop point
            if (currPos != loopPoints[currentLoopLocation])
            {
                //move the enemy towards the point
                Vector2 pos = Vector2.MoveTowards(this.transform.position, loopPoints[currentLoopLocation], currentSpeed * Time.deltaTime);
                controller.MovePosition(pos);
            }
            else
            {
                currentLoopLocation += 1;
            }
        }
        else
        {
            //the determined path has finished and now the enemy will continue it's normal routine
            //also set the currentSpeed value to the oolSpeed value
            currentSpeed = outOfLoopSpeed;
            //start the normal routine
            NormalRoutine(currentSpeed);
        }
    }

    //enemy objects are not destroyed but instead are reset in order to be used later in the level
    //by the object pooler
    private void OnBecameInvisible()
    {
        Reset();
    }

    //method sued to determine the enemy screen location
    public bool IsEnemyInView()
    {
        return CameraControl.sharedInstance.IsObjectInView(this.transform.position, 0, 0);
    }

    //get the current speed value
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    //get a random direction to move towards
    //unused
    public Vector2 GetRandomDirection()
    {
        return Random.insideUnitCircle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Player" || collision.tag=="PlayerBullet")
        {
            this.gameObject.SetActive(false);
        }
    }

    public void Reset()
    {
        Debug.Log("invisible");
        //deactivate the object
        this.gameObject.SetActive(false);
        //and reset it's pathing values
        hasPath = false;
        isLooping = false;
        loopPoints = new List<Vector2>();
        currentLoopLocation = 0;
    }
}
