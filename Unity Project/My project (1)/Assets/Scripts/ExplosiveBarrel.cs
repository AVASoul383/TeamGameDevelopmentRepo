using System.Collections;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 5f; 
    [SerializeField] private float explosionForce = 1000f; 
    [SerializeField] private float damage = 50f;
    [SerializeField] private GameObject explosionEffect; 

    private bool hasExploded = false;


    private void Start()
    {
        
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            Debug.LogError("Explosive Barrel is missing a BoxCollider!");
        }
        else
        {
            boxCollider.isTrigger = true; 
        }
    }

  
    private void OnTriggerEnter(Collider other)
    {
        if (hasExploded) return;

     
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy detected within radius! Triggering explosion.");
            Explode();
        }
    }

 
    private void Explode()
    {
        hasExploded = true;

        
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
           
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            
            IDamage enemy = nearbyObject.GetComponent<IDamage>();
            if (enemy != null)
            {
                enemy.takeDamage((int)damage);
            }
        }

      
        Destroy(gameObject);
    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
