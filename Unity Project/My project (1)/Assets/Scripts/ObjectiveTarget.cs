using System.Collections;
using UnityEngine;

public class ObjectiveTarget : MonoBehaviour, IDamage
{
    [SerializeField] int HP = 10;
    [SerializeField] Renderer model;
    [SerializeField] GameObject[] wallsToOpen; 
    void Start()
    {
        
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            OpenWalls();
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.14f);
        model.material.color = Color.white;
    }

    void OpenWalls()
    {
        foreach (GameObject wall in wallsToOpen)
        {
            wall.SetActive(false);
        }
    }
}
