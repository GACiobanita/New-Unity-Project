using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomSpawn : MonoBehaviour {

    public List<GameObject> toys;
    public float horizontalChange=0.0f;
    public float minSpawnTime=1.0f;
    public float maxSpawnTime=2.0f;
    public float enemyShipScale = 1.0f;
    public Image background = null;
    public bool changeRes = false;

    private void Awake()
    {
        if(changeRes)
        {
            Screen.SetResolution(800, 1280, true);
        }
        foreach(GameObject go in toys)
        {
            go.transform.localScale = new Vector3(enemyShipScale, enemyShipScale, 0.0f);
        }
    }

    // Use this for initialization
    void Start () {
        LoadAmmunition();
        ObjectPooler.sharedInstance.TriggerObjectPooling();
        SpawnEnemy();
        StartCoroutine(WaitToSpawn());
    }

    private void Update()
    {
        ScrollBackground();
    }

    public void ScrollBackground()
    {
        Vector2 offset = new Vector2(0, Time.time * 0.5f);
        background.GetComponent<Image>().material.mainTextureOffset = offset;
    }

    Vector3 RandomXPos()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(horizontalChange, Screen.width), Screen.height, 0.0f));
        pos = new Vector3(pos.x, pos.y, 0.0f);
        return pos;
    }

    void SpawnEnemy()
    {
        float randomEnemy = Random.Range(0, toys.Count - 1);
        GameObject go = toys[(int)randomEnemy];
        go.transform.position = RandomXPos();
        Instantiate(go);
    }

    public void LoadAmmunition()
    {
        ObjectPooler.sharedInstance.AddItemToPool(Resources.Load("PlayerBullet") as GameObject, 2, true);
        ObjectPooler.sharedInstance.AddItemToPool(Resources.Load("EnemyBullet") as GameObject, 2, true);
    }

    IEnumerator WaitToSpawn()
    {
        yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
        SpawnEnemy();
        StartCoroutine(WaitToSpawn());
    }
}
