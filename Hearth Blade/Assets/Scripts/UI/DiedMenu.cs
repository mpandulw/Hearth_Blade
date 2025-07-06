using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiedMenu : MonoBehaviour
{
    [Header("Respawn")]
    public GameObject player;
    public float fadeDuration;
    [SerializeField] private GameObject deadPanelGameObject;
    [SerializeField] private CanvasGroup deadMenu;
    public Vector2 startPoint;

    private PlayerHealth playerHealth;
    private PlayerMovements playerMovements;
    private Animator anim;

    void Awake()
    {
        playerHealth = player.GetComponent<PlayerHealth>();
        anim = player.GetComponent<Animator>();
        playerMovements = player.GetComponent<PlayerMovements>();
    }

    public void Respawn()
    {
        deadMenu.DOFade(0, fadeDuration);
        deadPanelGameObject.SetActive(false);

        string checkpointScene = PlayerPrefs.GetString("scene");
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene != checkpointScene)
        {
            PlayerPrefs.SetInt("ShouldRespawn", 1);
            SceneManager.LoadScene(PlayerPrefs.GetString("scene"));
            return;
        }

        DoRespawnOnCheckPoint();
    }

    private void DoRespawnOnCheckPoint()
    {
        Vector2 checkpointPos = new Vector2(
            PlayerPrefs.GetFloat("checkpointPosX"),
            PlayerPrefs.GetFloat("checkpointPosY")
        );

        player.transform.position = checkpointPos;
        playerHealth.currentHealth = playerHealth.maxHealth;
        anim.SetBool("isDead", false);
        playerMovements.enabled = true;
    }
}
