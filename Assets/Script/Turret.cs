using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

    //public enum shootingType
    //{
    //    none, repeater, burst
    //}
    //public shootingType turretType;
    public float shotInterval = 0f;
    public float burstInterval = 0f;
    public float burstDelay = 0f;
    float nextShot = 0.0f;
    float nextBurst = 0.0f;
    public float burstSize = 0f;

    // Use this for initialization
    void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    public void GetBullet()
    {
        //get a bullet from the pool, search by tag set in the unity inspector, on the prefab
        GameObject bullet = ObjectPooler.sharedInstance.GetPooledObject("EnemyBullet");
        //just to be sure we aren't firing blanks
        if (bullet != null)
        {
            //set the bullet's start position at the location of the bullet spawn point
            bullet.transform.position = this.transform.position;
            //will go in the direction the gatling gun is pointing towards
            bullet.transform.rotation = this.transform.rotation;
            bullet.SetActive(true);
        }
    }

    public void Repeater()
    {
        if (Time.time > nextShot)
        {
            nextShot = Time.time + shotInterval;
            GetBullet();
        }
    }

    public void Burst()
    {
        if(Time.time >nextBurst)
        {
            nextBurst = Time.time + burstInterval+burstDelay;
            StartCoroutine(BurstShots());
        }
    }

    public IEnumerator BurstShots()
    {
        int i = 0;
        shotInterval = burstInterval / burstSize;
        while(i<burstSize)
        {
            i++;
            yield return new WaitForSeconds(shotInterval);
            GetBullet();
        }
    }

    public void ShootCaller(string turretType)
    {
        Invoke(turretType.ToString(), 0.0f);
    }
}
