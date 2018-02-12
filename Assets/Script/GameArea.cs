using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//this script should cover the game area in which the game itself should happen
public class GameArea : MonoBehaviour
{
    //this value is used to call GameArea script function in other scripts, e.g. GameArea.sharedInstance.CheckPosition(vector3);
    public static GameArea sharedInstance;
    //need the camera of the game, in order to set up the screen borders based on the game camera
    //all objects/art should be set based on the game camera size
    public Camera gameCamera;

    private void Awake()
    {
        //when this is activated
        sharedInstance = this;
    }

    //check if the passed vector3 is between screen bounds
    //could use some extra touches
    public bool CheckPosition(Vector3 objectPosition)
    {
        //transform the passed vector3 into a screen coordinate with values between 0 and 1 on all axis
        Vector3 viewportPosition = gameCamera.WorldToViewportPoint(objectPosition);
        viewportPosition = new Vector2(Mathf.Clamp01(viewportPosition.x), Mathf.Clamp01(viewportPosition.y));
        //left border
        if (viewportPosition.x < 0.17f)
            return false;
        //right border
        if (viewportPosition.x > 0.83f)
            return false;
        //top border
        if (viewportPosition.y == 0.0f)
            return false;
        //bottom border
        if (viewportPosition.y == 1.0f)
            return false;
        return true;
    }
}
