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

    async void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(("Player")))
        {
            await FadeIn();
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private async Task FadeIn()
    {
        await fadePanel.DOFade(1, tweenDuration).SetUpdate(true).AsyncWaitForCompletion();
    }
}
