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
        BackgroundMusic.instance.PlayNormalBGM();
        deadMenu.DOFade(0, fadeDuration);
        deadPanelGameObject.SetActive(false);

        string checkpointScene = PlayerPrefs.GetString("scene", "");
        string currentScene = SceneManager.GetActiveScene().name;

        // Case 1: No saved checkpoint
        if (string.IsNullOrEmpty(checkpointScene))
        {
            SceneManager.sceneLoaded += OnSceneLoadedWithoutCheckpoint;
            SceneManager.LoadScene(currentScene);
            return;
        }

        // Case 2: Checkpoint exists but different scene
        if (currentScene != checkpointScene)
        {
            PlayerPrefs.SetInt("ShouldRespawn", 1);
            SceneManager.LoadScene(checkpointScene);
            return;
        }

        // Case 3: Checkpoint exists and already in same scene
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

    private void OnSceneLoadedWithoutCheckpoint(Scene scene, LoadSceneMode mode)
    {
        player.transform.position = startPoint;
        playerHealth.currentHealth = playerHealth.maxHealth;
        anim.SetBool("isDead", false);
        playerMovements.enabled = true;

        SceneManager.sceneLoaded -= OnSceneLoadedWithoutCheckpoint; // Clean up
    }

}
