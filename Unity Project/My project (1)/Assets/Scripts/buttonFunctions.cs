using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        GameManager.instance.stateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.stateUnpause();
    }

    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        if(SceneManager.GetActiveScene().name == "Main Menu" && Application.platform != RuntimePlatform.WebGLPlayer)
            Application.Quit();
        else
            SceneManager.LoadScene("Main Menu");
#endif
    }

    public void progress()
    {
        GameManager.instance.stateUnpause();
    }
public void PlayGame()
    {
        
        MenuManager.instance.setActiveMenu(null);
        //StartCoroutine(loadGame());
        SceneManager.LoadScene("Town_New");

    }

    public void options()
    {
        MenuManager.instance.options();
    }
    public void back()
    {
        MenuManager.instance.prevMenuCall();
    }

    public void creditsRoll()
    {
        MenuManager.instance.CreditsOn();
    }

    IEnumerator loadGame()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Mining Colony", LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if(asyncLoad.progress >= 0.9f)
            {
                MenuManager.instance.promptInput();

                if (Input.GetButtonDown("Submit"))
                {
                    break;
                }
            }
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
    }
}
