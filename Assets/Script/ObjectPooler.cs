using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]//allows for instances of this class to be editable from the inspector
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int amountToPool;
    public bool shouldExpand = true;
}

public class ObjectPooler : MonoBehaviour {


    public static ObjectPooler sharedInstance; //allows other scripts to access object pooler without getting a component from a GameObject
    public List<GameObject> pooledObjects;
    public List<ObjectPoolItem> itemsToPool;

    void Awake()
    {
        sharedInstance = this;
    }

    // Use this for initialization
    void Start () {
        pooledObjects = new List<GameObject>();
        foreach(ObjectPoolItem item in itemsToPool)//iterate through all instances of objectpoolitem
        {
            for(int i=0; i<item.amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }

    public GameObject GetPooledObject(string tag)
    {
        //iterate through the pooledObjects list.
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            //check to see if the item in your list is not currently active in the Scene. If it is, the loop moves to the next object in the list.
            //If not, you exit the method and hand the inactive object to the method
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag==tag)
            {
                return pooledObjects[i];
            }
        }
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.tag == tag)
            {
                //if there is no inactive object, exit the method
                if (item.shouldExpand)
                {
                    GameObject obj = (GameObject)Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                    return obj;
                }
            }
        }
        return null;
    }
}
