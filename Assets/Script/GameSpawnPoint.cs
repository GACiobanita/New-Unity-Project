using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSpawnPoint : MonoBehaviour
{
    //speeds to be added to the enemy entity
    public float eBaseSpeed = 0;
    public float eLoopSpeed = 0;
    public float eOolSpeed = 0;
    //the list of points attached to this, also contains the spawn/control point on the first position
    public List<Vector2> points;
    //name of the attached object
    public string objName;
    //the actual attached entity object
    public GameObject attachedObj;
    //is the attache object looping through points?
    bool isLooping = false;

    private void Start()
    {
        isLooping = false;
    }

    public virtual void SetRouteVectors(List<Vector2> list)
    {
        points = new List<Vector2>();
        foreach(Vector2 p in list)
        {
            points.Add(p);
        }
    }

    //pass the current list of points available
    public virtual List<Vector2> PassPoints()
    {
        return points;
    }

    public virtual void SpawnObject()
    {
        AddObject();
        //attached obj pos is the spawn points pos
        attachedObj.transform.position = this.transform.position;
        //activate it
        attachedObj.SetActive(true);
    }

    //in the editor there is a box collider that will trigger this method
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "SpawnBoundary")
        {
            SpawnObject();
        }
    }

    public void GetEntityName(string name)
    {
        objName = name;
    }

    //attach the entity to this spawn point
    public void AddObject()
    {
        //get object from the pooler
        attachedObj = ObjectPooler.sharedInstance.GetPooledObject(objName);
        //and the entity's default speed values
        if(attachedObj.tag=="Enemy")
        {
            EnemyObject();
        }
    }

    public void EnemyObject()
    {
        attachedObj.GetComponent<Enemy>().baseSpeed = eBaseSpeed;
        attachedObj.GetComponent<Enemy>().loopSpeed = eLoopSpeed;
        attachedObj.GetComponent<Enemy>().outOfLoopSpeed = eOolSpeed;
        //setup enemy pathing using only a list of vector2 positions, and a bool for looping
        attachedObj.GetComponent<Enemy>().SetupPathing(PassPoints(), isLooping);
    }

    //method called when the Unity UI button is pressed to set new speed values
    //or when creating the spawnpoint during gameplay
    public void SetSpeedValue(float baseSpeed, float loopSpeed, float oolSpeed)
    {
        eBaseSpeed = baseSpeed;
        eLoopSpeed = loopSpeed;
        eOolSpeed = oolSpeed;
    }

    //for level file saving purposes
    public float GetBaseSpeed()
    {
        return eBaseSpeed;
    }

    //for level file saving purposes
    public float GetLoopSpeed()
    {
        return eLoopSpeed;
    }

    //for level file saving purposes
    public float GetOolSpeed()
    {
        return eOolSpeed;
    }

    //for level file saving purposes
    public virtual bool IsEntityLooping()
    {
        return isLooping;
    }
    
    public virtual void SetupLooping(string value)
    {
        if(value=="1")
        {
            isLooping = true;
            return;
        }
        isLooping = false;
    }

    public GameObject GetAttachedObject()
    {
        if (attachedObj != null)
            return attachedObj;
        else
            return null;
    }
}
