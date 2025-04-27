using System.Collections;
using UnityEngine;

public class Targets : MonoBehaviour, IDamage
{
    enum type {moving, stationary };
    [SerializeField] Renderer model;

    GameObject[] waypoints;

    bool checkMove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }



    public void move()
    {
        
    }

    public void takeDamage(int amount)
    {
        StartCoroutine(flashRed());
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        model.material.color = Color.white;
    }
}
