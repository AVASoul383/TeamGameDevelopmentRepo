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
        MenuManager.instance.setActiveMenu(null);
        //SceneManager.LoadScene("Credits");
        yield return new WaitForSecondsRealtime(45f);
        LoadNextScene();
    }
}
