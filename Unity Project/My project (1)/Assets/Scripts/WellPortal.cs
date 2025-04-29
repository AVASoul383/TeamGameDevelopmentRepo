using UnityEngine;
using UnityEngine.SceneManagement;

public class WellPortal : MonoBehaviour
{
    [SerializeField] string sceneToLoad = "Mining Colony";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
