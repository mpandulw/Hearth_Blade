using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private DiedMenu diedMenu;
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

    [Header("Checkpoint")]
    [SerializeField] private GameObject buttonInteract;
    [SerializeField] private GameObject checkpointPos;

    private float currentVelocity = 0;

    void Awake()
    {
        playerMovements = GetComponent<PlayerMovements>();
        anim = GetComponent<Animator>();
        diedMenu = FindAnyObjectByType<DiedMenu>();
    }

    void Start()
    {
        float playerHealthPoint = PlayerPrefs.GetFloat("playerHealthPoint");
        if (PlayerPrefs.HasKey("playerHealthPoint"))
        {
            currentHealth = playerHealthPoint;
        }
        else
        {
            currentHealth = maxHealth;
        }

        hpBar.maxValue = maxHealth;
        hpBar.value = currentHealth;

        // Cek respawn karena mati
        if (PlayerPrefs.GetInt("ShouldRespawn", 0) == 1)
        {
            Vector2 checkpointPos = new Vector2(
                PlayerPrefs.GetFloat("checkpointPosX"),
                PlayerPrefs.GetFloat("checkpointPosY")
            );

            transform.position = checkpointPos;
            currentHealth = maxHealth;
            anim.SetBool("isDead", false);
            playerMovements.enabled = true;

            PlayerPrefs.SetInt("ShouldRespawn", 0);
        }
        // Cek continue dari menu utama
        else if (PlayerPrefs.GetInt("ShouldContinue", 0) == 1)
        {
            Vector2 lastPos = new Vector2(
                PlayerPrefs.GetFloat("lastPlayerPosX"),
                PlayerPrefs.GetFloat("lastPlayerPosY")
            );

            transform.position = lastPos;
            PlayerPrefs.SetInt("ShouldContinue", 0);
        }
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

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            buttonInteract.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            buttonInteract.SetActive(false);
        }
    }

    public void Checkpoint()
    {
        currentHealth = maxHealth;
        SaveCheckpoint(checkpointPos.transform.position, SceneManager.GetActiveScene().name);
        diedMenu.startPoint = (Vector2)checkpointPos.transform.position;
        Debug.Log(diedMenu.startPoint);
    }

    private void SaveCheckpoint(Vector2 checkpointPos, string sceneName)
    {
        PlayerPrefs.SetFloat("checkpointPosX", checkpointPos.x);
        PlayerPrefs.SetFloat("checkpointPosY", checkpointPos.y);
        PlayerPrefs.SetString("scene", sceneName);
    }
}
