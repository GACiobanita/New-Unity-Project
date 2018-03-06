using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is bad there is a better way to do this and it is just for flavour at the moment, too much for too little, we need to look into this
public class ScorePrompt : MonoBehaviour {

    public float waitTime=1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnBecameVisible()
    {
        StartCoroutine(Disappear());
    }

    public IEnumerator Disappear()
    {
        yield return new WaitForSeconds(waitTime);
        this.gameObject.SetActive(false);
    }
}
