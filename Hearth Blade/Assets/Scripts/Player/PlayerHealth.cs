using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private PlayerMovements playerMovements;
    private Animator anim;

    [Header("Health")]
    public float currentHealth;
    public float maxHealth;
    public Slider hpBar;

    [Header("Dead Panel")]
    [SerializeField] private GameObject deadPanelGameObject;
    [SerializeField] private CanvasGroup deadPanel;
    [SerializeField] private float tweenDuration;


    private float currentVelocity = 0;

    void Awake()
    {
        playerMovements = GetComponent<PlayerMovements>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        hpBar.maxValue = maxHealth;
        hpBar.value = currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.value = Mathf.SmoothDamp(hpBar.value, currentHealth, ref currentVelocity, 0.1f);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        anim.SetTrigger("isHit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        deadPanelGameObject.SetActive(true);
        deadPanel.DOFade(1, tweenDuration).SetUpdate(true);
        anim.SetBool("isDead", true);
        playerMovements.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Death"))
        {
            Debug.Log("Death");
            currentHealth = 0;
            Die();
        }
    }
}
