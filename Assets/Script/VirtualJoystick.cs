using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

//script is attached directly to the joystick
//IDragHandler, IpointerDownHandle, IPointerUpHandler are used in the EventSystem for Unity UI, check https://docs.unity3d.com/ScriptReference/EventSystems.EventTrigger.html
public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    //joystick images
    private Image bgImg;
    private Image joyImg;

    //a vector3 that is the direction the joystick is pointing towards
    public Vector3 inputDirection { set; get; }

    private void Start()
    {
        //get image component of the parent object, the background image
        bgImg = GetComponent<Image>();
        //get image of the child object of the background image, the joystick image
        joyImg = transform.GetChild(0).GetComponentInChildren<Image>();
        //zero just to be sure
        inputDirection = Vector3.zero;
    }

    //when the joystick is moving
    public virtual void OnDrag(PointerEventData ped)
    {
        //have a position vector
        Vector2 pos = Vector2.zero;
        //calculate the position of the player finger
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform,ped.position, ped.pressEventCamera, out pos))
        {
            //compare against the background image
            pos.x = pos.x / bgImg.rectTransform.sizeDelta.x;
            pos.y = pos.y / bgImg.rectTransform.sizeDelta.y;

            //then compare the calculated position against the position
            //in order to find the direction of the point against the pivot, the center of the bgImg
            float x = (bgImg.rectTransform.pivot.x == 1) ? pos.x * 2 + 1 : pos.x * 2 - 1;
            float y = (bgImg.rectTransform.pivot.y == 1) ? pos.y * 2 + 1 : pos.y * 2 - 1;

            //set the direction, this is a 2d game
            inputDirection = new Vector3(x, y, 0);
            //if the input direction magnitude is hire than 1, normalize it, else don't change it, if a value bigger than 1 is passed the ship may move in wonky directions
            inputDirection = (inputDirection.magnitude > 1) ? inputDirection.normalized : inputDirection;

            //also move the joystick image to match the finger location against the background image
            //while also staying inside the bgImg
            joyImg.rectTransform.anchoredPosition = new Vector3(inputDirection.x * (bgImg.rectTransform.sizeDelta.x / 3), inputDirection.y * (bgImg.rectTransform.sizeDelta.y / 3));
        }
    }

    //function called in the unity inspector, set in the object itself
    //this is when the thumbstick is currently pushed, or used aka when the player puts his finger on the thumbstick
    //event handle activates when the UI element, in this case the joystick image, is pressed
    public virtual void OnPointerDown(PointerEventData ped)
    {
        //Debug.Log("OnPointerDown");
        //update the direction
        OnDrag(ped);
    }

    //function called in the unity inspector, set in the object itself
    //called when the thumbstick is released
    //event handle activates when the UI element, in this case the joystick image, is release, aka removing the finger from it
    public virtual void OnPointerUp(PointerEventData ped)
    {
        //Debug.Log("OnPointerUp");
        //reset the direction
        inputDirection = Vector3.zero;
        //putt he joystick image back in the middle
        joyImg.rectTransform.anchoredPosition = Vector3.zero;
    }
}
