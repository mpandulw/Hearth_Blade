using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic instance;
    private AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keeps music when changing scenes
        }
        else
        {
            Destroy(gameObject); // Avoid duplicate music
            return;
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }
}
