using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllySpawner : MonoBehaviour
{
    public int cost = 100;

    public bool allySpawned = false;

    private GameObject player;

    private Transform spawnPlace;

    public GameObject ally;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        spawnPlace = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < 4 && !allySpawned)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponent<MeshCollider>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<MeshCollider>().enabled = false;
        }
    }

    public void SpawnAlly()
    {
        GameObject spawnedAlly = Instantiate(ally, spawnPlace.position, spawnPlace.rotation);
        allySpawned = true;

        spawnedAlly.GetComponent<Ally>().SetSpawnerReference(gameObject);
    }
}
