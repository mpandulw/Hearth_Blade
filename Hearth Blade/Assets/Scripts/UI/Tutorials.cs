using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Tutorials : MonoBehaviour
{
    public CanvasGroup darkPanel;
    public GameObject tutorialObj;
    public float topPosY, midPosY;
    public float tweenDuration;

    [Header("Move Tutorial")]
    [SerializeField]
    private GameObject moveTutorialPanel;
    [SerializeField]
    private RectTransform moveTutorialRectTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke(nameof(showMoveTutorial), 0.05f);
    }

    private void showMoveTutorial()
    {
        Time.timeScale = 0;
        darkPanel.DOFade(1, tweenDuration).SetUpdate(true);
        tutorialObj.SetActive(true);
        moveTutorialRectTransform.anchoredPosition = new Vector2(moveTutorialRectTransform.anchoredPosition.x, topPosY);
        moveTutorialPanel.SetActive(true);
        MoveTutorialIntro();
    }

    public async void CloseMoveTutorial()
    {
        darkPanel.DOFade(0f, tweenDuration).SetUpdate(true);
        await MoveTutorialOutro();
        Time.timeScale = 1f;
        tutorialObj.SetActive(false);
    }

    async Task MoveTutorialOutro()
    {
        await moveTutorialRectTransform.DOAnchorPosY(topPosY, tweenDuration).SetUpdate(true).AsyncWaitForCompletion();
    }

    private void MoveTutorialIntro()
    {
        moveTutorialRectTransform.DOAnchorPosY(midPosY, tweenDuration).SetUpdate(true);
    }
}
