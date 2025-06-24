using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private float currentHealth;
    public float maxHealth;
    private Animator anim;
    private bool isDead = false;

    public bool getIsDead => isDead;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);
        anim.SetTrigger("hit");
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        anim.SetBool("dead", true);
        isDead = true;

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.enabled = false;

        Destroy(gameObject, 2f);
    }
}