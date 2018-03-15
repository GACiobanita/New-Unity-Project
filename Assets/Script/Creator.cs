using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creator : MonoBehaviour {

    public InputField levelSizeInput;
    float levelSize;
    List<GameObject> bgTiles;
    float tileOffset;
    Vector3 bottomLeftCorner;
    Vector3 bottomRightCorner;
    Vector3 topLeftCorner;
    Vector3 topRightCorner;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

    public void SetLevelSize()
    {
        levelSize = int.Parse(levelSizeInput.text);
        bottomLeftCorner = Camera.main.ScreenToWorldPoint(new Vector2(0.16f * Screen.width, 0.0f));
        Debug.Log(Camera.main.ScreenToWorldPoint(new Vector2(0.16f * Screen.width, 0.0f)));
        bottomLeftCorner = new Vector3(bottomLeftCorner.x, bottomLeftCorner.y);
        bottomRightCorner = Camera.main.ScreenToWorldPoint(new Vector2(0.16f * Screen.width + 0.66f * Screen.width, 0.0f));
        bottomRightCorner = new Vector3(bottomRightCorner.x, bottomRightCorner.y);
        topLeftCorner = Camera.main.ScreenToWorldPoint(new Vector2(0.16f * Screen.width, levelSize));
        topLeftCorner = new Vector3(topLeftCorner.x, topLeftCorner.y);
        topRightCorner = Camera.main.ScreenToWorldPoint(new Vector2(0.16f * Screen.width + 0.66f * Screen.width, levelSize));
        topRightCorner = new Vector3(topRightCorner.x, topRightCorner.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(bottomLeftCorner, bottomRightCorner);
        Gizmos.DrawLine(bottomRightCorner, topRightCorner);
        Gizmos.DrawLine(topRightCorner, topLeftCorner);
        Gizmos.DrawLine(topLeftCorner, bottomLeftCorner);
    }
}
