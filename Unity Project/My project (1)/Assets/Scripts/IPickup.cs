using UnityEngine;

public interface IPickup
{
    void addAmmo(int ammoAmount);
    void getGunStats(GunStats gun);
}
