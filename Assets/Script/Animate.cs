using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//animation component for the player ship
//shit code, only for 5 frames and only for a single entity
[System.Serializable]
public enum AnimStates
{
    //the animation frames of the player ship
    FarLeft=0, SlightLeft, Idle, SlightRight, FarRight
}

public class Animate : MonoBehaviour {

    //at what animation frame is the ship currently at
    SpriteRenderer sr;
    [HideInInspector]
    public AnimStates currentState = AnimStates.Idle;
    //a list of the animation frames
    //when setting up the list in the editor follow the same 
    //animation pattern as in the AnimStates enum
    //e.g. 0 is farleft, 1 is slight left, 3 is idle, 4 is slight right, 5 is far right
    //meaning that idle anim frame should be in the middle
    public List<Sprite> animSprites;
    //timers as to when the ship should change sprite according to the animation frame
    public float slightTimer = 0.0f;
    public float farTimer = 0.0f;
    public float rightTimer = 0.0f;
    public float leftTimer = 0.0f;
    //the game object this script is attached to
    public GameObject go;
    //move joystick input in order to determine which animation frame should happen
    [HideInInspector]
    public VirtualJoystick moveJoystick;
    bool animating = false;

    private void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
    }

    public void ShipAnim()
    {
        //this would only work for horizontal animation
        //if there is input from the joystick
        if (moveJoystick.inputDirection != Vector3.zero)
        {
            //if the joystick/ship movement is towards the right direction, x++
            if (moveJoystick.inputDirection.x > 0.0f)
            {
                //start the timer for the right animation frames
                rightTimer += Time.deltaTime;
                //reset the left animation frames timer
                leftTimer = 0.0f;
            }
            else
            {
                //the left direction, x--
                if (moveJoystick.inputDirection.x < 0.0f)
                {
                    //reset the timer for the right anim frames
                    rightTimer = 0.0f;
                    //start the timer for the left anim frames
                    leftTimer += Time.deltaTime;
                }
            }
        }
        else
        {
            //if there is movement from the joystick, reset all timers
            leftTimer = 0.0f;
            rightTimer = 0.0f;
            //and the current sprite of the ship should be idle
            currentState = AnimStates.Idle;
        }
        //determine the state in order to determine the sprite
        DetermineState();
        ChangeSprite();
    }

    public void DetermineState()
    {
        if(rightTimer>0.0f)
        {
            //based on how much time has passed, determine with frame to use
            if(rightTimer>slightTimer && rightTimer< farTimer)
            {
                currentState = AnimStates.SlightRight;
            }
            else if(rightTimer>farTimer)
            {
                currentState = AnimStates.FarRight;
            }
        }
        else
        {
            if (leftTimer > 0.0f)
            {
                if (leftTimer > slightTimer && leftTimer < farTimer)
                {
                    currentState = AnimStates.SlightLeft;
                }
                else if(leftTimer>farTimer)
                {
                    currentState = AnimStates.FarLeft;
                }
            }
            else
            {
                //keep using the idle frame until the timers reach a point where the sprite can change
                if(currentState!=AnimStates.Idle)
                {
                    currentState = AnimStates.Idle;
                }
            }
        }
    }

    public void ChangeSprite()
    {
        if (go.GetComponent<SpriteRenderer>().sprite != animSprites[(int)currentState])
        {
            //change the sprite of the ship using the currentState
            go.GetComponent<SpriteRenderer>().sprite = animSprites[(int)currentState];
            //if(go.GetComponent<BoxCollider2D>()!=null)
            //if (currentState == AnimStates.FarLeft || currentState==AnimStates.FarRight)
            //{
            //    go.GetComponent<BoxCollider2D>().size = new Vector2(0.5f, go.GetComponent<BoxCollider2D>().size.y);
            //}
            //else
            //{
            //    go.GetComponent<BoxCollider2D>().size = new Vector2(1.0f, go.GetComponent<BoxCollider2D>().size.y);
            //}
        }
    }

    //1 second flicker
    public IEnumerator SpriteFlicker(float duration)
    {
        for(float f=duration; f>=0; f-=0.1f)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        sr.enabled = true;
    }

    public IEnumerator HeadToPos(Vector3 pos, float time)
    {
        animating = true;
        Vector3 A = this.transform.position;
        float t = 0.0f;
        while(t<1.0f)
        {
            t += Time.deltaTime / time;
            this.transform.position = Vector3.Lerp(A, pos, t);
            yield return null;
        }
        animating = false;
    }

    public IEnumerator HeadToObj(Transform go, float time)
    {
        animating = true;
        Vector3 A = this.transform.position;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime / time;
            this.transform.position = Vector3.Lerp(A, go.transform.position, t);
            yield return null;
        }
        animating = false;
    }

    public bool IsAnimating()
    {
        return animating;
    }
}
