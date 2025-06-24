using DG.Tweening;
using UnityEngine;

public class DiedMenu : MonoBehaviour
{
    [Header("Respawn")]
    public GameObject player;
    public float fadeDuration;
    [SerializeField] private GameObject deadPanelGameObject;
    [SerializeField] private CanvasGroup deadMenu;
    private Vector2 startPoint;

    private PlayerHealth playerHealth;
    private PlayerMovements playerMovements;
    private Animator anim;

    void Awake()
    {
        playerHealth = player.GetComponent<PlayerHealth>();
        anim = player.GetComponent<Animator>();
        startPoint = player.transform.position;
        playerMovements = player.GetComponent<PlayerMovements>();
    }

    public void Respawn()
    {
        deadMenu.DOFade(0, fadeDuration);
        deadPanelGameObject.SetActive(false);
        player.transform.position = startPoint;
        playerHealth.currentHealth = playerHealth.maxHealth;
        anim.SetBool("isDead", false);
        playerMovements.enabled = true;
    }
}
