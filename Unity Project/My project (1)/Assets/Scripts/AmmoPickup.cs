using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] int ammoAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IPickup pickupable = other.GetComponent<IPickup>();

            if (pickupable != null)
            {
                pickupable.addAmmo(ammoAmount);
                Destroy(gameObject);
            }

            // Optional: Add sound or particle effects here

            Destroy(gameObject); // Remove the pickup after being collected
        }
    }
}
