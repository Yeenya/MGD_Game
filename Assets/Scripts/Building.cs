using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{

    public int health;

    // Start is called before the first frame update
    void Start()
    {
        health = 200;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            health = 0;
            if (gameObject.activeSelf)
            {
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}
