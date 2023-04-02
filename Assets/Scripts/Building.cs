using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{

    public int health;

    void Start()
    {
        health = 200;
    }

    public void GetDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            health = 0;
            if (gameObject.activeSelf) // Make building invisible if destroyed
            {
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}
