using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class that inherits from Enemy, parent, class
//basic enemy ship
public class EnemyStraight : Enemy
{
    Enemy thisShip;
    //a turret system component attached to the enemy ship
    //in order to handle firing
    TurretSystem shipTurrets;

    private void Start()
    {
        //get the parent class component
        thisShip = GetComponent<Enemy>();
        //get the rigidbody2d component
        thisShip.controller = this.GetComponent<Rigidbody2D>();
        //get the turretsystem component
        shipTurrets = this.GetComponent<TurretSystem>();
    }

    private void Update()
    {
        //follow the movement behaviour based on path length and looping
        ActivateBehaviour();
        //enemies with turret system should only be allowed to shot when the enemy is in the same screen area
        //as the play, a.k.a. the player area
        if (IsEnemyInView() || shipTurrets.test==true)
        {
            //get the current speed of the enemy as it is used by the turret system to fire bullets
            shipTurrets.GetShipSpeed(thisShip.GetCurrentSpeed());
            //start the shooting routine, that follows rules set in the editor
            shipTurrets.ShootingRoutine();
        }
    }
}
