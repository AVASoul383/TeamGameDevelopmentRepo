using UnityEngine;

public class BoxPickup : MonoBehaviour
{
    public QuestManager questManager;
    public Transform holdParent;
    public float throwForce = 500f;

    private bool playerInRange = false;
    private CarryableObject carryable;
    private bool isCarrying = false;

    private void Start()
    {
        carryable = GetComponent<CarryableObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCarrying)
        {
            playerInRange = true;
            questManager.ShowPressGPrompt(); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isCarrying)
        {
            playerInRange = false;
            questManager.ClearText();
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.G) && !isCarrying)
        {
            if (carryable != null)
            {
                carryable.OnPickUp(holdParent);
                questManager.questText.text = "<color=#888888><i>Press D to drop or T to throw</i></color>";
                isCarrying = true;
            }
        }

        if (isCarrying)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                Drop();
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                Throw();
            }
        }
    }

    private void Drop()
    {
        carryable.OnDrop();
        isCarrying = false;
        questManager.ClearText();
    }

    private void Throw()
    {
        carryable.OnDrop();
        Rigidbody rb = carryable.GetComponent<Rigidbody>();
        rb.AddForce(holdParent.forward * throwForce);
        isCarrying = false;
        questManager.ClearText();
    }
}
