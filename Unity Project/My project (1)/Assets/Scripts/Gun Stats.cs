using UnityEngine;

[CreateAssetMenu]
public class GunStats : ScriptableObject
{
    public GameObject model;
    [Range(1, 10)] public int shootDamage;
    [Range(5, 1000)] public int shootDis;
    [Range(0.1f, 2f)] public float shootRate;

    public GameObject bullet;
    public GameObject lastBullet;

    public int ammoReserve;
    [HideInInspector] public int ammoCur;
    [Range(5, 50)] public int ammoMax;

    public float reloadTimer;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    [Range(0, 1)] public float shootVol;
}
