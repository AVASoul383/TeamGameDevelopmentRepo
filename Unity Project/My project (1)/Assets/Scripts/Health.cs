using System.Collections;
using UnityEngine;

public class health : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    enum healthType { stationary, overtime}
    [SerializeField] healthType type;
    [SerializeField] Rigidbody rb;

    [Range(-10, -1)][SerializeField] int healingAmount;
    [Range(0.25f, 1)][SerializeField] float healingTime;
    [Range(10, 45)][SerializeField] int speed;
    [Range(1, 4)][SerializeField] int destroyTime;

    bool isHealing;

    public void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage heal = other.GetComponent<IDamage>();

        if (heal != null && type == healthType.stationary)
        {
            heal.takeDamage(healingAmount);
            Destroy(gameObject);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if(other.isTrigger) return;

        IDamage healing = other.GetComponent<IDamage>();

        if(healing != null && type == healthType.overtime)
        {
            if (!isHealing)
                StartCoroutine(HealingOther(healing));
        }
    }

    IEnumerator HealingOther(IDamage h)
    {
        isHealing = true;

        h.takeDamage(healingAmount);
        yield return new WaitForSeconds(healingTime);

        isHealing = false;
    }
}
