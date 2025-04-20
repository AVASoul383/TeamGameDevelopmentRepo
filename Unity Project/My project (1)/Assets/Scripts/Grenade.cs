using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour
{
    public GrenadeStats stats;
    public Transform playerCamera;
    public float throwForce = 15f;
    public KeyCode pickUpKey = KeyCode.E;
    public KeyCode throwKey = KeyCode.Mouse0;

    private bool isHeld = false;
    private bool playerNearby = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (playerNearby && !isHeld && Input.GetKeyDown(pickUpKey))
        {
            Debug.Log("Picked up grenade");
            PickUp();
        }
        else if (isHeld && Input.GetKeyDown(throwKey))
        {
            Debug.Log("Threw grenade");
            Throw();
        }
    }

    void PickUp()
    {
        isHeld = true;
        rb.isKinematic = true;
        transform.SetParent(playerCamera);
        transform.localPosition = new Vector3(0, -0.5f, 1.5f);
        transform.localRotation = Quaternion.identity;

        QuestManager qm = Object.FindFirstObjectByType<QuestManager>();
        if (qm != null)
        {
            qm.OnGrenadePickedUp();
        }
    }

    void Throw()
    {
        isHeld = false;
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = playerCamera.forward * throwForce;

        StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(stats.fuseTime);

        if (stats.explosionEffect)
            Instantiate(stats.explosionEffect, transform.position, Quaternion.identity);

        if (stats.explosionSound)
            AudioSource.PlayClipAtPoint(stats.explosionSound, transform.position, stats.explosionVolume);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, stats.explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            Debug.Log("Hit " + hit.name + " with grenade");
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            QuestManager qm = Object.FindFirstObjectByType<QuestManager>();
            if (qm != null)
            {
                qm.ShowObjective("Press 'E' to pick up the grenade");
            }

            Debug.Log("Player entered grenade trigger");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            QuestManager qm = Object.FindFirstObjectByType<QuestManager>();
            if (qm != null)
            {
                qm.HideObjective();
            }

            Debug.Log("Player exited grenade trigger");
        }
    }
}
