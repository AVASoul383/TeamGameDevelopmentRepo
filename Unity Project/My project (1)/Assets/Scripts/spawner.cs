using UnityEngine;


public class spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] GameObject[] wallsToTrap;
    [SerializeField] GameObject[] wallsToOpen;
    [SerializeField] int numToSpawn;
    [SerializeField] int timeBetweenSpawns;
    [SerializeField] Transform[] spawnPos;


    float spawnTimer;
    int spawnCount;
    bool startSpawning;

    int enemiesAlive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.updateGameGoal(numToSpawn);
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (startSpawning)
        {
            if (spawnCount < numToSpawn && spawnTimer >= timeBetweenSpawns)
            {
                spawn();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
            enemiesAlive = numToSpawn;
            ActivateTrapWalls();
        }
    }

    void spawn()
    {
        int arrayPos = Random.Range(0, spawnPos.Length);
        GameObject newEnemy = Instantiate(objectToSpawn, spawnPos[arrayPos].position, spawnPos[arrayPos].rotation);

        // Connect this spawner to the spawned enemy
        if (newEnemy.TryGetComponent<droneEnemyAI>(out droneEnemyAI enemyAI))
        {
            enemyAI.setSpawner(this);
        }

        spawnCount++;
        spawnTimer = 0;
    }
    public void EnemyDied()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0)
        {
            OpenWalls();
        }
    }
    void ActivateTrapWalls()
    {
        foreach (GameObject wall in wallsToTrap)
        {
            wall.SetActive(true);
        }
    }

    void OpenWalls()
    {
        foreach (GameObject wall in wallsToOpen)
        {
            wall.SetActive(false); // "open" by disabling
        }
    }
}
