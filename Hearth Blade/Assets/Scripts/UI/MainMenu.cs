using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject creditsPanel;
    [SerializeField]
    private RectTransform rectTransformCreditsPanel;
    [SerializeField]
    private float topPosY, midPosY;
    [SerializeField]
    private float tweenDuration;

    public void StartButton()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void CreditsButton()
    {
        rectTransformCreditsPanel.anchoredPosition = new Vector2(rectTransformCreditsPanel.anchoredPosition.x, topPosY);
        creditsPanel.SetActive(true);
        IntroCreditsPanel();
    }

    public async void CloseCredits()
    {
        await OutroCreditsPanel();
        creditsPanel.SetActive(false);
    }

    async Task OutroCreditsPanel()
    {
        await rectTransformCreditsPanel.DOAnchorPosY(topPosY, tweenDuration).SetUpdate(true).AsyncWaitForCompletion();
    }

    private void IntroCreditsPanel()
    {
        rectTransformCreditsPanel.DOAnchorPosY(midPosY, tweenDuration).SetUpdate(true);
    }
}
