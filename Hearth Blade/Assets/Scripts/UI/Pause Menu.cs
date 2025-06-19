using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseButton;


    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private RectTransform pausePanelRect;
    [SerializeField] private float topPosY, middlePosY;
    [SerializeField] private float tweenDuration;
    [SerializeField] private CanvasGroup darkPanel;


    public void PauseGame()
    {
        Time.timeScale = 0;
        darkPanel.DOFade(1, tweenDuration).SetUpdate(true);
        pausePanelRect.anchoredPosition = new Vector2(pausePanelRect.anchoredPosition.x, topPosY);
        pauseMenuPanel.SetActive(true);
        PausePanelIntro();
    }

    public async void ResumeGame()
    {
        Time.timeScale = 1;
        darkPanel.DOFade(0, tweenDuration).SetUpdate(true);
        await PausePanelOutro();
        pauseMenuPanel.SetActive(false);

    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    private void PausePanelIntro()
    {
        pausePanelRect.DOAnchorPosY(middlePosY, tweenDuration).SetUpdate(true);
    }

    async Task PausePanelOutro()
    {
        await pausePanelRect.DOAnchorPosY(topPosY, tweenDuration).SetUpdate(true).AsyncWaitForCompletion();
    }
}
