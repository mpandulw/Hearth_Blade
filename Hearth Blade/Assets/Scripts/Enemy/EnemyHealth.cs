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

        anim.SetTrigger("hit");
        currentHealth -= damage;
        Debug.Log(currentHealth);

        if (enemyMovement != null)
        {
            enemyMovement.enabled = false; // nonaktifkan movement saat hit
            Invoke(nameof(EnableMovement), 0.5f); // aktifkan kembali setelah 0.5 detik
        }
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
        Time.timeScale = 0;
    }

    private void PausePanelIntro()
    {
        endPanelRect.DOAnchorPosY(middlePosY, tweenDuration).SetUpdate(true);
    }
}
