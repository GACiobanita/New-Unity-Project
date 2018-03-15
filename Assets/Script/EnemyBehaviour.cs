using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{

    private Rigidbody2D controller;
    public Vector3 direction;
    public float moveSpeed = 5.0f;

    private void Awake()
    {

    }

    // Use this for initialization
    void Start()
    {
        direction = Vector3.down;
        controller = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveShip();
    }

    void MoveShip()
    {
        Vector3 movement = direction * moveSpeed * Time.deltaTime;
        //check if the future position is within bounds
        controller.MovePosition(controller.transform.position + movement);
    }

    private void OnBecameInvisible()
    {
        //object is deactivated 
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerBullet")
        {
            this.gameObject.SetActive(false);
            collision.gameObject.SetActive(false);
        }
    }
}
