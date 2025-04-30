using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.Rendering;

public class playerController : MonoBehaviour, IDamage, IPickup
{
    [Header("----- References -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] Transform carryPosition;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] LineRenderer grappleLine;
    [SerializeField] AudioSource aud;
    [SerializeField] Transform playerCamera;

    [Header("----- Player Stats -----")]
    public int HP = 10;
    public int item1Count;
    public int item2Count;
    public int item3Count;
    public int item4Count;
    [SerializeField] int speed = 10;
    [SerializeField] int sprintMod = 2;
    [SerializeField] int jumpSpeed = 10;
    [SerializeField] int jumpsMax = 2;
    [SerializeField] int gravity = 10;
    [SerializeField] int armor = 0;
    [SerializeField] int ExpMax = 10;

    int ExpAmount;
    int playerLevel;

    [Header("----- Movement -----")]
    public float standingHeight = 2f;
    public float crouchingHeight = 1.5f;
    public float proneHeight = 0.5f;
    public int proneSpeed = 3;
    public int crouchingSpeed = 5;
    public int slideSpeed = 20;
    public float slideDuration = 0.75f;
    float slideTimer = 0f;
    public Vector3 standingCenter = new Vector3(0, 1, 0);
    public Vector3 crouchingCenter = new Vector3(0, 0.75f, 0);
    public Vector3 proneCenter = new Vector3(0, 0.25f, 0);
    public Vector3 standingCameraPos = new Vector3(0, 1.7f, 0);
    public Vector3 crouchingCameraPos = new Vector3(0, 1.2f, 0);
    public Vector3 proneCameraPos = new Vector3(0, 0.5f, 0);

    [Header("----- Guns -----")]
    [SerializeField] Transform shootPoint;
    [SerializeField] GameObject gunModel;
    [SerializeField] Transform muzzleFlash;
    [SerializeField] List<GunStats> gunList = new List<GunStats>();
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    public Animator shootAnim;
    public Animator reloadAnim;

    [Header("----- Grapple -----")]
    [SerializeField] int grappleSpeed = 20;
    [SerializeField] int grappleDist = 50;

    [Header("----- Audio Clips -----")]
    [SerializeField] AudioClip[] audSteps;
    [SerializeField] AudioClip[] audJump;
    [SerializeField] AudioClip[] audHurt;

    ICarryable heldObject;

    Animator anim;

    Vector3 moveDir, playerVel, dodgeDir;
    int HPOrig, jumpCount, gunListPos;
    float shootTimer;
    bool isSliding, isDodging, isPlayingSteps, isCrouching, isReloading, isPlayerBuffed;
    int currSpeed;

    KeyCode crouch = KeyCode.C;
    KeyCode prone = KeyCode.LeftControl;
    KeyCode slideKey = KeyCode.V;

    void Start()
    {
        playerCamera = Camera.main.transform;
        anim = GetComponentInChildren<Animator>();
        HPOrig = HP;
        currSpeed = speed;
        playerLevel = 1;
        shootAnim = transform.Find("Main Camera/Gun Model").GetComponent<Animator>();
        //SetStanding();
        //StartCoroutine(NudgeToGround());
    }

    IEnumerator NudgeToGround()
    {
        yield return new WaitForEndOfFrame();
        controller.Move(Vector3.down * 0.1f);
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isPaused) return;

        Movement();
        HandleInputs();
        handleSlide();
        anim.SetInteger("Health", HP);  

        if (Input.GetKeyDown(KeyCode.G)) TryPickup();
        if (Input.GetKeyDown(KeyCode.D)) DropHeldObject();
        if (Input.GetKeyDown(KeyCode.T)) ThrowHeldObject();
    }

    void Movement()
    {
        shootTimer += Time.deltaTime;
        anim.SetFloat("Speed", moveDir.magnitude);
        if (Input.GetButtonDown("Jump") && jumpCount < jumpsMax)
        {
            playerVel.y = jumpSpeed;
            anim.SetTrigger("Jump");
        }
        if (controller.isGrounded)
        {
            if (moveDir.magnitude > 0.3f && !isPlayingSteps)
                StartCoroutine(PlaySteps());

            playerVel.y = playerVel.y < 0 ? -1f : playerVel.y;
            jumpCount = 0;
        }
        else
        {
            playerVel.y -= gravity * Time.deltaTime;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        Vector3 finalMove = moveDir * currSpeed;
        finalMove.y = playerVel.y;
        controller.Move(finalMove * Time.deltaTime);

        Jump();

        if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[gunListPos].ammoCur > 0 && shootTimer >= shootRate)
            shoot();

        selectGun();
        reloadGun();
    }

    void HandleInputs()
    {
        if (Input.GetKeyDown(crouch)) ToggleCrouch();
        if (Input.GetKeyDown(prone)) ToggleProne();
        if (Input.GetButtonDown("Sprint")) currSpeed = speed * sprintMod;
        if (Input.GetButtonUp("Sprint")) currSpeed = speed;
    }
    void HandleItemKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItem(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseItem(3);
        if (Input.GetKeyDown(KeyCode.Alpha4)) UseItem(4);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpsMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }

    IEnumerator PlaySteps()
    {
        isPlayingSteps = true;
        if (audSteps.Length > 0) aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)]);
        yield return new WaitForSeconds(0.5f);
        isPlayingSteps = false;
    }

    void ToggleCrouch()
    {
        if (controller.height == crouchingHeight)
            SetStanding();
        else
        {
            isCrouching = true;
            controller.height = crouchingHeight;
            controller.center = crouchingCenter;
            playerCamera.localPosition = crouchingCameraPos;
            currSpeed = crouchingSpeed;
            anim.SetBool("isCrouching", isCrouching);
        }
    }

    void ToggleProne()
    {
        if (controller.height == proneHeight)
            SetStanding();
        else
        {
            controller.height = proneHeight;
            controller.center = proneCenter;
            playerCamera.localPosition = proneCameraPos;
            currSpeed = proneSpeed;
        }
    }

    void SetStanding()
    {
        isCrouching = false;
        controller.height = standingHeight;
        controller.center = standingCenter;
        playerCamera.localPosition = standingCameraPos;
        currSpeed = speed;
        anim.SetBool("isCrouching", isCrouching);
    }

    void startSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;
        currSpeed = slideSpeed;
        controller.height = crouchingHeight;
    }

    void endSlide()
    {
        isSliding = false;
        controller.height = standingHeight;
        currSpeed = speed;
    }

    void handleSlide()
    {
        if(Input.GetKeyDown(slideKey) && controller.isGrounded && moveDir.magnitude > 0.1f)
        {
            startSlide();
        }
        if(isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f) endSlide();
        }
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

    public void takeDamage(int amount)
    {
        HP -= Mathf.Max(amount - armor, 0);
        if (HP <= 0) GameManager.instance.youLose();
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
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);
            Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }

            IInteract act = hit.collider.GetComponent<IInteract>();

            if (act != null)
            {
                act.talkTo();
            }
        }
        */


    }
    public void getGunStats(GunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;
        gunList[gunListPos].ammoReserve = gunList[gunListPos].ammoMax;
        updateGunAmmo();
        changeGun();
    }


    public void SetPlayerExp(int amount)
    {
        ExpAmount += amount;
        if (ExpAmount >= ExpMax)
        {
            ExpAmount -= ExpMax;
            playerLevel++;
        }
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

    public void addAmmo(int amount)
    {
        gunList[gunListPos].ammoReserve += amount;
        updateGunAmmo();
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
