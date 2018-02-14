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
    public Vector3 CheckPosition(Vector3 objPos, Vector3 objDir, float objSize)
    {
        //transform the passed vector3 into a screen coordinate with values between 0 and 1 on all axis
        Vector3 viewportPosition = gameCamera.WorldToViewportPoint(new Vector3(objPos.x, objPos.y));
        Vector3 endPos = viewportPosition;
        viewportPosition = new Vector2(Mathf.Clamp01(viewportPosition.x), Mathf.Clamp01(viewportPosition.y));
        //direction based movement check
        if(objDir.x<0)
        {
            if (viewportPosition.x < 0.17f)
                endPos.x = 0.17f;
        }
        else
        {
            if (viewportPosition.x > 0.83f)
                endPos.x = 0.83f;
        }
        if(objDir.y<0)
        {
            if (viewportPosition.y == 0.0f)
                endPos.y = 0.0f;
        }
        else
        {
            if (viewportPosition.y == 1.0f)
                endPos.y = 1.0f;
        }
        return gameCamera.ViewportToWorldPoint(endPos);
    }
}
