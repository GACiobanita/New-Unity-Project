using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

//a turret object class that will contain all information about turrets attached to the entity using
//a turret system
//main turret system uses a list of TurretObjects, each with their own valeus
[System.Serializable]
public class TurretObjects
{
    //objects called "turret" with no additional functionality are attached to the entity
    //and then referenced in this class
    public GameObject turretObj;
    //bullet types
    public enum BulletType
    {
        none, EnemyBullet, PlayerBullet
    }
    //used bullet type by the turret
    public BulletType usedBulletType;
    //a set bullet speed, the script handles it so it also adds the entity speed to the calculation
    public float bulletSpeed;
    [SerializeField]
    //if the turret should track the player ship entity
    public bool tracker;
    //if the turret should rotate
    public bool rotator;
    //directions
    public enum rotationDirection
    {
        None = 0, Left = -1, Right = 1
    }
    //what direction should the turret turn towards
    public rotationDirection rotDir;
    //using what rotation speed
    public float rotationSpeed;
    public enum shootingType
    {
        None, Repeater, Burst
    }
    //how the turret will shoot out bullets
    public shootingType turretType;
    //interval at which the shooting sequence will start
    public float shotInterval = 0f;
    //interval at which the burst sequence will start
    public float burstInterval = 0f;
    //interval used to delay between bursts
    public float burstDelay = 0f;
    [HideInInspector]
    //if the next shot is available, for bullet shooting
    public float nextShot = 0.0f;
    [HideInInspector]
    //if the next burst sequence is available
    public float nextBurst = 0.0f;
    //amount of bullets in a burst
    //this is calculated in such a way that bullets will be fired at an interval determined by code
    //in order to fire a certain amount or bullets in a certain amount of time
    //e.g. will fire 5 bullets in 1 second every 0.2
    //or
    //10 bullets in 1 sec every 0.1 
    public float burstSize = 0f;
}

public class TurretSystem : MonoBehaviour {

    //a list of all the turretobjects that each contain their own variables that create a unique turret
    [SerializeField]
    public List<TurretObjects> turretList;
    //separate lists each containing extra functionality for turrets of different types
    List<TurretObjects> trackerList=new List<TurretObjects>();
    List<TurretObjects> rotatorList = new List<TurretObjects>();
    //the target
    public Transform playerShip;
    //the speed of the entity this script is attached to
    public float shipSpeed;
    public bool test=false;

    void Start()
    {
        //playerShip = GameArea.sharedInstance.GetPlayerShipTransform();
        //setup special turret types
        SetupTrackers();
        SetupRotators();
        //setup shooting values for each turret to be safe
        foreach(TurretObjects turret in turretList)
        {
            turret.nextBurst = 0.0f;
            turret.nextShot = 0.0f;
            if(turret.turretType==TurretObjects.shootingType.Burst)
            {
                turret.nextBurst = Time.time + turret.burstInterval + turret.burstDelay;
            }
        }
    }
    
    private void Update()
    {
        //the only method called each frame only if there are rotating turrets
        if(rotatorList.Count>0)
        {
            UpdateRotation();
        }
    }

    //turret targetting for tracking turrets
    void UpdateTurretTarget()
    {
        Vector2 targetPoint;
        float angle;
        Quaternion rotation;
        //the turretObj, inside TurretObjects class, itself is rotated towards the target in order
        //to fire the bullet towards the direction of the target
        foreach (TurretObjects turret in trackerList)
        {
            //calculate the distance between the turret and the player
            targetPoint = playerShip.position - turret.turretObj.transform.position;
            //get the angle in degrees(rad2deg) between the X axis and a 2D vector starting at 0
            //and terminating at (x,y)
            //	^
            //	|
            //	|
            //	|
            //-----0,0------>this axis
            //	| \
            //	|  \
            //	|  x,y and this point
            //	|
            //https://docs.unity3d.com/ScriptReference/Mathf.Atan2.html
            angle = Mathf.Atan2(targetPoint.y, targetPoint.x) * Mathf.Rad2Deg;
            //creates a rotation which rotates angle degrees around axis
            //in this case the angle above and the forward vector, which counts as the forwards vector for all
            //entities in a shmup
            rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            //apply rotation
            turret.turretObj.transform.rotation = rotation;
            //Debug.Log(playerShip.transform.position);
            //Vector2 direction = new Vector2(playerShip.transform.position.x - turret.turretObj.transform.position.x, playerShip.transform.position.y - turret.turretObj.transform.position.y);
            //turret.turretObj.transform.up = direction;
        }
      
    }

    void UpdateRotation()
    {
        foreach(TurretObjects turret in rotatorList)
        {
            //change the Z over time 
            turret.turretObj.transform.Rotate(0, 0, turret.rotationSpeed * Time.deltaTime*(float)turret.rotDir);
        }
    }

    void SetupTrackers()
    {
        //setting up the list of turrets that would track the enemy position when firing bullets
        foreach (TurretObjects turret in turretList)
        {
            if (turret.tracker)
            {
                trackerList.Add(turret);
            }
        }
    }

    void SetupRotators()
    {
        //setup the list of turrets that would rotate
        foreach(TurretObjects turret in turretList)
        {
            if(turret.rotator)
            {
                rotatorList.Add(turret);
            }
        }
    }


    //should be called in the entity that has a turretsystem attached
    public void ShootingRoutine()
    {
        //only update the trackin turrets position/targets when firing
        if (trackerList.Count > 0)
        {
            UpdateTurretTarget();
        }
        //all turrets will fire
        foreach (TurretObjects turret in turretList)
        {
            ShootCaller(turret);
        }
    }

    //called when firing a bullet
    public void GetBullet(TurretObjects turret)
    {
        //get a bullet from the pool, search by tag set in the unity inspector, on the prefab
        GameObject bullet = ObjectPooler.sharedInstance.GetPooledObject(turret.usedBulletType.ToString());
        //just to be sure we aren't firing blanks
        if (bullet != null)
        {
            //set the bullet's start position at the location of the bullet spawn point
            bullet.transform.position = turret.turretObj.transform.position;
            //will go in the direction the gatling gun is pointing towards
            bullet.transform.rotation = turret.turretObj.transform.rotation;
            //get the bullet's rigidbody2d component, setting it in the controller variable>();
            //and set its velocity
            bullet.SetActive(true);
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * (turret.bulletSpeed+shipSpeed);
        }
    }

    //repeater type 
    public void Repeater(TurretObjects turret)
    {
        //when enough time has passed
        if (Time.time > turret.nextShot)
        {
            //the new nextshot is the current time + the time it takes to fire
            turret.nextShot = Time.time + turret.shotInterval;
            //fire a bullet
            GetBullet(turret);
        }
    }

    //burst type
    public void Burst(TurretObjects turret)
    {
        //when enough time has passed
        if (Time.time > turret.nextBurst)
        {
            //the next burst is the current time+ the time it takes to fire a burst + it's delay
            turret.nextBurst = Time.time + turret.burstInterval + turret.burstDelay;
            //a couroutine that fires the burst is started
            StartCoroutine(BurstShots(turret));
        }
    }

    public IEnumerator BurstShots(TurretObjects turret)
    {
        int i = 0;
        //shot interval is determined based on burst interval and the amount of bullets to be fired in said
        //interval
        turret.shotInterval = turret.burstInterval / turret.burstSize;
        //count the bullets
        while (i < turret.burstSize)
        {
            i++;
            //wait for the calculated shotinterval
            yield return new WaitForSeconds(turret.shotInterval);
            //then fire a single bullet
            GetBullet(turret);
        }
    }

    public void ShootCaller(TurretObjects turret)
    {
        //fires the specific method depending on turret type
        switch(turret.turretType)
        {
            case TurretObjects.shootingType.None:
                break;
            case TurretObjects.shootingType.Burst:
                Burst(turret);
                break;
            case TurretObjects.shootingType.Repeater:
                Repeater(turret);
                break;
        }
    }

    //get the entity speed that uses this system
    public void GetShipSpeed(float speed)
    {
        shipSpeed = speed;
    }
}
