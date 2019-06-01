using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowerUp : MonoBehaviour {

    //if the power up has a duration
    public bool expires = true;
    //time the power up will stay on screen before it disappears
    public float objDuration;
    //duration of the actual power up
    public float powerUpDuration;
    //speed of the power up obj as it moves in the game
    public float movementSpeed;
    //power up animations such as flickering
    [HideInInspector]
    public Animate animComp;
    //box collider size of the power up
    [HideInInspector]
    public Vector2 powerUpSizes;
    //diagonal direction
    [HideInInspector]
    public Vector2 diagDir;
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public bool canMove;

    //class constructors
    public PowerUp()
    {
        expires = true;
        objDuration = 0.0f;
        powerUpDuration = 0.0f;
        movementSpeed = 0.0f;
        diagDir = Vector2.zero;
        canMove = false;
    }

    public PowerUp(bool e, float oD, float pD, float mS)
    {
        expires = e;
        objDuration = oD;
        powerUpDuration = pD;
        movementSpeed = mS;
    }

    private void Awake()
    {
        ComponentSetup();
        StartCoroutine(Countdown());
    }

    public void ComponentSetup()
    {
        RandomDiagDirection();
        animComp = this.GetComponent<Animate>();
        rb = this.GetComponent<Rigidbody2D>();
        powerUpSizes = new Vector2(this.GetComponent<BoxCollider2D>().size.x, this.GetComponent<BoxCollider2D>().size.y);
    }

    void FixedUpdate()
    {
        if(canMove)
            Movement();
    }

    public void Movement()
    {
        float speed = movementSpeed * CameraControl.sharedInstance.GetCameraSpeed() * Time.deltaTime;
        Vector3 movement = diagDir*speed;
        if (!CameraControl.sharedInstance.XAxisConstraint(rb.transform.position + movement, powerUpSizes.x))
        {
            diagDir = new Vector2(-diagDir.x, diagDir.y);
        }
        else
        {
            if (!CameraControl.sharedInstance.YAxisConstraint(rb.transform.position+movement, powerUpSizes.y))
                diagDir = new Vector2(diagDir.x, -diagDir.y);
        }
        movement = diagDir * speed;
        rb.MovePosition(rb.transform.position + movement);
        
    }

    public void RandomDiagDirection()
    {
        diagDir =new Vector2(RandomDiagCoordinate(), RandomDiagCoordinate());
    }

    public float RandomDiagCoordinate()
    {
        if (Random.value > 0.5)
            return 1.0f;
        else
            return -1.0f;
    }

    public void AttachToObj(GameObject go)
    {
        this.transform.SetParent(go.transform);
        this.transform.position = go.transform.position;
    }

    public void RemovePowerUp()
    {
        this.gameObject.SetActive(false);
    }

    public IEnumerator Countdown()
    {
        canMove = true;
        yield return new WaitForSeconds(objDuration/2);
        StartCoroutine(animComp.SpriteFlicker(objDuration/2));
        yield return new WaitForSeconds(objDuration / 2);
        RemovePowerUp();
    }
}
