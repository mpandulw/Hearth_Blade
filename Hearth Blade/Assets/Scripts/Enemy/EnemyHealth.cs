using System.Collections;
using DG.Tweening;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private float currentHealth;
    public float maxHealth;
    private Animator anim;
    private bool isDead = false;

    [SerializeField]
    private bool isBoss;

    [SerializeField]
    private GameObject endPanel;
    [SerializeField]
    private RectTransform endPanelRect;
    [SerializeField]
    private float topPosY, middlePosY;
    [SerializeField]
    private float tweenDuration;
    [SerializeField]
    private CanvasGroup darkPanel;

    public bool getIsDead => isDead;

    private EnemyMovements enemyMovement; // referensi script movement

    void Awake()
    {
        anim = GetComponent<Animator>();
        enemyMovement = GetComponent<EnemyMovements>(); // ambil komponen movement
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log(currentHealth);

        if (enemyMovement != null)
        {
            enemyMovement.enabled = false;
            Invoke(nameof(EnableMovement), 0.2f); // small pause
        }

        // If boss, pause animation instead of triggering hit
        if (isBoss)
        {
            StartCoroutine(PauseAnimatorBriefly());
        }
        else
        {
            anim.SetTrigger("hit");
        }
    }

    private IEnumerator PauseAnimatorBriefly()
    {
        anim.speed = 0f;
        yield return new WaitForSecondsRealtime(0.2f); // Real-time, not affected by Time.timeScale
        anim.speed = 1f;
    }


    private void EnableMovement()
    {
        if (!isDead && enemyMovement != null)
            enemyMovement.enabled = true;
    }

    void Update()
    {
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        anim.SetBool("dead", true);
        isDead = true;

        if (enemyMovement != null)
            enemyMovement.enabled = false;

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.enabled = false;

        if (isBoss)
        {
            StartCoroutine(ShowEndPanel());
        }
        else
        {
            Destroy(gameObject, 2f);
        }
    }

    private IEnumerator ShowEndPanel()
    {
        yield return new WaitForSeconds(2);
        darkPanel.DOFade(1, tweenDuration).SetUpdate(true);
        endPanelRect.anchoredPosition = new Vector2(endPanelRect.anchoredPosition.x, topPosY);
        endPanel.SetActive(true);
        PausePanelIntro();
        BackgroundMusic.instance.PlayWinBGM();
        Time.timeScale = 0;
    }

    private void PausePanelIntro()
    {
        endPanelRect.DOAnchorPosY(middlePosY, tweenDuration).SetUpdate(true);
    }
}
