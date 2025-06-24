using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float damage;

    private PlayerHealth playerHealth;
    private PlayerMovements playerMovements;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            playerHealth.TakeDamage(damage);
            playerMovements = collision.gameObject.GetComponent<PlayerMovements>();
            playerMovements.knockbackCounter = playerMovements.knockbackTotalTime;
            if (collision.transform.position.x <= transform.position.x)
            {
                playerMovements.knockbackFromRight = true;
            }
            if (collision.transform.position.x > transform.position.x)
            {
                playerMovements.knockbackFromRight = false;
            }
        }
    }
}
