using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class TutorialWallSlideAndWallJump : MonoBehaviour
{
    public CanvasGroup darkPanel;
    public GameObject tutorialObj;
    public float topPosY, midPosY;
    public float tweenDuration;

    [Header("Tutorial")]
    [SerializeField]
    private GameObject tutorialPanel;
    [SerializeField]
    private RectTransform tutorialRectTransform;

    private void showTutorial()
    {
        Time.timeScale = 0;
        darkPanel.DOFade(1, tweenDuration).SetUpdate(true);
        tutorialObj.SetActive(true);
        tutorialRectTransform.anchoredPosition = new Vector2(tutorialRectTransform.anchoredPosition.x, topPosY);
        tutorialPanel.SetActive(true);
        TutorialIntro();
    }

    public async void CloseTutorial()
    {
        darkPanel.DOFade(0f, tweenDuration).SetUpdate(true);
        await TutorialOutro();
        Time.timeScale = 1f;
        tutorialObj.SetActive(false);
    }

    async Task TutorialOutro()
    {
        await tutorialRectTransform.DOAnchorPosY(topPosY, tweenDuration).SetUpdate(true).AsyncWaitForCompletion();
    }

    private void TutorialIntro()
    {
        tutorialRectTransform.DOAnchorPosY(midPosY, tweenDuration).SetUpdate(true);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            showTutorial();
            gameObject.SetActive(false);
        }
    }
}
