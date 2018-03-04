using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    public Vector3 direction;
    private Rigidbody2D controller;

    private void Awake()
    {
        
    }

    // Use this for initialization
    void Start ()
    {
        controller = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
