using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        StartCoroutine(BackToMainMenu());
    }

    //void StartCredits()
    //{

    //}


    public void LoadNextScene()
    {
        SceneManager.LoadScene("Main Menu");
    }

    IEnumerator BackToMainMenu()
    {
        
        yield return new WaitForSecondsRealtime(44f);
        LoadNextScene();
    }
}
