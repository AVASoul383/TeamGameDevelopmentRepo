using UnityEngine;

public class NPC : MonoBehaviour, playerInteract.IInteractable
{
    public string npcName;
    public string[] dialogueLines;
    // This method is called when the player interacts with the NPC
    public void Interact()
    {
        // Display the dialogue lines in the console or UI
        foreach (string line in dialogueLines)
        {
            if (dialogueLines.Length > 0)
            {
                
                GameManager.instance.showDialogue(dialogueLines[0], npcName);

            }
            Debug.Log(npcName + ": " + line);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is in range of " + npcName);
            GameManager.instance.showInteractionPrompt();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.hideInteractionPrompt();
        }
    }
   
}
