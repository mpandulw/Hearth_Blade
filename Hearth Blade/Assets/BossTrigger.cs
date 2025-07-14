using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject invisibleWall;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            invisibleWall.SetActive(true);
            var bossMov = boss.GetComponent<EnemyMovements>();
            bossMov.enabled = true;
            BackgroundMusic.instance.PlayBossBGM();
        }
    }
}
