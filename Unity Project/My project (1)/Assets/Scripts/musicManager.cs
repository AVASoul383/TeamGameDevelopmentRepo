using UnityEngine;

public class musicManager : MonoBehaviour
{
    public static musicManager instance;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip menuMusic;
    [Range(0, 1)][SerializeField] float menuVol;
    [SerializeField] AudioClip gameplayMusic;
    [Range(0, 1)][SerializeField] float gameplayVol;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    public void playMenuMusic()
    {
        if (audioSource.clip != menuMusic)
        {
            audioSource.clip = menuMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void playGameplayMusic()
    {
        if (audioSource.clip != gameplayMusic)
        {
            audioSource.clip = gameplayMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
