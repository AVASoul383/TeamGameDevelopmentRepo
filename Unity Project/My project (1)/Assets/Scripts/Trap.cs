using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour
{
    [SerializeField] int damageAmount = 10; 
    [SerializeField] float damageDuration = 1f; 

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Enemy"))
        {
            IDamage enemy = other.GetComponent<IDamage>();

            if (enemy != null)
            {
              
                enemy.takeDamage(damageAmount);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
        if (other.CompareTag("Enemy"))
        {
            IDamage enemy = other.GetComponent<IDamage>();

            if (enemy != null)
            {
               
                StartCoroutine(DamageOverTime(enemy));
            }
        }
    }

    private IEnumerator DamageOverTime(IDamage enemy)
    {
     
        float elapsedTime = 0f;
        while (elapsedTime < damageDuration)
        {
            enemy.takeDamage(damageAmount);
            elapsedTime += damageDuration;
            yield return new WaitForSeconds(damageDuration);
        }
    }
}
