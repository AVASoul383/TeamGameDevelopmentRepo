using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class merchantAI : MonoBehaviour, IDamage, IInteract
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;

    [SerializeField] GameObject menuShop;
    [SerializeField] int item1Price;
    [SerializeField] int item2Price;
    [SerializeField] int item3Price;
    [SerializeField] int item4Price;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip menuOpenSound;

    Vector3 playerDir;

    bool playerInRange;

    
    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {

            playerDir = GameManager.instance.player.transform.position - transform.position;

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }
        }
    }

    public void talkTo()
    {
        if(playerInRange)
        {

            if (audioSource != null && menuOpenSound != null)
            {
                audioSource.PlayOneShot(menuOpenSound);
            }

            GameManager.instance.statePause();
            GameManager.instance.setActiveMenu(menuShop);
        }
    }

    public void purchaseItem1()
    {
        if(GameManager.instance.playerScript.currency >= item1Price)
        {
            GameManager.instance.playerScript.currency -= item1Price;
            GameManager.instance.updateMoneyUI();
            grantItem(1);
        }
    }
    public void purchaseItem2()
    {
        if (GameManager.instance.playerScript.currency >= item2Price)
        {
            GameManager.instance.playerScript.currency -= item2Price;
            GameManager.instance.updateMoneyUI();
            grantItem(2);
        }
    }
    public void purchaseItem3()
    {
        if (GameManager.instance.playerScript.currency >= item3Price)
        {
            GameManager.instance.playerScript.currency -= item3Price;
            GameManager.instance.updateMoneyUI();
            grantItem(3);
        }
    }
    public void purchaseItem4()
    {
        if (GameManager.instance.playerScript.currency >= item4Price)
        {
            GameManager.instance.playerScript.currency -= item4Price;
            GameManager.instance.updateMoneyUI();
            grantItem(4);
        }
    }

    public void grantItem(int keyPosition)
    {
        switch(keyPosition)
        {
            case 1:
                // Health Boost
                GameManager.instance.playerScript.item1Count++;
                GameManager.instance.updateItemCount1();
                break;
            case 2:
                // Damage Boost
                GameManager.instance.playerScript.item2Count++;
                GameManager.instance.updateItemCount2();
                break;
            case 3:
                // Jump boost
                GameManager.instance.playerScript.item3Count++;
                GameManager.instance.updateItemCount3();
                break;
            case 4:
                // Speed buff
                GameManager.instance.playerScript.item4Count++;
                GameManager.instance.updateItemCount4();
                break;
        }
        
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.14f);
        model.material.color = Color.white;
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
        }
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
}
