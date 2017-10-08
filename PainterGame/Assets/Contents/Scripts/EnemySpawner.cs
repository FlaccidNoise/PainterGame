using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    [SerializeField]
    GameObject baseEnemy;

    [SerializeField]
    Transform floor;

    [SerializeField]
    float spawnRate = 2.0f;
    [SerializeField]
    float minSpawnRate = 0.3f;

    float spawnTimer = 0.0f;

    [SerializeField]
    float rateIncrementer = 0.05f;

    [SerializeField]
    int enemiesToSpawn = 100;

    [SerializeField]
    int spawnedCount = 0;

    [SerializeField]
    bool canSpawn = true;

    void Update()
    {
        if (canSpawn)
        {
            if (spawnTimer > spawnRate && spawnedCount < enemiesToSpawn)
            {
                spawnTimer = 0.0f;
                spawnRate -= rateIncrementer * 1.012f;
                if (spawnRate <= minSpawnRate)
                {
                    spawnRate = minSpawnRate;
                }
                StartCoroutine(SpawnEnemy());
            }
            spawnTimer += Time.deltaTime;
        }
    }

    IEnumerator SpawnEnemy()
    {
        GameObject enemy = Instantiate(baseEnemy, transform.position, transform.rotation) as GameObject;
        spawnedCount++;
        yield return null;
    }

}
