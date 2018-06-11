using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorSpawnPoint : GameSpawnPoint
{
    //level builder direct reference
    public Creator levelBuilder;
    //the list of points attached to this, also contains the spawn/control point on the first position
    public List<GameObject> pointsGO;
    //used for linerenderer to represent looping and subsequently also used to set looping
    bool isClosed = false;
    //secondary points entity
    public GameObject pivot;
    //positioning new secondary points on creation
    Vector3 nextPivotPosition;
    //the route renderer component of the spawn/control point
    public LineRenderer routeRenderer;

    private void Awake()
    {
        pointsGO = new List<GameObject>();
        pointsGO.Add(this.gameObject);
        routeRenderer = this.GetComponent<LineRenderer>();
        routeRenderer.sortingLayerName = "Foreground";
    }

    private void Update()
    {
        //call the linerenderer functionality
        DrawLine();
        //in order to move the spawnpoint only, the spacebar needs to be held down
        //when the spacebar is lifted all of the points have their parent set back to the spawnpoint
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (pointsGO[0].transform.childCount == 0)
            {
                for (int i = 1; i < pointsGO.Count; i++)
                {
                    pointsGO[i].transform.SetParent(pointsGO[0].transform);
                }
            }
        }
    }

    public override void SetRouteVectors(List<Vector2> list)
    {
        pointsGO.Add(this.gameObject);
        for (int i=1; i<list.Count; i++)
        {
            //instanciate the object
            GameObject reference = Instantiate(pivot, list[i], Quaternion.identity);
            reference.GetComponent<SecondaryPoint>().SetupPivotPoint(this.gameObject, pointsGO.Count);
            //then add to the list
            this.pointsGO.Add(reference);
            routeRenderer.positionCount = pointsGO.Count;
        }
    }

    //mouse over functionality
    //the spawnpoint needs a collider for this to work
    private void OnMouseOver()
    {
        //on left click and while the left control is held a secondarypoint is created
        //to the left
        if (Input.GetKeyDown(KeyCode.Mouse0) && Input.GetKey(KeyCode.LeftControl))
        {
            nextPivotPosition = pointsGO[pointsGO.Count - 1].transform.position;
            //instanciate the object
            CreateSecondaryPoint(nextPivotPosition + Vector3.left);
        }
        //right click and while left control is held
        //delete the last secondary point in the list
        if (Input.GetKeyDown(KeyCode.Mouse1) && Input.GetKey(KeyCode.LeftControl))
        {
            //destroy and remove the last point in the list
            Destroy(pointsGO[pointsGO.Count - 1]);
            pointsGO.RemoveAt(pointsGO.Count - 1);
            if(pointsGO.Count==1)
                nextPivotPosition = pointsGO[pointsGO.Count - 1].transform.position;
            //set the new line count
            //also remove looping if any
            routeRenderer.loop = false;
        }
        //right click will just set looping, only if there is atleast 1 other secondary point besides the 
        //spawn/control point
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            if (pointsGO.Count > 1)
            { 
                isClosed = !isClosed;
                routeRenderer.loop = isClosed;
            }
        }
        //select the current spawn point to set speed values
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            levelBuilder.DisplayEnemySpeed(eBaseSpeed, eLoopSpeed, eOolSpeed, this);
        }
    }

    public void CreateSecondaryPoint(Vector2 pos)
    {
        GameObject reference = Instantiate(pivot, pos, Quaternion.identity);
        reference.GetComponent<SecondaryPoint>().SetupPivotPoint(this.gameObject, pointsGO.Count);
        reference.transform.SetParent(this.transform);
        //then add to the list
        this.pointsGO.Add(reference);
    }

    //drag the points
    private void OnMouseDrag()
    {
        //drag only the spawnpoint while spacebar is held down
        if (Input.GetKey(KeyCode.Space))
        {
            //detach children so they dont move with the parent
            pointsGO[0].transform.DetachChildren();
        }
        //if spacebar is not held then all the object will move together with the parent point
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3(mousePos.x, mousePos.y, 0.0f);
        pointsGO[0].transform.position = mousePos;
    }

    void DrawLine()
    {
        routeRenderer.positionCount = pointsGO.Count;
        //for each point in the list set a new point in the line renderer
        for (int i = 0; i < pointsGO.Count; i++)
        {
            routeRenderer.SetPosition(i, pointsGO[i].transform.position);
        }
    }

    //called from the secondary point itself
    public void UpdatePivotPosition(Vector3 pos, int i)
    {
        pointsGO[i].transform.position = pos;
    }

    //pass the current list of points available
    public override List<Vector2> PassPoints()
    {
        List<Vector2> vectorPoints = new List<Vector2>();
        foreach (GameObject go in pointsGO)
        {
            vectorPoints.Add(go.transform.position);
        }
        return vectorPoints;
    }

    //make sure this list is removed from the main list containing all spawn points
    private void OnDestroy()
    {
        Creator.sharedInstance.RemovePointsFromControlList(this.gameObject);
    }

    //functionality needed for editor testing
    public override void SpawnEnemy()
    {
        //setup enemy pathing using only a list of vector2 positions, and a bool for looping
        attachedObj.GetComponent<Enemy>().SetupPathing(this.PassPoints(), routeRenderer.loop);
        if (eBaseSpeed != 0 && eLoopSpeed != 0 && eOolSpeed != 0)
        { 
            //setup the values
            attachedObj.GetComponent<Enemy>().baseSpeed = eBaseSpeed;
            attachedObj.GetComponent<Enemy>().loopSpeed = eLoopSpeed;
            attachedObj.GetComponent<Enemy>().outOfLoopSpeed = eOolSpeed;
        }
        //attached obj pos is the spawn points pos
        attachedObj.transform.position = this.transform.position;
        //activate it
        attachedObj.SetActive(true);
    }

    //in the editor there is a box collider that will trigger this method
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="SpawnBoundary")
        {
            SpawnEnemy();
        }
    }

    //attach the entity to this spawn point
    public void AddObject(string name)
    {
        objName = name;
        //get object from the pooler
        attachedObj = ObjectPooler.sharedInstance.GetPooledObject(objName);
        //and the entity's default speed values
        eBaseSpeed = attachedObj.GetComponent<Enemy>().baseSpeed;
        eLoopSpeed = attachedObj.GetComponent<Enemy>().loopSpeed;
        eOolSpeed = attachedObj.GetComponent<Enemy>().outOfLoopSpeed;
    }

    //method used to turn the points invisible in play mode, only the spawnpoint is visible
    public void PointsVisibility()
    {
        if(routeRenderer.enabled)
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
            routeRenderer.enabled = false;
            if(pointsGO.Count>1)
            {
                for (int i = 1; i < pointsGO.Count; i++)
                {
                    pointsGO[i].GetComponent<SpriteRenderer>().enabled = false;
                    pointsGO[i].GetComponent<BoxCollider2D>().enabled = false;
                }
            }
        }
        else
        {
            this.GetComponent<SpriteRenderer>().enabled = true;
            routeRenderer.enabled = true;
            if (pointsGO.Count > 1)
            {
                for (int i = 1; i < pointsGO.Count; i++)
                {
                    pointsGO[i].GetComponent<SpriteRenderer>().enabled = true;
                    pointsGO[i].GetComponent<BoxCollider2D>().enabled = true;
                }
            }
        }
    }

    //creator reference setup method
    public void SetCreatorManager(Creator manager)
    {
        levelBuilder = manager;
    }

    //for level file saving purposes
    public int IsLooping()
    {
        return routeRenderer.loop ? 1 : 0;
    }

    public override void SetupLooping(string value)
    {
        if (value == "1")
        {
            routeRenderer.loop = true;
            return;
        }
        routeRenderer.loop = false;
    }
}
