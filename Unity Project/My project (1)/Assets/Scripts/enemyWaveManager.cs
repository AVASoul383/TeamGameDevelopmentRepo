using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public class enemyWaveManager : MonoBehaviour
{

    public GameObject[] enemyPrefabs;
    
    public Transform[] spawnPoints;
    public int startingEnemyCount = 6;
    public float spawnDelay = 1f;
    public float waveDelay = 3f;

    List<GameObject> enemySpawn = new List<GameObject>();
    private GameObject currEnemyPrefab;
    private int currWave = 1;
    private int enemiesToSpawn;
    private int enemiesAlive = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateEnemyPrefab();
        enemiesToSpawn = startingEnemyCount;
        enemySpawn = GameManager.instance.getEnemySpawn();
        StartCoroutine(SpawnWave());
    }

    void updateEnemyPrefab()
    {
        int index = Mathf.Clamp(currWave - 1, 0, enemyPrefabs.Length - 1);
        currEnemyPrefab = enemyPrefabs[index];
    }

    IEnumerator SpawnWave()
    {
        //for (int i = 0; i < enemiesToSpawn; i++)
        //{
        //    int spawnPoint = i % spawnPoints.Length;
        //    GameObject enemy = Instantiate(currEnemyPrefab, spawnPoints[spawnPoint].position, spawnPoints[spawnPoint].rotation);
        //    enemiesAlive++;
        //    enemy.GetComponent<enemyAI>().setWaveManager(this);
        //    yield return new WaitForSeconds(spawnDelay);
        //}

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            int enemiesPoint = i % enemySpawn.Count;
            GameObject enemy = Instantiate(currEnemyPrefab, enemySpawn[enemiesPoint].transform.position, enemySpawn[enemiesPoint].transform.rotation);
            enemiesAlive++;
            enemy.GetComponent<enemyAI>().setWaveManager(this);
            yield return new WaitForSeconds(spawnDelay);
        }

        yield return new WaitUntil(() => enemiesAlive <= 0);

        yield return new WaitForSeconds(waveDelay);

        startNextWave();
    }

    public void enemyDefeated()
    {
        enemiesAlive--;
    }

    void startNextWave()
    {
        currWave++;
        updateEnemyPrefab();
        enemiesToSpawn = startingEnemyCount;
        StartCoroutine(SpawnWave());
    }

}
