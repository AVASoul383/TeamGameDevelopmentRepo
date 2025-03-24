using UnityEngine;

public class TurnOnOff : MonoBehaviour
{
    [SerializeField] GameObject levelItem;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            levelItem.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            levelItem.SetActive(false);
        }
    }

}
