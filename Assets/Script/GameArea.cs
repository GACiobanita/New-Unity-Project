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
    public Camera gameCamera;
    public List<GameObject> gameBounds;
    //value used to make user that the bounds cover the entire edge of the screen
    public float boundsOverhand;
    //should replace this with a special list that also has: size, the bool for list expansion in the object pooler, and the gameobject
    //very bad
    public GameObject playerShip;
    public GameObject enemyShip;
    public GameObject playerBullet;
    public GameObject enemyBullet;
    public Text scoreText;
    private float gameScore=0.0f;

    private void Awake()
    {
        //when this is activated
        sharedInstance = this;
    }

    private void Start()
    {
        gameScore = 0;
        GetResources();
        PrepareObjectPooler();
        ObjectPooler.sharedInstance.TriggerObjectPooling();
        CreatePlayer();
    }

    private void Update()
    {
        Debug.DrawLine(gameCamera.ScreenToWorldPoint(new Vector2(gameCamera.pixelWidth * 0.17f, gameCamera.pixelHeight / 2)), gameCamera.ScreenToWorldPoint(new Vector2(gameCamera.pixelWidth - gameCamera.pixelWidth * 0.17f, gameCamera.pixelHeight / 2)), Color.red, 1000, true);
    }

    public void GetResources()
    {
        playerShip = Resources.Load("playerShip") as GameObject;
        enemyShip = Resources.Load("enemy1") as GameObject;
        playerBullet = Resources.Load("bullet") as GameObject;
        enemyBullet = Resources.Load("enemyBullet") as GameObject;
    }

    void CreatePlayer()
    {
        Vector2 shipPos = gameCamera.ScreenToWorldPoint(new Vector2(gameCamera.pixelWidth / 2, gameCamera.pixelHeight/4));
        playerShip.GetComponent<ShipControl>().moveJoystick = GameObject.FindGameObjectWithTag("MovementController").GetComponent<VirtualJoystick>();
        playerShip = Instantiate(playerShip, shipPos ,Quaternion.identity);
    }

    public void PrepareObjectPooler()
    {
        ObjectPooler.sharedInstance.AddItemToPool(playerBullet, 2, true);
        ObjectPooler.sharedInstance.AddItemToPool(enemyBullet, 2, true);
        ObjectPooler.sharedInstance.AddItemToPool(enemyShip, 2, true);
    }

    //don't forget to make it so they are at they properly position at the edge of the 66% of the screen area
    //this is too hardcoded
    public void CreateArenaBoundaries()
    {

        //from Unity Documentation
        //A screen space point is defined in pixels.
        //The bottom-left of the screen is (0,0); 
        //the right-top is (pixelWidth, pixelHeight).
        //The z position is in world units from the Camera.
        //top
        gameBounds[0].transform.position = gameCamera.ScreenToWorldPoint(new Vector2(gameCamera.pixelWidth / 2, gameCamera.pixelHeight));
        //from the Unity Documentation
        //The orthographicSize is half the size of the vertical viewing volume
        //aka
        //half the size of the current view
        //The horizontal size of the viewing volume depends on the aspect ratio: width*aspectratio
        gameBounds[0].GetComponent<BoxCollider2D>().size = new Vector2(gameCamera.orthographicSize * 2 * gameCamera.aspect+ boundsOverhand, 0.1f);
        //bottom
        gameBounds[1].transform.position = gameCamera.ScreenToWorldPoint(new Vector2(gameCamera.pixelWidth / 2, 0.0f));
        gameBounds[1].GetComponent<BoxCollider2D>().size = new Vector2(gameCamera.orthographicSize * 2 * gameCamera.aspect + boundsOverhand, 0.1f);
        //left
        gameBounds[2].transform.position = gameCamera.ScreenToWorldPoint(new Vector2(gameCamera.pixelWidth*0.17f, gameCamera.pixelHeight / 2));
        gameBounds[2].GetComponent<BoxCollider2D>().size = new Vector2(0.1f, gameCamera.orthographicSize * 2+ boundsOverhand);
        //right
        gameBounds[3].transform.position = gameCamera.ScreenToWorldPoint(new Vector2(gameCamera.pixelWidth-gameCamera.pixelWidth * 0.17f, gameCamera.pixelHeight / 2));
        gameBounds[3].GetComponent<BoxCollider2D>().size = new Vector2(0.1f, gameCamera.orthographicSize * 2+ boundsOverhand);
    }

    public void UpdateScore(float amount)
    {
        gameScore += amount;
        scoreText.text = gameScore.ToString();
    }

    public void ScorePrompt(Vector3 pos)
    {
        GameObject flavour = ObjectPooler.sharedInstance.GetPooledObject("Flavour");
        flavour.transform.position = pos;
        flavour.SetActive(true);
    }
}
