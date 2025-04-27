using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class playerController : MonoBehaviour, IDamage, IPickup
{
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;
    [SerializeField] Transform carryPosition;

    ICarryable heldObject;

    [Header("----- Stats -----")]
    [Range(0, 40)] public int HP;
    [SerializeField] int ExpMax = 10;
    [SerializeField] int speed = 10;
    [SerializeField] int sprintMod = 2;
    [SerializeField] int jumpSpeed = 10;
    [SerializeField] int jumpsMax = 2;
    [SerializeField] int gravity = 20;
    [SerializeField] int armor = 0;

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
    [SerializeField] int grappleSpeed = 20;
    [SerializeField] int grappleDist = 50;
    [SerializeField] LineRenderer grappleLine;

    [Header("---- Audio ----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audSteps;
    [SerializeField] float audStepsVol = 1;
    [SerializeField] AudioClip[] audJump;
    [SerializeField] float audJumpVol = 1;
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


    public Transform playerCamera;

    int gunListPos;
    float shootTimer;

    [Header("----- Movement States -----")]
    public float standingHeight = 2f, crouchingHeight = 1.5f, proneHeight = 0.5f;
    public bool isCrouching = false, isProne = false;
    public Vector3 standingCenter = new(0, 1, 0), crouchingCenter = new(0, 0.75f, 0), proneCenter = new(0, 0.25f, 0);
    public Vector3 standingCameraPos = new(0, 1.7f, 0), crouchingCameraPos = new(0, 1.2f, 0), proneCameraPos = new(0, 0.5f, 0);

    Vector3 moveDir, playerVel;

    public KeyCode crouch = KeyCode.C, prone = KeyCode.LeftControl;

    void Start()
    {
        HPOrig = HP;
        ExpAmount = 0;
        playerLevel = 1;
        updatePlayerUI();

        shootAnim = transform.Find("Main Camera/Gun Model").GetComponent<Animator>();
        setStanding();
      
        StartCoroutine(NudgeToGround());
    }

    IEnumerator NudgeToGround()
    {
        yield return new WaitForEndOfFrame();
        controller.Move(Vector3.down * 0.1f);
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isPaused) return;

        movement();
        crouchInput();
        proneInput();
        handleCrouchProneMovement();
        sprint();
        HandleItemKeys();

        if (Input.GetKeyDown(KeyCode.G)) TryPickup();
        if (Input.GetKeyDown(KeyCode.D)) DropHeldObject();
        if (Input.GetKeyDown(KeyCode.T)) ThrowHeldObject();
    }

    void movement()
    {
        shootTimer += Time.deltaTime;

        if (controller.isGrounded)
        {
            if (moveDir.magnitude > 0.3f && !isPlayingSteps && audSteps.Length > 0)
                StartCoroutine(playSteps());

            playerVel.y = playerVel.y < 0 ? -1f : playerVel.y;
            jumpCount = 0;
        }
        else
        {
            playerVel.y -= gravity * Time.deltaTime;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        Vector3 finalMove = moveDir * speed;
        finalMove.y = playerVel.y;
        controller.Move(finalMove * Time.deltaTime);

        jump();
        isGrappling();

        if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[gunListPos].ammoCur > 0 && shootTimer >= shootRate)
            shoot();

        selectGun();
        reloadGun();
    }

    void TryPickup()
    {
        if (heldObject != null) return;

        Ray ray = new(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
        {
            if (hit.collider.TryGetComponent(out ICarryable carryable))
            {
                PickUpObject(carryable);
            }
        }
    }

    void HandleItemKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItem(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseItem(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) UseItem(4);
    }

    public void PickUpObject(ICarryable obj)
    {
        heldObject = obj;
        obj.OnPickUp(carryPosition);
    }

    void DropHeldObject()
    {
        if (heldObject == null) return;
        heldObject.OnDrop();
        heldObject = null;
    }

    void ThrowHeldObject()
    {
        if (heldObject == null) return;

        heldObject.OnDrop();
        Rigidbody rb = ((MonoBehaviour)heldObject).GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(playerCamera.forward * 10f, ForceMode.Impulse);
        }
        heldObject = null;
    }

    void crouchInput()
    {
        if (!Input.GetKeyDown(crouch)) return;

        if (isCrouching) setStanding();
        else
        {
            isProne = false;
            controller.height = crouchingHeight;
            controller.center = crouchingCenter;
            playerCamera.localPosition = crouchingCameraPos;
            isCrouching = true;
        }
    }

    void proneInput()
    {
        if (!Input.GetKeyDown(prone)) return;

        if (isProne) setStanding();
        else
        {
            isCrouching = false;
            controller.height = proneHeight;
            controller.center = proneCenter;
            playerCamera.localPosition = proneCameraPos;
            isProne = true;
        }
    }

    void setStanding()
    {
        controller.height = standingHeight;
        controller.center = standingCenter;
        playerCamera.localPosition = standingCameraPos;
        isCrouching = false;
        isProne = false;
    }

    void handleCrouchProneMovement()
    {
        speed = isCrouching ? 5 : isProne ? 2 : 10;
    }

    IEnumerator playSteps()
    {
        isPlayingSteps = true;
        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);
        yield return new WaitForSeconds(isSprinting ? 0.3f : 0.5f);
        isPlayingSteps = false;
    }

    void jump()
    {
        if (!Input.GetButtonDown("Jump") || jumpCount >= jumpsMax) return;

        jumpCount++;
        playerVel.y = jumpSpeed;

        if (audJump.Length > 0)
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint")) speed *= sprintMod;
        else if (Input.GetButtonUp("Sprint")) speed /= sprintMod;
    }

    void isGrappling()
    {
        if (Input.GetButton("Fire2") && grapple()) grappleLine.enabled = true;
        else
        {
            controller.Move(playerVel * Time.deltaTime);
            playerVel.y -= gravity * Time.deltaTime;
            grappleLine.enabled = false;
        }
    }

    bool grapple()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, grappleDist))
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
        if (gunList[gunListPos].shootSound.Length > 0)
            aud.PlayOneShot(gunList[gunListPos].shootSound[Random.Range(0, gunList[gunListPos].shootSound.Length)], gunList[gunListPos].shootVol);

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
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, shootDist, ~ignoreLayer))

        {
            Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);
            hit.collider.GetComponent<IDamage>()?.takeDamage(shootDamage);
            hit.collider.GetComponent<IInteract>()?.talkTo();
        }

        */

    }

    void UseItem(int slot)
    {
        if (isPlayerBuffed) return;

        switch (slot)
        {
            case 1: if (item1Count-- > 0) ActivateItemEffect(1); break;
            case 2: if (item2Count-- > 0) ActivateItemEffect(2); break;
            case 3: if (item3Count-- > 0) ActivateItemEffect(3); break;
            case 4: if (item4Count-- > 0) ActivateItemEffect(4); break;
        }
    }

    void ActivateItemEffect(int item)
    {
        switch (item)
        {
            case 1: takeDamage(-10); break;
            case 2: StartCoroutine(damageBoost()); break;
            case 3: StartCoroutine(defenseBoost()); break;
            case 4: StartCoroutine(speedBoost()); break;
        }
    }

    IEnumerator damageBoost()
    {
        isPlayerBuffed = true;
        int orig = shootDamage;
        shootDamage += 2;
        GameManager.instance.playerDamageBoostScreen.SetActive(true);
        yield return new WaitForSeconds(10f);
        GameManager.instance.playerDamageBoostScreen.SetActive(false);
        shootDamage = orig;
        isPlayerBuffed = false;
    }

    IEnumerator speedBoost()
    {
        isPlayerBuffed = true;
        int orig = sprintMod;
        sprintMod += 2;
        GameManager.instance.playerSpeedBoostScreen.SetActive(true);
        yield return new WaitForSeconds(10f);
        GameManager.instance.playerSpeedBoostScreen.SetActive(false);
        sprintMod = orig;
        isPlayerBuffed = false;
    }

    IEnumerator defenseBoost()
    {
        isPlayerBuffed = true;
        int orig = armor;
        armor += 2;
        GameManager.instance.playerDefenseBoostScreen.SetActive(true);
        yield return new WaitForSeconds(10f);
        GameManager.instance.playerDefenseBoostScreen.SetActive(false);
        armor = orig;
        isPlayerBuffed = false;
    }

    public void takeDamage(int amount)
    {
        if (amount > 0)
        {
            int total = amount - armor;
            if (total > 0) HP -= (HP <= 5) ? total / 2 : total;
            StartCoroutine(flashDamageScreen());
            if (audHurt.Length > 0)
                aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
        }
        else
        {
            HP -= amount;
            StartCoroutine(flashHealingScreen());
        }

        updatePlayerUI();
        if (HP > HPOrig) HP = HPOrig;
        if (HP <= 0) GameManager.instance.youLose();
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
        if (GameManager.instance == null) return;
        GameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
        GameManager.instance.playerExpBar.fillAmount = (float)ExpAmount / ExpMax;
    }

    public void SetPlayerExp(int amount)
    {
        ExpAmount += amount;
        if (ExpAmount >= ExpMax)
        {
            ExpAmount -= ExpMax;
            playerLevel++;
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
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1) { gunListPos++; changeGun(); }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0) { gunListPos--; changeGun(); }
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
        if (Input.GetButtonDown("Reload"))
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

        while (elapsed < gunList[gunListPos].reloadTimer)
        {
            elapsed += Time.deltaTime;
            fill.fillAmount = Mathf.Clamp01(elapsed / gunList[gunListPos].reloadTimer);
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

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;
        if (rb != null && !rb.isKinematic)
        {
            Vector3 pushDir = new(hit.moveDirection.x, 0, hit.moveDirection.z);
            rb.AddForce(pushDir * 5f, ForceMode.Impulse);
        }
    }
}
