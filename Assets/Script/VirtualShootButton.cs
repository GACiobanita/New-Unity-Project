using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class VirtualShootButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public virtual void OnPointerDown(PointerEventData ped)
    {
        ShipControl.sharedInstance.AllowShooting();
    }

    //function called in the unity inspector, set in the object itself
    //called when the thumbstick is released
    //event handle activates when the UI element, in this case the joystick image, is release, aka removing the finger from it
    public virtual void OnPointerUp(PointerEventData ped)
    {
        ShipControl.sharedInstance.DisableShooting();
    }
}
