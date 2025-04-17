using UnityEngine;

public class TurnOnOff : MonoBehaviour
{
    public GameObject[] levelItem;

    

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            for (int i = 0; i < levelItem.Length; i++)
            {
                levelItem[i].SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < levelItem.Length; i++)
            {
                levelItem[i].SetActive(false);
            }
        }
    }

}
