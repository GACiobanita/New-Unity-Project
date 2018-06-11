using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the secondary points attached to a spawn point in the level editor
public class SecondaryPoint : MonoBehaviour {

    //the spawnpoint/controlpoint to which this secondary point is attached to
    public GameObject controlPoint;
    //the position in the list of points, belonging to the controlpoint, of this point
    public int posInList;

    //mouse drag functionality from Unity editor
    //left mouse click to drag
    //object this is attached to needs a box collider to function
    private void OnMouseDrag()
    {
        //the mouse position when dragging
        //converted from screen to world point position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //removing the Z value
        mousePos = new Vector3(mousePos.x, mousePos.y, 0.0f);
        //update the position of this point in the control point
        Debug.Log(posInList);
        controlPoint.GetComponent<EditorSpawnPoint>().UpdatePivotPosition(mousePos, posInList);
    }

    //method used to setup secondary points values
    public void SetupPivotPoint(GameObject go, int i)
    {
        controlPoint = go;
        posInList = i;
    }
}
