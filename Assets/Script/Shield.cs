using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

    public float armor = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="EnemyBullet")
        {
            armor--;
            CheckArmor();
        }
    }

    public void SetArmor(float hits)
    {
        armor = hits;
    }

    void CheckArmor()
    {
        if(armor<=0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
