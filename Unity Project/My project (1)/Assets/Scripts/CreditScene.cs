using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(BackToMainMenu());
    }

    IEnumerator BackToMainMenu()
    {
        SceneManager.LoadScene("Credits");
        yield return new WaitForSeconds(45f);
        SceneManager.LoadScene("Main Menu");
    }
}
