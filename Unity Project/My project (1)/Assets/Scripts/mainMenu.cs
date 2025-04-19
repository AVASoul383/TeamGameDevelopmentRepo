using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayGame()
    {
        StartCoroutine(loadGame());
    }

    public void options()
    {
        MenuManager.instance.options();
    }

    public void back()
    {
        MenuManager.instance.main();
    }

    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif   
    }

    IEnumerator loadGame()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Tower Level", LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            Debug.Log($"Tower Level Loading: {asyncLoad.progress}");
            if (asyncLoad.progress == 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
                break;
            }
            yield return null;
        }

        
        Debug.Log("Tower Level Loaded");
    }
}
