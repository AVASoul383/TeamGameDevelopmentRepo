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
            Debug.Log(npcName + ": " + line);
        }
    }
    // This method is called when the script instance is being loaded
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
