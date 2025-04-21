using UnityEngine;

public class CarryableObject : MonoBehaviour, ICarryable
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnPickUp(Transform holdParent)
    {
        transform.SetParent(holdParent);
        transform.localPosition = new Vector3(0, -0.3f, 1.2f);
        transform.localRotation = Quaternion.identity;

        rb.useGravity = false;
        rb.isKinematic = true;
    }

    public void OnDrop()
    {
        transform.SetParent(null);
        rb.useGravity = true;
        rb.isKinematic = false;
    }
}
