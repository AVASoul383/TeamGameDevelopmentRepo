using UnityEngine;

public class AutoSnapToTerrain : MonoBehaviour
{
    void Start()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 10f;
        if (Physics.Raycast(origin, Vector3.down, out hit, 100f))
        {
            Debug.Log("Hit ground at: " + hit.point);
            transform.position = hit.point + Vector3.up * 0.1f;
        }
        else
        {
            Debug.LogWarning("No ground detected below player!");
        }
    }
}
