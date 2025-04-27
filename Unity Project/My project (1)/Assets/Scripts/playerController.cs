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

    Vector3 moveDir, playerVel, dodgeDir;
    int HPOrig, jumpCount, gunListPos;
    float shootTimer;
    bool isSliding, isDodging, isPlayingSteps;
    int currSpeed;

    KeyCode crouch = KeyCode.C;
    KeyCode prone = KeyCode.LeftControl;
    KeyCode slideKey = KeyCode.V;

    void Start()
    {
        playerCamera = Camera.main.transform;
        HPOrig = HP;
        currSpeed = speed;
        playerLevel = 1;
        shootAnim = transform.Find("Main Camera/Gun Model").GetComponent<Animator>();
        SetStanding();
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

        Movement();
        HandleInputs();
        handleSlide();

        if (Input.GetKeyDown(KeyCode.G)) TryPickup();
        if (Input.GetKeyDown(KeyCode.D)) DropHeldObject();
        if (Input.GetKeyDown(KeyCode.T)) ThrowHeldObject();
    }

    void Movement()
    {
        shootTimer += Time.deltaTime;

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
    }

    void HandleInputs()
    {
        if (Input.GetKeyDown(crouch)) ToggleCrouch();
        if (Input.GetKeyDown(prone)) ToggleProne();
        if (Input.GetButtonDown("Sprint")) currSpeed = speed * sprintMod;
        if (Input.GetButtonUp("Sprint")) currSpeed = speed;
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
            controller.height = crouchingHeight;
            controller.center = crouchingCenter;
            playerCamera.localPosition = crouchingCameraPos;
            currSpeed = crouchingSpeed;
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
        controller.height = standingHeight;
        controller.center = standingCenter;
        playerCamera.localPosition = standingCameraPos;
        currSpeed = speed;
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

    public void getGunStats(GunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;
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
