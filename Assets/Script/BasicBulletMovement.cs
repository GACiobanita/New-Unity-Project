using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// system should be upgraded more once the basic gameplay elements have been added
public class BasicBulletMovement : MonoBehaviour {

    //add the tag this bullet should hit
    public string target;

    //automatically called once the object is offscreen, or outside of the camera bounds, becoming invisible
    private void OnBecameInvisible()
    {
        //object is deactivated 
        this.gameObject.SetActive(false); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == target)
        {
            this.gameObject.SetActive(false);
        }
    }
}
