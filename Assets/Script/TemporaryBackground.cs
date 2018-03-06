using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryBackground : MonoBehaviour {

    bool move = true;

    private void Awake()
    {

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(move)
            Move();
	}

    void Move()
    {
        Vector3 movement = Vector3.down * Time.deltaTime;
        //check if the future position is within bounds
        this.GetComponent<Rigidbody2D>().MovePosition(this.GetComponent<Rigidbody2D>().transform.position + movement);
    }

    private void OnBecameInvisible()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        move = false;
        float value = Random.Range(1, 3);
        this.transform.localScale=new Vector3(value, value, 0.0f);
        value = Random.Range(-2, 2);
        this.transform.position = new Vector3(value, 2.0f, 0.0f);
        StartCoroutine(TurnOn());
    }

    IEnumerator TurnOn()
    {
        yield return new WaitForSeconds(1.0f);
        this.GetComponent<SpriteRenderer>().enabled = true;
        move = true;
    }
}
