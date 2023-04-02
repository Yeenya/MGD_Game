using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    // Class for every fireball

    private int damage = 50;
    private int liveTime = 5;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(End());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += 30 * Time.deltaTime * transform.forward;
    }
    
    void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Enemy")
        {
            other.gameObject.GetComponent<Enemy>().GetDamage(damage);
            Destroy(this.gameObject);
        }
    }

    IEnumerator End()
    {
        yield return new WaitForSeconds(liveTime);
        Destroy(this.gameObject);
    }
}
