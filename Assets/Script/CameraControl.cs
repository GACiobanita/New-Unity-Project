using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public float minCameraSize;
    public float maxCameraSize;
    public float zoomSpeed = 0.2f;
    Vector3 dragOrigin;
    float dragSpeed = 1.0f;

	// Use this for initialization
	void Start () {
        minCameraSize = Camera.main.orthographicSize;
        maxCameraSize = 2 * minCameraSize;
	}
	
	// Update is called once per frame
	void Update () {
        Zoom();
        MouseDrag();
	}

    void Zoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && (GetComponent<Camera>().orthographicSize - zoomSpeed) > minCameraSize)
        {
            for (int sensitivityOfScrolling = 3; sensitivityOfScrolling > 0; sensitivityOfScrolling--) GetComponent<Camera>().orthographicSize=GetComponent<Camera>().orthographicSize-zoomSpeed;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && (zoomSpeed + GetComponent<Camera>().orthographicSize) < maxCameraSize)
        {
            for (int sensitivityOfScrolling = 3; sensitivityOfScrolling > 0; sensitivityOfScrolling--) GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize + zoomSpeed;
        }
    }

    void MouseDrag()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(1)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0.0f);

        transform.Translate(move, Space.World);
    }

    public void ResetPosition()
    {
        Camera.main.transform.position = new Vector3(0.0f, 0.0f, -10.0f);
    }
}
