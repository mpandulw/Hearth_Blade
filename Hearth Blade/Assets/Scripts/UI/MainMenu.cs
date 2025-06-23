using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Credits Panel Animation")]
    [SerializeField]
    private GameObject creditsPanel;
    [SerializeField]
    private RectTransform rectTransformCreditsPanel;
    [SerializeField]
    private float topPosY, midPosY;
    [SerializeField]
    private float tweenDuration;

    [Header("Buttons Animation")]
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private RectTransform[] buttonRectTransforms;
    [SerializeField] private float leftPosX, rightPosX;

    void Start()
    {
        Debug.Log("Start");
        for (int i = 0; i < buttons.Length; i++)
        {
            // buttons[i].SetActive(false);
        }
        StartCoroutine(ButtonsIntro());
        Debug.Log("Buttons Length :" + buttons.Length);
    }

    void OnDisable()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttonRectTransforms[i].DOKill();
        }
    }


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

    private IEnumerator ButtonsIntro()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(true);
            buttonRectTransforms[i].anchoredPosition = new Vector2(rightPosX, buttonRectTransforms[i].anchoredPosition.y);

            float duration = 0.5f + 0.1f * i;
            buttonRectTransforms[i].DOAnchorPosX(leftPosX, duration).SetUpdate(true);
        }
    }

    async Task ButtonsOutro()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            float duration = 0.5f + 0.1f * i;
            await buttonRectTransforms[i].DOAnchorPosX(rightPosX, duration).SetUpdate(true).AsyncWaitForCompletion();
            buttons[i].SetActive(false);
        }
    }
}
