using UnityEngine;

//githubtest
//scale the camera view with the screen of the hardware
public class CameraPixelDensity : MonoBehaviour {

    //default values
    //how many pixels in a unity unit?
    public float pixelToUnits = 100;
    //what is the target width resolution
    public float targetWidth = 640;

	// Update is called once per frame
	void Awake () {
        //scale the camera size by the targetWidth using the current hardware screen
        int height = Mathf.RoundToInt(targetWidth / (float)Screen.width * Screen.height);
        GetComponent<Camera>().orthographicSize = height / pixelToUnits / 2;
        //this explains the procedure, https://www.youtube.com/watch?v=eIHqe8opFoU

        //create the game area bounds
        GameArea.sharedInstance.CreateArenaBoundaries();
    }
}
