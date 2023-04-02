using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllySpawner : MonoBehaviour
{
    [Tooltip("Cost of ally to be spawned")]
    public int cost = 100;

    [Tooltip("Checks if ally is spawned from this spawner")]
    public bool allySpawned = false;

    private GameObject player;

    [Tooltip("Reference to the building this spawner is near to")]
    public GameObject building;

    private Transform spawnPlace;

    [Tooltip("Prefab for ally to spawn")]
    public GameObject ally;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        spawnPlace = transform.GetChild(0);
    }

    void Update()
    {
        // Make the spawner visible or invisible according to the distance of the player
        if (Vector3.Distance(player.transform.position, transform.position) < 4 && !allySpawned && building.GetComponent<MeshRenderer>().enabled)
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
