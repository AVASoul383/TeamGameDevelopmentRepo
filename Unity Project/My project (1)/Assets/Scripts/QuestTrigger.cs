using UnityEngine;
using TMPro;

public class QuestTrigger : MonoBehaviour
{
    [SerializeField] private TMP_Text questText;
    [SerializeField] private Canvas questCanvas;

    private void Start()
    {
        questCanvas.enabled = false; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            questCanvas.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            questCanvas.enabled = false; 
        }
    }
}
