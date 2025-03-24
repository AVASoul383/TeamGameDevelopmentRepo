using System.Collections;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] Renderer model;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.playerSpawnPos.transform.position != transform.position)
        {
            GameManager.instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(checkpointFeedback());
        }
    }

    IEnumerator checkpointFeedback()
    {
        model.material.color = Color.red;
        GameManager.instance.checkpointPopup.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        model.material.color = Color.white;
        GameManager.instance.checkpointPopup.SetActive(false);
    }

}
