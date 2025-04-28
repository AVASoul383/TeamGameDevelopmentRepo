using UnityEngine;

public class NPC : MonoBehaviour, playerInteract.IInteractable
{
    public string npcName;
    public string[] dialogueLines;
    bool hasBeenInteracted = false;
    // This method is called when the player interacts with the NPC
    public void Interact()
    {

        if (!hasBeenInteracted)
        {
            hasBeenInteracted = true;
            GameManager.instance.registerNPCInteraction();
        }
        // Display the dialogue lines in the console or UI
        foreach (string line in dialogueLines)
        {
            if (dialogueLines.Length > 0)
            {
                
                GameManager.instance.showDialogue(dialogueLines[0], npcName);

            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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
