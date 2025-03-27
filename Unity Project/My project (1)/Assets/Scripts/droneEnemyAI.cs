using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;



public class droneEnemyAI : MonoBehaviour, IDamage
{
    enum enemyType { moving, stationary, boss}
    [SerializeField] enemyType type;

    [Header("----- Model -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;

    [Header("---- Stats ----")]
    [Range(0, 20)][SerializeField] int HP;
    [Range(0, 10)][SerializeField] float speed;
    [Range(0, 10)][SerializeField] int faceTargetSpeed;
    [Range(0, 360)][SerializeField] int FOV;
    [Range(0, 10)][SerializeField] int roamPauseTime;
    [Range(0, 20)][SerializeField] int roamDist;

    [Header("---- Rewards ----")]
    [Range(0, 10)][SerializeField] int Exp;
    [Range(0, 50)][SerializeField] int moneyDropped;

    [Header("---- Combat ----")]
    [SerializeField] Transform[] shootPos;
    [SerializeField] GameObject[] bullet;
    [Range(0, 5)][SerializeField] float shootRate;

    float shootTimer;
    float roamTimer;
    float angleToPlayer;
    float stoppingDist;
    int shootRotation;
    int bulletRotation;

    Vector3 playerDir;
    Vector3 shootDir;
    Vector3 startingPos;

    bool playerInRange;

    public enemyWaveManager waveManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.updateGameGoal(1);
        if (type == enemyType.boss)
            GameManager.instance.bossFight(1);
        if (type == enemyType.moving || type == enemyType.boss)
            agent.speed = speed;
        startingPos = transform.position;
        stoppingDist = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isPaused)
        {

            shootTimer += Time.deltaTime;

            if (agent.remainingDistance < 0.01f)
                roamTimer += Time.deltaTime;
            if (type == enemyType.moving)
            {
                if (playerInRange && !canSeePlayer())
                {
                    checkRoam();

                }
                else if (!playerInRange)
                {
                    checkRoam();
                }
            }
            else if (type == enemyType.stationary) 
            {
                if (playerInRange && !canSeePlayer())
                {

                }
            }
        }
    }

    bool canSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                if(type == enemyType.moving)
                    agent.SetDestination(GameManager.instance.player.transform.position);

                if (shootTimer >= shootRate)
                {
                    shoot();
                }

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }

                agent.stoppingDistance = stoppingDist;

                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;

    }

    void checkRoam()
    {
        if ((roamTimer > roamPauseTime && agent.remainingDistance < 0.01f))
        {
            roam();
        }

    }

    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = UnityEngine.Random.insideUnitSphere * roamDist;
        ranPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
            roam();
        }
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());
        roamTimer = 0;

        if (type == enemyType.moving)
            agent.SetDestination(GameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            Destroy(gameObject);
            enemyDead();
            GameManager.instance.updateMoneyCount(moneyDropped);
            GameManager.instance.updateGameGoal(-1);
            GameManager.instance.playerScript.SetPlayerExp(Exp);
            if (type == enemyType.boss)
                GameManager.instance.bossFight(-1);

        }
    }

    public void enemyDead()
    {
        if (waveManager != null)
        {
            waveManager.enemyDefeated();
        }
    }

    public void setWaveManager(enemyWaveManager manager)
    {
        waveManager = manager;
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.14f);
        model.material.color = Color.white;
    }

    void shoot()
    {
        if (shootRotation < shootPos.Length - 1)
        {
            shootRotation++;
            bulletRotation++;
        }
        else
        {
            shootRotation = 0;
            bulletRotation = 0;
        }

        shootDir = (GameManager.instance.player.transform.position - shootPos[shootRotation].position).normalized;
        shootTimer = 0;
        Instantiate(bullet[bulletRotation], shootPos[shootRotation].position, Quaternion.LookRotation(shootDir));
    }

    public int GetExp() { return Exp; }
}
