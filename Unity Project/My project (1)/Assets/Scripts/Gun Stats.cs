using UnityEngine;

[CreateAssetMenu]

public class GunStats : ScriptableObject
{
    public GameObject model;
    enum enemyType { bullet, raycast }
    [Range(1, 10)] public int shootDamage;
    [Range(5, 1000)] public int shootDis;
    [Range(0.1f, 2f)] public float shootRate;
    [HideInInspector] public int ammoCur;
    [Range(5, 50)] public int ammoMax;
    [Range(0, 50)] public int ammoReserve;
    [Range(.5f, 5f)] public float reloadTimer;

    public GameObject bullet;
    public GameObject lastBullet;
    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public AudioClip[] reloadSuccess;
    public AudioClip[] reloadFail;
    [Range(0, 1)] public float shootVol;
}