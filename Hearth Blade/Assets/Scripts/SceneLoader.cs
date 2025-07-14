using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad;
    public CanvasGroup fadePanel;
    public float tweenDuration;
    public bool isBack;

    [SerializeField]
    private GameObject player;

    async void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            await FadeIn();

            if (isBack)
            {
                PlayerPrefs.SetString("PlayerSpawnPoint", "SpawnPoint_Exit");
            }
            else
            {
                PlayerPrefs.SetString("PlayerSpawnPoint", "SpawnPoint_Entry");
            }
            PlayerPrefs.SetFloat("playerHealthPoint", player.GetComponent<PlayerHealth>().currentHealth);

            PlayerPrefs.Save();

            SceneManager.LoadScene(sceneToLoad);
        }
    }


    private async Task FadeIn()
    {
        await fadePanel.DOFade(1, tweenDuration).SetUpdate(true).AsyncWaitForCompletion();
    }
}
