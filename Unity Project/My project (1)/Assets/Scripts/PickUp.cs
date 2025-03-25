using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] GunStats gun;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gun.ammoCur = gun.ammoMax;
    }


    private void OnTriggerEnter(Collider other)
    {
        IPickup pickupable = other.GetComponent<IPickup>();

        if(pickupable != null)
        {
            pickupable.getGunStats(gun);
            Destroy(gameObject);
        }
    }

}
