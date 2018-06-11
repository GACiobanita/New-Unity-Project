using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the enemy spikes entity
//inherits from the parent class
public class EnemySpikes : Enemy
{
    //has a new speed variable when chasing the player
    public float chaseSpeed=0.5f;
    //detection radius
    public float detectionRadius;
    //reference to the Enemy component
    Enemy thisShip;
    //if the player ship was detected
    bool targetAcquired = false;
    //direction from the current enemy spikes entity towards the player ship
    Vector3 dir;
    //target position of the player ship
    Vector3 targetPosition;

    private void Start()
    {
        //setup the class component
        thisShip = GetComponent<Enemy>();
        //and the rigidbody component of the Enemy class
        thisShip.controller = this.GetComponent<Rigidbody2D>();
        //no initial target position
        targetPosition = Vector3.zero;
        dir = Vector3.zero;
    }

    //follow the enemy behaviour routine depending on what pathing has been set
    private void Update()
    {
        ActivateBehaviour();
        if(!targetAcquired)
            FindTarget();
    }

    private void OnBecameInvisible()
    {
        Reset();
        targetAcquired = false;
        targetPosition = Vector3.zero;
        dir = Vector3.zero;
    }

    //override the NormalRoutine method of the Enemy parent class
    public override void NormalRoutine(float speed)
    {
        //the enemy spikes entity will use the chaseSpeed once the player ship entity has been detected
        if (targetAcquired)
        {
            //move in the direction of the player ship 
            thisShip.controller.MovePosition(this.transform.position + dir * chaseSpeed *Time.deltaTime);
        }
        else
        {
            //else if the player ship hasn't been detected move towards the bottom of the screen
            //using the parent class Enemy method for NormalRoutine
            base.NormalRoutine(thisShip.GetCurrentSpeed());
        }
    }

    void FindTarget()
    {
        //1<<layer.nametolayer bitwise shift 
        //https://stackoverflow.com/questions/25435467/overlapcircleall-is-not-picking-up-other-objects
        //NameToLayer returns a layer index (ie: 7), but OverlapCircleAll expects a bitwise layer mask (ie: 7th bit enabled).
        Collider2D hit = Physics2D.OverlapCircle(this.transform.position, detectionRadius, 1 << LayerMask.NameToLayer("Player"));
        if(hit)
        {
            //get the pos of the player ship
            targetPosition = hit.transform.position;
            //detetermine direction
            dir = targetPosition - this.transform.position;
            //stop searching for the player ship
            targetAcquired = true;
            //stop the pathing routine, if any
            hasPath = false;
        }
    }
}
