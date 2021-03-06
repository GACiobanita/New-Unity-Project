﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]//allows for instances of this class to be editable from the inspector
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int amountToPool;
    public bool shouldExpand = true;

    public ObjectPoolItem(GameObject go, int amount, bool allowExpansion)
    {
        objectToPool = go;
        amountToPool = amount;
        shouldExpand = allowExpansion;
    }
}

public class ObjectPooler : MonoBehaviour {


    public static ObjectPooler sharedInstance; //allows other scripts to access object pooler without getting a component from a GameObject
    public List<GameObject> pooledObjects;
    public List<ObjectPoolItem> itemsToPool;

    public void Awake()
    {
        sharedInstance = this;
    }

    public void Start()
    {
        if(SceneManager.GetActiveScene().name=="EnemyCreation")
        {
            TriggerObjectPooling();
        }
    }

    public void AddItemToPool(GameObject go, int amount, bool allowExpansion)
    {
        ObjectPoolItem newItem=new ObjectPoolItem(go, amount, allowExpansion);
        itemsToPool.Add(newItem);
    }

    public void TriggerObjectPooling()
    {
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool)//iterate through all instances of objectpoolitem
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }

    public GameObject GetPooledObject(string name)
    {
        //iterate through the pooledObjects list.
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            //check to see if the item in your list is not currently active in the Scene. If it is, the loop moves to the next object in the list.
            //If not, you exit the method and hand the inactive object to the method
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].name==name)
            {
                return pooledObjects[i];
            }
        }
        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.name == name)
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

    public void DeactivateObjects()
    {
        foreach(GameObject obj in pooledObjects)
        {
            obj.SetActive(false);
        }
    }
}
