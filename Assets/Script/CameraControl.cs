using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//camera controls while using the level editor
public class CameraControl : MonoBehaviour {

    public static CameraControl sharedInstance;
    public float cameraSpeed=2.0f;
    Vector3 cameraEndPos;
    bool shouldMove=false;

    private void Awake()
    {
        sharedInstance = this;
    }

    private void Update()
    {
        if(shouldMove)
        {
            CameraMovement();
        }
    }

    public void SetCameraSpeed(float speed)
    {
        cameraSpeed = speed;
    }

    public float GetCameraSpeed()
    {
        return cameraSpeed;
    }

    public void CameraMovement()
    {
        if (Camera.main.transform.position.y < cameraEndPos.y)
        {
            this.transform.position = Vector3.MoveTowards(Camera.main.transform.position, cameraEndPos, cameraSpeed * Time.deltaTime);
        }
        else
        {
            shouldMove = false;
            StartCoroutine(GameArea.sharedInstance.EndGame());
        }
    }

    public void SetTargetPosition(float endPos)
    {
        cameraEndPos = new Vector3(0.0f, endPos, -10.0f);
    }

    public float GetTargetPosition()
    {
        return cameraEndPos.y;
    }

    public void StartMovement()
    {
        shouldMove = true;
    }

    public void EndMovement()
    {
        shouldMove = false;
    }

    public bool IsObjectInView(Vector3 pos, float objWidth, float objHeight)
    {
        //this method is only used to determine whether the object is in the screen area where gameplay
        //happens, or if it hides behind UI
        if (XAxisConstraint(pos, objWidth))
            if (YAxisConstraint(pos, objHeight)) 
                return true;
        return false;    
    }

    bool XAxisConstraint(Vector3 pos, float objWidth)
    {
        Vector3 leftPos = Camera.main.WorldToScreenPoint(new Vector2(pos.x - objWidth / 2, pos.y));
        Vector3 rightPos = Camera.main.WorldToScreenPoint(new Vector2(pos.x + objWidth / 2, pos.y));
        if(Camera.main.pixelWidth * 0.17f < leftPos.x && Camera.main.pixelWidth - Camera.main.pixelWidth * 0.17f > rightPos.x)
                return true;
        return false;
    }

    bool YAxisConstraint(Vector3 pos, float objHeight)
    {
        Vector3 topPos = Camera.main.WorldToScreenPoint(new Vector2(pos.x, pos.y + objHeight/2));
        Vector3 botPos = Camera.main.WorldToScreenPoint(new Vector2(pos.x, pos.y - objHeight / 2));
        if (0.0f < botPos.y && Camera.main.pixelHeight > topPos.y)
            return true;
        return false;
    }
}
