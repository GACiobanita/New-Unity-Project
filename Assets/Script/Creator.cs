using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creator : MonoBehaviour {

    public static Creator sharedInstance;
    //the UI in editor move
    public Canvas builderCanvas;
    //the ui used in play mode
    public Canvas foregroundCanvas;
    //horizontal size is always 1280(resolution, 16 blocks of 80x84) in the editor, this sets the vertical size of the level, levelSizeInput*blocks(80x84)
    public InputField levelSizeInput;
    //Unity UI input for each of the enemy speeds part of the selected spawn point
    public InputField baseSpeedInput;
    public InputField loopSpeedInput;
    public InputField oolSpeedInput;
    //set a camera speed that is also used for camera movement speed in the editor
    //currently not passed in the level file
    public InputField cameraSpeedInput;
    //the object name part of the selected spawn point
    public Text selectedName;
    //the selected spawn point
    public EditorSpawnPoint currentlySelectedSP;
    //unity UI dropdown list of all the enemies that can be placed in the editor
    public Dropdown toyList;
    //sprite matching name of attached object
    [System.Serializable]
    public class SpriteMatch
    {
        public string objName;
        public Sprite objSprite;
    }
   
    public List<SpriteMatch> toySprites;
    //spawn point prefab reference
    public GameObject spawnPoint;
    //the bg tile used for level measurement
    GameObject bgObj;
    //the lengths of the bgObj
    float spriteLength;
    float spriteHeight;
    //used to determine how many bjObj will be placed
    //vertical size
    int levelSize;
    //the corner from each the bgObj will start being placed
    Vector3 bottomLeftCorner;
    //the linerenderer used to show the level borders in which spawnpoints can be placed
    LineRenderer editorLineRenderer;
    //current spawnpoints active in scene
    List<GameObject> spawnPointsInScene;
    //object that contains all bgObj for cleaner view int he inspector
    GameObject backgroundParent;
    //the maximum position the camera can move to determined by the levelSize
    float cameraMaxYPos;
    bool editorMode = true;
    //camera orthographic size
    //min size is the usual level size
    public float minCameraSize;
    public float maxCameraSize;
    //when using the mouse scroll wheel
    public float zoomSpeed = 0.2f;
    //location where the user right clicked and started to move either up or down
    Vector3 dragOrigin;
    //camera movement speed up or down the level
    public float dragSpeed = 1.0f;

    // Use this for initialization
    void Start () {
        minCameraSize = Camera.main.orthographicSize;
        maxCameraSize = 2 * minCameraSize;
        sharedInstance = this;
        spawnPointsInScene = new List<GameObject>();
        editorLineRenderer = this.GetComponent<LineRenderer>();
        bgObj = Resources.Load("background_square") as GameObject;
        spawnPoint.GetComponent<EditorSpawnPoint>().SetCreatorManager(this);
        //sizes of the bgObj
        spriteHeight = 0.8f;
        spriteLength = 0.84f;
        //the world point location of the bottom left corner from which the bgObj will the placed
        //the bottomLeftCorner is the corner of the scene in which gameplay takes place
        bottomLeftCorner = Camera.main.ScreenToWorldPoint(new Vector2(0.17f * Screen.width, 0.0f));
        backgroundParent = new GameObject();
        backgroundParent.name = "backgroundParent";
        //set resources
        GameArea.sharedInstance.EditorResources();
    }
	
	// Update is called once per frame
	void Update () {
        //in editor mode the camera can be controlled
        if (editorMode)
        {
            if(toyList.value!=0)
                CreatePath();
            Zoom();
            MouseDrag();
        }
        else
        {
            CameraControl.sharedInstance.CameraMovement();
            //if escape is pressed, play mode is cancelled
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                editorMode = true;
                builderCanvas.gameObject.SetActive(true);
                foregroundCanvas.gameObject.SetActive(false);
                GameArea.sharedInstance.EditorEnd();
                LineRendererVisibility();
                GameArea.sharedInstance.DeactivateBoundaries();
                ObjectPooler.sharedInstance.DeactivateObjects();
                ResetCamera();
            }
        }
	}

    public void GetLevelSize()
    {
        levelSize = int.Parse(levelSizeInput.text);
        SetLevelSize(levelSize);
    }

    public void GetLevelSizeFromCameraPos(int cameraSize)
    {
        SetLevelSize((int)(cameraSize / spriteHeight + 10));
    }

    //after setting the levelsize in the Unity UI Input, the bgObj will be placed horizontally and vertically
    //using it's sizes ar reference
    public void SetLevelSize(int lSize)
    {
        if (GameObject.FindGameObjectWithTag("Background"))
        {
            GameObject[] bgList = GameObject.FindGameObjectsWithTag("Background");
            for (int i = 0; i < bgList.Length; i++)
            {
                Destroy(bgList[i]);
            }
        }

        Vector2 pos = new Vector2(bottomLeftCorner.x + spriteLength / 2, bottomLeftCorner.y + spriteHeight / 2);
        for (int i = 0; i < lSize; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Instantiate(bgObj, pos, Quaternion.identity, backgroundParent.transform);
                pos = new Vector2(pos.x + spriteLength, pos.y);
            }
            pos = new Vector2(bottomLeftCorner.x + spriteLength / 2, pos.y + spriteHeight);
        }

        ResetCamera();


        cameraMaxYPos = (lSize-10) * spriteHeight;

        CameraControl.sharedInstance.SetTargetPosition(cameraMaxYPos);

        SetLevelBorders(lSize);
    }

    private void OnGUI()
    {
        //draw the currently selected toy under the mouse pointer
        if(toyList.value!=0)
        {
            GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 32, 32), toyList.captionImage.mainTexture);
        }
    }

    //create a spawn point at the mouse pointer world location
    void CreatePath()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //if left shit is  held down and left mouse button is pressed, while inside the level borders
        //create a spawnpoint
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Mouse0) && (pos.x < 6.4 && pos.x > -6.4))
        {
            CreateSpawnPoint(pos, toyList.captionText.text);
        }
    }

    void CreateSpawnPoint(Vector2 pos, string objName)
    {
        Sprite objSprite=null;
        for (int i = 0; i < toySprites.Count; i++)
        {
            if (objName == toySprites[i].objName)
            {
                objSprite = toySprites[i].objSprite;
                break;
            }
        }

        spawnPoint.GetComponent<SpriteRenderer>().sprite = objSprite;
        spawnPoint.gameObject.name = "spawnPoint" + spawnPointsInScene.Count;
        spawnPointsInScene.Add((GameObject)Instantiate(spawnPoint, pos, Quaternion.identity));
        spawnPointsInScene[spawnPointsInScene.Count - 1].GetComponent<EditorSpawnPoint>().AddObject(objName);
    }

    void SetLevelBorders(int lSize)
    {
        //starting from I going towards VII to draw the level boarders + marking the first screen of the game
        //          II X-->--X III
        //             .     .
        //             ^     V
        //             .     .
        //      V < -I X--<--X IV->VIII
        //             .     .
        //             V     ^
        //             .     .
        //          VI X-->--X VII
        // I
        Vector2 pos=Camera.main.ScreenToWorldPoint(new Vector2(0.0f, Screen.height));
        editorLineRenderer.SetPosition(0, pos);
        // II
        pos = new Vector2(pos.x, lSize * 0.8f - pos.y);
        editorLineRenderer.SetPosition(1, pos);
        // III
        pos = new Vector2(-pos.x, pos.y);
        editorLineRenderer.SetPosition(2, pos);
        // IV
        pos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        editorLineRenderer.SetPosition(3, pos);
        // V
        pos = Camera.main.ScreenToWorldPoint(new Vector2(0.0f, Screen.height));
        editorLineRenderer.SetPosition(4, pos);
        // VI 
        pos = Camera.main.ScreenToWorldPoint(new Vector2(0.0f, 0.0f));
        editorLineRenderer.SetPosition(5, pos);
        // VII 
        pos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0.0f));
        editorLineRenderer.SetPosition(6, pos);
        // VIII
        pos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        editorLineRenderer.SetPosition(7, pos);


    }

    //remove the point whe deleted from the editor
    public void RemovePointsFromControlList(GameObject go)
    {
        spawnPointsInScene.Remove(go);
    }

    public void StartTest()
    {
        Debug.Log("Start Test, Creator Script line 110");
        editorMode = false;
        ResetCamera();
        builderCanvas.gameObject.SetActive(false);
        foregroundCanvas.gameObject.SetActive(true);
        GameArea.sharedInstance.EditorStart();
        LineRendererVisibility();
        GameArea.sharedInstance.ActivateBoundaries();
    }

    //in play mode only the spawnpoints are visible from the editor view
    public void LineRendererVisibility()
    {
        foreach (GameObject sp in spawnPointsInScene)
        {
            sp.GetComponent<EditorSpawnPoint>().PointsVisibility();
        }
    }

    public void SetEnemySpeed()
    {
        if(currentlySelectedSP)
            currentlySelectedSP.SetSpeedValue(float.Parse(baseSpeedInput.text), float.Parse(loopSpeedInput.text), float.Parse(oolSpeedInput.text));
    }

    public List<GameObject> GetSpawnPointList()
    {
        return spawnPointsInScene;
    }

    public float ReturnLevelSize()
    {
        return levelSize;
    }

    public void DisplayEnemySpeed(float baseSpeed, float loopSpeed, float oolSpeed, EditorSpawnPoint sp)
    {
        baseSpeedInput.text = baseSpeed.ToString();
        loopSpeedInput.text = loopSpeed.ToString();
        oolSpeedInput.text = oolSpeed.ToString();
        currentlySelectedSP = sp;
        selectedName.text = currentlySelectedSP.name;
    }

    public void Zoom()
    {
        //check if the middle mouse button is moving 
        //actually changes the camera orthographic size and not the camera position in 3D, does not move camera on Z axis
        //this one reduces the orthographic size , making the level seem bigger
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && (Camera.main.orthographicSize - zoomSpeed) > minCameraSize)
        {
            //if scroll wheel is going upwards AND the camera size will not get smaller than the camera min size value then do the following
            for (int sensitivityOfScrolling = 3; sensitivityOfScrolling > 0; sensitivityOfScrolling--) Camera.main.orthographicSize = Camera.main.orthographicSize - zoomSpeed;
        }
        //increases the camera orthographic size, making the level seem smaller
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && (zoomSpeed + Camera.main.orthographicSize) < maxCameraSize)
        {
            for (int sensitivityOfScrolling = 3; sensitivityOfScrolling > 0; sensitivityOfScrolling--) Camera.main.orthographicSize = Camera.main.orthographicSize + zoomSpeed;
        }
    }

    //method is called in order to check for a button press
    public void MouseDrag()
    {
        //record the mouse pointer position on right click
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        //if there is no button press, exit this method
        if (!Input.GetMouseButton(1)) return;

        //since the right mouse button has been pressed, check
        //where the mouse pointer is going towards(up or down) in order to move the camera
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        //calculate move
        Vector3 move = new Vector3(0.0f, pos.y * dragSpeed, 0.0f);

        Camera.main.transform.Translate(move, Space.World);
    }

    public void ResetCamera()
    {
        //reset camera position in the level to the first screen of the level
        Camera.main.transform.position = new Vector3(0.0f, 0.0f, -10.0f);
        //camera size is also reset to it's original value
        Camera.main.orthographicSize = minCameraSize;
    }

    public void LoadCameraSpeed(int cameraSpeed)
    {
        CameraControl.sharedInstance.SetCameraSpeed(cameraSpeed);
        cameraSpeedInput.text = cameraSpeed.ToString();
    }

    public void PassCameraSpeed()
    {
        CameraControl.sharedInstance.SetCameraSpeed(float.Parse(cameraSpeedInput.text));
    }

    public void LoadFile()
    {
        FileReader.OpenExplorer();
    }

    public void CreateSpawnPoints(List<string> spawnPointPlacement)
    {
        string[] spawnPointInfo;
        string[] spawnPos;
        float pointCount;
        float oolSpeed;
        float loopSpeed;
        float baseSpeed;
        string objName;
        string isLooping;
        foreach (string sp in spawnPointPlacement)
        {
            spawnPointInfo = sp.Split('_');
            spawnPos = spawnPointInfo[0].Split(',');
            Vector2 pos = new Vector2(float.Parse(spawnPos[0]), float.Parse(spawnPos[1]));    
            //if there are any secondary points
            objName = spawnPointInfo[spawnPointInfo.Length - 4];
            CreateSpawnPoint(pos, objName);

            oolSpeed = float.Parse(spawnPointInfo[spawnPointInfo.Length - 1]);
            loopSpeed = float.Parse(spawnPointInfo[spawnPointInfo.Length - 2]);
            baseSpeed = float.Parse(spawnPointInfo[spawnPointInfo.Length - 3]);
            isLooping = spawnPointInfo[spawnPointInfo.Length - 5];
            spawnPointsInScene[spawnPointsInScene.Count-1].GetComponent<EditorSpawnPoint>().SetSpeedValue(baseSpeed, loopSpeed, oolSpeed);
            spawnPointsInScene[spawnPointsInScene.Count - 1].GetComponent<EditorSpawnPoint>().SetupLooping(isLooping);
            ////number of secondary points
            pointCount = float.Parse(spawnPointInfo[1]);
            if (pointCount > 0)
            {
                //add them to the point list
                for (int j = 2; j < spawnPointInfo.Length - 5; j++)
                {
                    string[] pointPos = spawnPointInfo[j].Split(',');
                    Vector2 location = new Vector2(float.Parse(pointPos[0]), float.Parse(pointPos[1]));
                    spawnPointsInScene[spawnPointsInScene.Count - 1].GetComponent<EditorSpawnPoint>().CreateSecondaryPoint(location);
                }
            }
        }
    }

    public void DeleteScene()
    {
        foreach(GameObject go in spawnPointsInScene)
        {
            Destroy(go);
        }
        spawnPointsInScene.Clear();
        CameraControl.sharedInstance.SetCameraSpeed(2.0f);
        SetLevelSize(0);
        ResetCamera();
        cameraSpeedInput.text = "2";
    }
}