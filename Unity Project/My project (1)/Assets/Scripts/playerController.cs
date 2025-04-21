using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;


public class playerController : MonoBehaviour, IDamage, IPickup
{
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    [Header("----- Stats -----")]
    [Range(0, 40)] public int HP;
    [Range(1, 50)][SerializeField] int ExpMax;
    [Range(2, 20)][SerializeField] int speed;
    [Range(1, 8)][SerializeField] int sprintMod;
    [Range(5, 20)][SerializeField] int jumpSpeed;
    [Range(2, 3)][SerializeField] int jumpsMax;
    [Range(15, 45)][SerializeField] int gravity;
    [Range(0, 10)][SerializeField] int armor;

    [Header("----- Guns -----")]
    [SerializeField] Transform shootPoint;
    [SerializeField] List<GunStats> gunList = new List<GunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] Transform muzzleFlash;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    public Animator shootAnim;
    public Animator reloadAnim;

    [Header("----- Grapple -----")]
    [SerializeField] int grappleSpeed;
    [SerializeField] int grappleDist;
    [SerializeField] LineRenderer grappleLine;

    [Header("---- Audio ----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audReload;
    [Range(0, 1)][SerializeField] float audReloadVol;
    [SerializeField] AudioClip[] audNoReload;
    [Range(0, 1)][SerializeField] float audNoReloadVol;


    public int item1Count;
    public int item2Count;
    public int item3Count;
    public int item4Count;
    public int currency;

    int playerLevel;
    int jumpCount;
    int HPOrig;
    int ExpAmount;

    bool isPlayerBuffed;
    bool isPlayingSteps;
    bool isSprinting;
    bool isReloading;

    int gunListPos;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        spawnPlayer();
        ExpAmount = 0;
        playerLevel = 1;
        updatePlayerUI();
        shootAnim = transform.Find("Main Camera/Gun Model").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isPaused) return;

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.green);

        if(!GameManager.instance.isPaused)
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
            if (moveDir.magnitude > 0.3f && !isPlayingSteps)
                StartCoroutine(playSteps());
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        //moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //transform.position += moveDir * speed * Time.deltaTime;

        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
                  (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();
        isGrappling();
        

        if(Input.GetButton("Fire1") && gunList.Count > 0 && gunList[gunListPos].ammoCur > 0 && shootTimer >= shootRate)
            shoot();

        
        selectGun();
        reloadGun();
    }

    IEnumerator playSteps()
    {
        isPlayingSteps = true;
        aud.PlayOneShot(audSteps[UnityEngine.Random.Range(0, audSteps.Length)]);

        if (!isSprinting)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.3f);
        isPlayingSteps = false;
    }

    void jump()
    {
        if(Input.GetButtonDown("Jump") && jumpCount < jumpsMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
            aud.PlayOneShot(audJump[UnityEngine.Random.Range(0, audJump.Length)], audJumpVol);
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

    void isGrappling()
    {
        if(Input.GetButton("Fire2") && grapple())
        {
            grappleLine.enabled = true;
        }
        else
        {
            controller.Move(playerVel * Time.deltaTime);
            playerVel.y -= gravity * Time.deltaTime;
            grappleLine.enabled = false;
        }
        
    }

    bool grapple()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grappleDist))
        {
            if (hit.collider.CompareTag("Grapple Point"))
            {
                controller.Move((hit.point - transform.position).normalized * grappleSpeed * Time.deltaTime);
                grappleLine.SetPosition(0, transform.position);
                grappleLine.SetPosition(1, hit.point);

                return true;
            }
        }

        
        return false;
    }

    void shoot()
    {
        if (isReloading || gunList[gunListPos].ammoCur <= 0)
            return;

        shootTimer = 0;

        gunList[gunListPos].ammoCur--;
        aud.PlayOneShot(gunList[gunListPos].shootSound[UnityEngine.Random.Range(0, gunList[gunListPos].shootSound.Length)], gunList[gunListPos].shootVol);
        updateGunAmmo();

        StartCoroutine(flashMuzzle());
        shootAnim.SetTrigger("Shoot");
        //Shooting bullets
        Vector3 targetPoint;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // center of screen
        if (Physics.Raycast(ray, out RaycastHit hit, gunList[gunListPos].shootDis, ~ignoreLayer))
        {
            targetPoint = hit.point;
        }
        else
        {
            // nothing hit, shoot far away
            targetPoint = ray.origin + ray.direction * 100f;
        }
        Vector3 shootDir = (targetPoint - shootPoint.position).normalized;
        if (gunList[gunListPos].ammoCur == 0)
            Instantiate(gunList[gunListPos].lastBullet, shootPoint.position, Quaternion.LookRotation(shootDir));
        else
            Instantiate(gunList[gunListPos].bullet, shootPoint.position, Quaternion.LookRotation(shootDir));

        //Raycast Hitting
        //may look into creating different gun types
        /*
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);
            Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);

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
        */

    }
    void UseItem(int slot)
    {
        if(isPlayerBuffed)
        {

        }
        else
        {
            switch (slot)
            {
                case 1:
                    if (item1Count > 0)
                    {
                        item1Count--;
                        ActivateItemEffect(1);
                        GameManager.instance.updateItemCount1();
                    }
                    break;
                case 2:
                    if (item2Count > 0)
                    {
                        item2Count--;
                        ActivateItemEffect(2);
                        GameManager.instance.updateItemCount2();
                    }
                    break;
                case 3:
                    if (item3Count > 0)
                    {
                        item3Count--;
                        ActivateItemEffect(3);
                        GameManager.instance.updateItemCount3();
                    }
                    break;
                case 4:
                    if (item4Count > 0)
                    {
                        item4Count--;
                        ActivateItemEffect(4);
                        GameManager.instance.updateItemCount4();
                    }
                    break;
            }
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
                StartCoroutine(defenseBoost());
                break;
            case 4:
                // Speed boost
                StartCoroutine(speedBoost());
                break;
        }
    }
    IEnumerator damageBoost()
    {
        isPlayerBuffed = true;
        int origDam = shootDamage;
        shootDamage += 2;
        GameManager.instance.playerDamageBoostScreen.SetActive(true);
        yield return new WaitForSeconds(10f);
        GameManager.instance.playerDamageBoostScreen.SetActive(false);
        isPlayerBuffed = false;
        shootDamage = origDam;
    }

    IEnumerator speedBoost()
    {
        isPlayerBuffed = true;
        int origSpeed = sprintMod;
        sprintMod += 2;
        GameManager.instance.playerSpeedBoostScreen.SetActive(true);
        yield return new WaitForSeconds(10f);
        GameManager.instance.playerSpeedBoostScreen.SetActive(false);
        isPlayerBuffed = false;
        sprintMod = origSpeed;
    }
    IEnumerator defenseBoost()
    {
        isPlayerBuffed = true;
        int origArmor = armor;
        armor += 2;
        GameManager.instance.playerDefenseBoostScreen.SetActive(true);
        yield return new WaitForSeconds(10f);
        GameManager.instance.playerDefenseBoostScreen.SetActive(false);
        isPlayerBuffed = false;
        armor = origArmor;
    }

    public void takeDamage(int amount)
    {
        //check if amount is supposed to damage player 
        if (amount > 0)
        {
            int totalDamage = amount - armor;
            if(totalDamage > 0)
                HP -= totalDamage;
            StartCoroutine(flashDamageScreen());
            aud.PlayOneShot(audHurt[UnityEngine.Random.Range(0, audHurt.Length)], audHurtVol);
        }
        else //check if amount is supposed to heal player
        {
            HP -= amount;
            StartCoroutine(flashHealingScreen());
        }
        updatePlayerUI();

        if (HPOrig < HP)
        {
            HP = HPOrig;
        }
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
        GameManager.instance.playerExpBar.fillAmount = (float)ExpAmount / ExpMax;
    }
    public void SetPlayerExp(int amount)
    {
        ExpAmount += amount;

        if(ExpAmount >= ExpMax)
        {
            ExpAmount -= ExpMax;
            ++playerLevel;
        }
            
        updatePlayerUI();
    }

    public void getGunStats(GunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;
        gunList[gunListPos].ammoReserve = gunList[gunListPos].ammoMax;
        updateGunAmmo();
        changeGun();
        
    }

    void selectGun()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            changeGun();
        }
    }

    void changeGun()
    {
        shootDamage = gunList[gunListPos].shootDamage;
        shootDist = gunList[gunListPos].shootDis;
        shootRate = gunList[gunListPos].shootRate;
        updateGunAmmo();

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterials = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterials;
       
    }

    void reloadGun()
    {
        if(Input.GetButtonDown("Reload"))
        {
            if (gunList[gunListPos].ammoReserve > 0 && gunList[gunListPos].ammoCur < gunList[gunListPos].ammoMax && !isReloading)
            {
                //Reload
                StartCoroutine(ReloadCoroutine());
            }
            else
            {
                //play clip for no reload
                //aud.PlayOneShot(audNoReload[UnityEngine.Random.Range(0, audNoReload.Length)], audNoReloadVol);
            }
        }
    }

    IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        Image bar = GameManager.instance.playerReloadBar;
        Image fill = GameManager.instance.playerReloadFillBar;
        fill.gameObject.SetActive(true);
        bar.gameObject.SetActive(true);
        fill.fillAmount = 0;

        //aud.PlayOneShot(audReload[UnityEngine.Random.Range(0, audReload.Length)], audReloadVol);

        float elapsed = 0f;

        while(elapsed < gunList[gunListPos].reloadTimer)
        {
            elapsed += Time.deltaTime;
            fill.fillAmount = Mathf.Clamp01(elapsed/ gunList[gunListPos].reloadTimer);
            yield return null;
        }
        

        int ammoDifference = gunList[gunListPos].ammoMax - gunList[gunListPos].ammoCur;
        if (ammoDifference <= gunList[gunListPos].ammoReserve)
        {
            gunList[gunListPos].ammoReserve -= ammoDifference;
            gunList[gunListPos].ammoCur += ammoDifference;
        }
        else
        {
            gunList[gunListPos].ammoCur += gunList[gunListPos].ammoReserve;
            gunList[gunListPos].ammoReserve = 0;
        }

        fill.fillAmount = 0;
        bar.gameObject.SetActive(false);
        fill.gameObject.SetActive(false);
        updateGunAmmo();
        isReloading = false;
    }

    public void spawnPlayer()
    {
        controller.transform.position = GameManager.instance.playerSpawnPos.transform.position;

        HP = HPOrig;
        updatePlayerUI();
    }

    void updateGunAmmo()
    {
        GameManager.instance.ammoAmt.text = gunList[gunListPos].ammoCur.ToString("F0") + "/ " + gunList[gunListPos].ammoMax.ToString("F0") + "   " + gunList[gunListPos].ammoReserve.ToString("F0");
    }

    IEnumerator flashMuzzle()
    {
        muzzleFlash.localEulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
        muzzleFlash.gameObject.SetActive(true);
        yield return new WaitForSeconds(.05f);
        muzzleFlash.gameObject.SetActive(false);
    }
}
