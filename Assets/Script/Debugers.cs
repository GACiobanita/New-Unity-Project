using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debugers : MonoBehaviour
{

    public List<InputField> inputs;
    public Dropdown dropdown;
    public Dropdown deadzone;

    public void Activate()
    {
        if (inputs[0].gameObject.activeSelf == false)
        {
            foreach (InputField item in inputs)
            {
                item.gameObject.SetActive(true);
            }
            dropdown.gameObject.SetActive(true);
            deadzone.gameObject.SetActive(true);
        }
        else
        {
            foreach (InputField item in inputs)
            {
                item.gameObject.SetActive(false);
            }
            dropdown.gameObject.SetActive(false);
            deadzone.gameObject.SetActive(false);
        }
    }

    public void SetMaxSpeed()
    {
        ShipControl.sharedInstance.maxSpeed = float.Parse(inputs[0].text);
    }

    public void SetStartSpeed()
    {
        ShipControl.sharedInstance.startSpeed = float.Parse(inputs[1].text);
    }

    public void SetSpeedIncrement()
    {
        ShipControl.sharedInstance.speedIncrement = float.Parse(inputs[2].text);
    }

    public void SetAccelerationInterval()
    {
        ShipControl.sharedInstance.accelerationInterval = float.Parse(inputs[3].text);
    }

    public void SetDeceleration()
    {
        if(dropdown.value==0)
        {
            ShipControl.sharedInstance.decelerate = true;
        }
        else
        {
            ShipControl.sharedInstance.decelerate = false;
        }
    }

    public void RemoveDeadZone()
    {
        GameObject joystick = GameObject.FindGameObjectWithTag("MovementController");
        if (deadzone.value == 0)
        {
            joystick.GetComponent<VirtualJoystick>().deadzoneActive = true; 
        }
        else
        {
            joystick.GetComponent<VirtualJoystick>().deadzoneActive = false;
        }
    }
}
