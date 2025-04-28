using UnityEngine;

public interface ICarryable
{
    void OnPickUp(Transform holdParent);
    void OnDrop();
}
