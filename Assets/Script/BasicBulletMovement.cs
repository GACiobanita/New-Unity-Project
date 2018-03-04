using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// test pt fratii mei de pe github
public class BasicBulletMovement : MonoBehaviour {

    private Rigidbody2D controller;
    public float moveSpeed;

    //when the object is enable for the first time
    //meaning this is only called once
    //this is different from Awake() as this object can be awake but not be active, e.g. when this actives: object.enabled(true)
    void OnEnable()
    {
        //get the bullet's rigidbody2d component, setting it in the controller variable
        controller = this.transform.GetComponent<Rigidbody2D>();
        //and set its velocity
        controller.velocity = this.transform.right * moveSpeed;
    }

    //automatically called once the object is offscreen, or outside of the camera bounds, becoming invisible
    private void OnBecameInvisible()
    {
        //object is deactivated 
        this.gameObject.SetActive(false); 
    }
}
