using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Class for an enemy spawner

    [Tooltip("Enemies that can spawn in this spawner")]
    public List<GameObject> enemies = new List<GameObject>();

    [Tooltip("How many enemies spawn each wave (increases over time)")]
    public int numberToSpawn = 1;

    [Tooltip("Time between spawns/waves")]
    public int spawnCooldown = 10;

    [Tooltip("Waves remaining")]
    public int waves = 24;

    void Start()
    {
        StartCoroutine(Cooldown());
    }

    void Spawn()
    {
        numberToSpawn = (int)Time.time / 60 + 1;
        waves--;
        spawnCooldown += 5;
        for (int i = 0; i < numberToSpawn; i++)
        {
            int randomEnemy = Random.Range(0, enemies.Count);

            // Give the enemy a random offset so that they don't all spawn on one place
            Vector3 offset = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));

            GameObject enemy = Instantiate(enemies[randomEnemy], transform.position + offset, transform.rotation);
            enemy.transform.SetParent(transform.parent);
        }
        GameObject.FindGameObjectWithTag("UI").GetComponent<InGameMenuController>().UpdateWave(24 - waves);
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(spawnCooldown);
        if (waves >= 0) Spawn();
        StartCoroutine(Cooldown());
    }

    public void UpdateProperties()
    {

    }
}
