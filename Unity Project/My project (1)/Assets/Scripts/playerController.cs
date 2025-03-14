using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    [Range(1, 10)][SerializeField] int HP;
    [Range(2, 5)][SerializeField] int speed;
    [Range(2, 8)][SerializeField] int sprintMod;
    [Range(5, 20)][SerializeField] int jumpSpeed;
    [Range(2, 3)][SerializeField] int jumpsMax;
    [Range(15, 45)][SerializeField] int gravity;

    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;

    public int item1Count;
    public int item2Count;
    public int item3Count;
    public int item4Count;
    public int currency;

    int jumpCount;
    int HPOrig;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.green);

        movement();

        sprint();

        if (Input.GetKeyDown("1"))
        {
            UseItem(1);
        }
        else if (Input.GetKeyDown("2"))
        {
            UseItem(2);
        }
        else if (Input.GetKeyDown("3"))
        {
            UseItem(3);
        }
        else if (Input.GetKeyDown("4"))
        {
            UseItem(4);
        }
    }


    void movement()
    {
        shootTimer += Time.deltaTime;

        if(controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        //moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //transform.position += moveDir * speed * Time.deltaTime;

        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
                  (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;



        if(Input.GetButton("Fire1") && shootTimer >= shootRate)
        shoot();
    }

    void jump()
    {
        if(Input.GetButtonDown("Jump") && jumpCount < jumpsMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }

    void sprint()
    {
        if(Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if(Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    void shoot()
    {
        shootTimer = 0;

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if(dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }

            IInteract act = hit.collider.GetComponent<IInteract>();

            if(act != null)
            {
                act.talkTo();
            }
        }
    }
    void UseItem(int slot)
    {
        switch (slot)
        {
            case 1:
                if (item1Count > 0)
                {
                    item1Count--;
                    ActivateItemEffect(1);
                }
                break;
            case 2:
                if (item2Count > 0)
                {
                    item2Count--;
                    ActivateItemEffect(2);
                }
                break;
            case 3:
                if (item3Count > 0)
                {
                    item3Count--;
                    ActivateItemEffect(3);
                }
                break;
            case 4:
                if (item4Count > 0)
                {
                    item4Count--;
                    ActivateItemEffect(4);
                }
                break;
        }
    }
    void ActivateItemEffect(int item)
    {
        switch (item)
        {
            case 1:
                // Heal player
                takeDamage(-10);
                break;
            case 2:
                // Damage buff
                StartCoroutine(damageBoost());
                break;
            case 3:
                // Jump boost
                StartCoroutine(jumpBoost());
                break;
            case 4:
                // Speed boost
                StartCoroutine(speedBoost());
                break;
        }
    }
    IEnumerator damageBoost()
    {
        int origDam = shootDamage;
        shootDamage += 5;
        GameManager.instance.playerDamageBoostScreen.SetActive(true);
        yield return new WaitForSeconds(30f);
        shootDamage = origDam;
    }

    IEnumerator speedBoost()
    {
        int origSpeed = sprintMod;
        sprintMod *= 2;
        GameManager.instance.playerSpeedBoostScreen.SetActive(true);
        yield return new WaitForSeconds(30f);
        sprintMod = origSpeed;
    }
    IEnumerator jumpBoost()
    {
        int origJump = jumpsMax;
        jumpsMax += 5;
        //GameManager.instance.playerJumpBoostScreen.SetActive(true);
        yield return new WaitForSeconds(20f);
        jumpsMax = origJump;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        if (amount > 0)
            StartCoroutine(flashDamageScreen());
        else
            StartCoroutine(flashHealingScreen());

        if (HP <= 0)
        {
            GameManager.instance.youLose();
        }
    }

    IEnumerator flashDamageScreen()
    {
        GameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerDamageScreen.SetActive(false);
    }

    IEnumerator flashHealingScreen()
    {
        GameManager.instance.playerHealthScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerHealthScreen.SetActive(false);
    }

    public void updatePlayerUI()
    {
        GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;

    }

}
