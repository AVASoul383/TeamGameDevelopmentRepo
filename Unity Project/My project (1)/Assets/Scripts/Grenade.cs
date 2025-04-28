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
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isHeld && Vector3.Distance(playerCamera.position, transform.position) <= 3f && Input.GetKeyDown(pickUpKey))
        {
            PickUp();
        }
        else if (isHeld && Input.GetKeyDown(throwKey))
        {
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


    IEnumerator ExplodeAfterDelay()
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
}
