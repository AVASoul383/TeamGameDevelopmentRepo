using UnityEngine;

[System.Serializable]
public struct SpawnData
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
}
public class spawner : MonoBehaviour
{
    [SerializeField] SpawnData[] spawnGroups;
    [SerializeField] GameObject[] wallsToTrap;
    [SerializeField] GameObject[] wallsToOpen;

    bool hasSpawned;

    int enemiesAlive;

    void Start()
    {
        int totalEnemies = 0;
        foreach (var group in spawnGroups)
        {
            totalEnemies += group.spawnPoints.Length;
        }

        enemiesAlive = totalEnemies;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasSpawned)
        {
            hasSpawned = true;
            ActivateTrapWalls();
            SpawnAllEnemies();
        }
    }

    void SpawnAllEnemies()
    {
        foreach (var group in spawnGroups)
        {
            foreach (Transform spawnPoint in group.spawnPoints)
            {
                GameObject newEnemy = Instantiate(group.enemyPrefab, spawnPoint.position, spawnPoint.rotation);

                if (newEnemy.TryGetComponent<droneEnemyAI>(out droneEnemyAI enemyAI))
                {
                    enemyAI.setSpawner(this);
                }
            }
        }
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
            wall.SetActive(false);
        }
    }
}