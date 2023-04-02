using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();

    public int numberToSpawn = 1;

    public int spawnCooldown = 10;

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
