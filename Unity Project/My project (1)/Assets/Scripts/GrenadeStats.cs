using UnityEngine;

[CreateAssetMenu(fileName = "NewGrenade", menuName = "Items/Grenade")]
public class GrenadeStats : ScriptableObject
{
    public GameObject model;
    [Range(10, 200)] public int explosionDamage;
    [Range(1f, 10f)] public float explosionRadius;
    [Range(1f, 5f)] public float fuseTime;

    public GameObject explosionEffect;
    public AudioClip explosionSound;
    [Range(0f, 1f)] public float explosionVolume;
}
