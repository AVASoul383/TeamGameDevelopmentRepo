using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class playerInteract : MonoBehaviour
{
    public float interactDistance = 3f; 
    public LayerMask interactableLayer;
    public KeyCode interactKey = KeyCode.E;

    private Camera playerCamera;
    void Start()
    {
        playerCamera = Camera.main;
    }
    void Update()
    {
       handleInteraction();
    }

    void handleInteraction()
    {
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactDistance, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactDistance, interactableLayer))
        {
            if (Input.GetKeyDown(interactKey))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }

    public interface IInteractable
    {
        void Interact();
        
    }
}
