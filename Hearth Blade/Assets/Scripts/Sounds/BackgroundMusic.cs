using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic instance;
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip mainMenuBGM;
    [SerializeField]
    private AudioClip normalBGM;
    [SerializeField]
    private AudioClip bossBGM;
    [SerializeField]
    private AudioClip winBGM;
    [SerializeField]
    private AudioClip loseBGM;


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
        audioSource.ignoreListenerPause = true;
        // audioSource.playOnAwake = false;

    }

    public void PlayNormalBGM()
    {
        PlayMusic(normalBGM);
    }

    public void PlayMainMenuBGM()
    {
        PlayMusic(mainMenuBGM);
    }

    public void PlayBossBGM()
    {
        PlayMusic(bossBGM);
    }

    public void PlayWinBGM()
    {
        PlayMusic(winBGM, loop: false);
    }

    public void PlayLoseBGM()
    {
        PlayMusic(loseBGM, loop: false);
    }

    private void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (audioSource.clip == clip) return;
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }

}
