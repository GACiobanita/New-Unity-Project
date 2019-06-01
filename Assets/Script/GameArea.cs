using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;



//this script should cover the game area in which the game itself should happen
public class GameArea : MonoBehaviour
{
    //this value is used to call GameArea script function in other scripts, e.g. GameArea.sharedInstance.CheckPosition(vector3);
    public static GameArea sharedInstance;
    //need the camera of the game, in order to set up the screen borders based on the game camera
    //all objects/art should be set based on the game camera size
    public GameObject spawnBoundary;
    //value used to make user that the bounds cover the entire edge of the screen
    public float boundsOverhand;
    //should replace this with a special list that also has: size, the bool for list expansion in the object pooler, and the gameobject
    //very bad
    Transform shipSpawnPoint;
    Transform shipEntryPoint;
    Animation animComp;
    public GameObject playerShip;
    public GameObject spawnPoint;
    public List<GameObject> spawnPointsInLevel;
    public Image background=null;
    Animation fade = null;
    Button retryButton=null;
    public float bgScrollSpeed=0.5f;
    public float startDelay=5.0f;
    public SpriteRenderer startMessage;
    Sprite readySprite;
    Sprite goSprite;
    string levelFilename;
    float cameraSpeed;
    float cameraTargetPosition;
    string[] levelResources;
    List<string> levelEnemyPlacement;
    bool scroll = false;
    //gameArea constraints



    private void Awake()
    {
        //when this is activated
        sharedInstance = this;
        animComp = this.GetComponent<Animation>();
    }
    
    public void LoadResources()
    {
        LoadPlayer();
        for (int i=0; i<levelResources.Length; i++)
        {
            ObjectPooler.sharedInstance.AddItemToPool(Resources.Load(levelResources[i]) as GameObject, 2, true);
        }
        LoadAmmunition();
        ObjectPooler.sharedInstance.TriggerObjectPooling();
        //find the background object
        background = GameObject.FindGameObjectWithTag("Background").GetComponent<Image>();
        fade = GameObject.Find("Fade").GetComponent<Animation>();
        readySprite = Resources.Load("ready", typeof(Sprite)) as Sprite;
        goSprite = Resources.Load("go", typeof(Sprite)) as Sprite;
    }

    public void LoadAmmunition()
    {
        ObjectPooler.sharedInstance.AddItemToPool(Resources.Load("PlayerBullet") as GameObject, 2, true);
        ObjectPooler.sharedInstance.AddItemToPool(Resources.Load("EnemyBullet") as GameObject, 2, true);
    }

    public void LoadEverything()
    {
        ObjectPooler.sharedInstance.AddItemToPool(Resources.Load("enemyDoubleSlow") as GameObject, 2, true);
        ObjectPooler.sharedInstance.AddItemToPool(Resources.Load("enemyDoubleTurret") as GameObject, 2, true);
        ObjectPooler.sharedInstance.AddItemToPool(Resources.Load("enemyFourSpin") as GameObject, 2, true);
        ObjectPooler.sharedInstance.AddItemToPool(Resources.Load("enemySingleTurret") as GameObject, 2, true);
        ObjectPooler.sharedInstance.AddItemToPool(Resources.Load("enemySpikes") as GameObject, 2, true);
        ObjectPooler.sharedInstance.AddItemToPool(Resources.Load("PowerUpSpeed") as GameObject, 2, true);
        ObjectPooler.sharedInstance.AddItemToPool(Resources.Load("PowerUpShield") as GameObject, 1, true);
        ObjectPooler.sharedInstance.AddItemToPool(Resources.Load("Shield") as GameObject, 1, true);
    }

    public void LoadPlayer()
    {
        shipSpawnPoint = Camera.main.transform.GetChild(0);
        shipEntryPoint = Camera.main.transform.GetChild(1);
        playerShip = Resources.Load("playerShip") as GameObject;
        playerShip.GetComponent<ShipControl>().moveJoystick = GameObject.FindGameObjectWithTag("MovementController").GetComponent<VirtualJoystick>();
        playerShip.GetComponent<ShipControl>().SetOrigin(shipSpawnPoint);
        playerShip.GetComponent<ShipControl>().SetEntry(shipEntryPoint);
        playerShip = Instantiate(playerShip, shipSpawnPoint.position, Quaternion.identity, Camera.main.transform);
        playerShip.GetComponent<ShipControl>().GetPlayerPrefValues(PlayerSave.sharedInstance.GetLives(), PlayerSave.sharedInstance.GetRoF(), PlayerSave.sharedInstance.GetShield());
    }


    //editor functionality
    public void EditorResources()
    {
        LoadEverything();
        LoadAmmunition();
        CreateSpawnBoundary();
        ObjectPooler.sharedInstance.TriggerObjectPooling();
    }

    //editor start
    public void EditorStart()
    {
        LoadPlayer();
        playerShip.GetComponent<ShipControl>().EnterScene();
    }

    //editor end
    public void EditorEnd()
    {
        Destroy(GameObject.FindGameObjectWithTag("Player"));
    }

    private void Update()
    {
        if (background != null && scroll)
        {
            ScrollBackground();
        }
    }

    //don't forget to make it so they are at they properly position at the edge of the 66% of the screen area
    //this is too hardcoded
    public void CreateSpawnBoundary()
    {
        //from Unity Documentation
        //A screen space point is defined in pixels.
        //The bottom-left of the screen is (0,0); 
        //the right-top is (pixelWidth, pixelHeight).
        //The z position is in world units from the Camera.
        //top
        spawnBoundary.transform.position = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth / 2, Camera.main.pixelHeight));
        spawnBoundary.GetComponent<BoxCollider2D>().size= new Vector2(Camera.main.orthographicSize * 2 * Camera.main.aspect + boundsOverhand, 0.1f);
        //set camera as boundary parent
        spawnBoundary.transform.SetParent(Camera.main.transform);
    }

    public void ActivateBoundaries()
    {
        spawnBoundary.SetActive(true);
    }

    public void DeactivateBoundaries()
    {
        spawnBoundary.SetActive(false);
    }

    public void ScrollBackground()
    {
        Vector2 offset = new Vector2(0, Time.time * bgScrollSpeed);
        background.GetComponent<Image>().material.mainTextureOffset = offset;
    }

    public void GetLevelFilename(string filename)
    {
        levelFilename = filename;
    }

    public void GetLevelInformation()
    {
        levelEnemyPlacement = new List<string>();
        FileReader.ReadLevelFile(levelFilename, ref cameraSpeed, ref cameraTargetPosition, ref levelResources, ref levelEnemyPlacement);
        CreateSpawnPoints();
        LoadResources();
        CreateSpawnBoundary();
        CameraControl.sharedInstance.SetCameraSpeed(cameraSpeed);
        CameraControl.sharedInstance.SetTargetPosition(cameraTargetPosition);
        StartCoroutine(FadeOutScene());
        StartCoroutine(StartGame());
    }

    public IEnumerator StartGame()
    {
        startMessage.sprite = readySprite;
        animComp.Play("ready");
        yield return new WaitForSeconds(animComp.clip.length);
        startMessage.sprite = goSprite;
        animComp.Play("go");
        scroll = true;
        yield return new WaitForSeconds(animComp.clip.length);
        playerShip.GetComponent<ShipControl>().EnterScene();
        CameraControl.sharedInstance.StartMovement();
    }

    public IEnumerator EndGame()
    {
        scroll = false;
        playerShip.GetComponent<ShipControl>().ExitScene();
        if (SceneManagement.sharedInstance.GetCurrentScene() == "test") 
        StartCoroutine(FadeInScene());
        yield return null;
    }

    void CreateSpawnPoints()
    {
        string[] spawnPointInfo;
        string[] spawnPos;
        float pointCount;
        List<Vector2> pointsLocation;
        float oolSpeed;
        float loopSpeed;
        float baseSpeed;
        string objName;
        string isLooping;
        foreach(string levelenemy in levelEnemyPlacement)
        {
            pointsLocation = new List<Vector2>();
            spawnPointInfo = levelenemy.Split('_');
            spawnPos = spawnPointInfo[0].Split(',');
            //tranform position of the spawnpoint
            spawnPoint.transform.position = new Vector3(float.Parse(spawnPos[0]), float.Parse(spawnPos[1]), 0.0f);
            //add the spawnpoint as the first pos in the list
            pointsLocation.Add(spawnPoint.transform.position);
            //number of secondary points
            pointCount = float.Parse(spawnPointInfo[1]);
            //if there are any secondary points
            if(pointCount>0)
            {
                //add them to the point list
                for(int j=2; j<spawnPointInfo.Length-5; j++)
                {
                    string[] pointPos = spawnPointInfo[j].Split(',');
                    Vector2 location = new Vector2(float.Parse(pointPos[0]), float.Parse(pointPos[1]));
                    pointsLocation.Add(location);
                }
            }
            //the enemy speeds
            oolSpeed = float.Parse(spawnPointInfo[spawnPointInfo.Length - 1]);
            loopSpeed = float.Parse(spawnPointInfo[spawnPointInfo.Length - 2]);
            baseSpeed = float.Parse(spawnPointInfo[spawnPointInfo.Length - 3]);
            //the enemy name
            objName = spawnPointInfo[spawnPointInfo.Length - 4];
            //is the enemy looping through points?
            isLooping = spawnPointInfo[spawnPointInfo.Length - 5];
            spawnPoint.GetComponent<GameSpawnPoint>().SetRouteVectors(pointsLocation);
            spawnPoint.GetComponent<GameSpawnPoint>().GetEntityName(objName);
            spawnPoint.GetComponent<GameSpawnPoint>().SetSpeedValue(baseSpeed, loopSpeed, oolSpeed);
            spawnPoint.GetComponent<GameSpawnPoint>().SetupLooping(isLooping);
            spawnPointsInLevel.Add(Instantiate(spawnPoint));
        }
    }

    public void GameOver()
    {
        StartCoroutine(FadeInScene());
        CameraControl.sharedInstance.EndMovement();
        if(retryButton==null)
        {
            retryButton = GameObject.Find("Retry").GetComponent<Button>();
            retryButton.GetComponent<Image>().enabled = true;
            retryButton.onClick.AddListener(Retry);
        }
    }

    public void Retry()
    {
        retryButton.GetComponent<Image>().enabled = false;
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        if(gos.Length>0)
        {
            for(int i=0; i<gos.Length; i++)
            {
                gos[i].SetActive(false);
            }
        }
        if(bullets.Length>0)
        {
            for(int i=0; i<bullets.Length; i++)
            {
                bullets[i].SetActive(false);
            }
        }
        playerShip.SetActive(true);
        Camera.main.transform.position = Vector3.zero;
        playerShip.transform.position = shipSpawnPoint.transform.position;
        playerShip.GetComponent<ShipControl>().shipLives = 3;
        StartCoroutine(FadeOutScene());
        StartCoroutine(StartGame());
    }

    IEnumerator FadeOutScene()
    {
        fade.Play("fadeOut");
        yield return new WaitForSeconds(fade.clip.length);
    }

    IEnumerator FadeInScene()
    {
        fade.Play("fadeIn");
        yield return new WaitForSeconds(fade.clip.length);
    }
}
