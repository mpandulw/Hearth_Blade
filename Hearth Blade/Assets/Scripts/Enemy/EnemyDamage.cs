using UnityEngine;
using UnityEngine.Rendering;

public class EnemyDamage : MonoBehaviour
{
    public float damage;

    private PlayerHealth playerHealth;
    private PlayerMovements playerMovements;
    public GameObject player;
    public Transform attackPos;
    public float rad;
    private Animator anim;
    public float attackCooldown = 1.0f;
    private float cooldownTimer;
    public float attackRange = 1.5f;

    public bool IsAttacking { get; private set; }

    void Update()
    {
        if (IsAttacking) return;

        cooldownTimer -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance <= attackRange && cooldownTimer <= 0)
        {
            Attack(); // Start attack once
        }
    }


    void Awake()
    {
        anim = GetComponent<Animator>();
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

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (attackPos == null) return;

        Vector3 drawPos = attackPos.position;

        // Cek arah hadap musuh berdasarkan localScale.x
        if (transform.localScale.x < 0)
        {
            // Jika menghadap kanan tapi posisi attackPos di kiri, geser ke kanan
            float offsetX = attackPos.localPosition.x * 2f;
            drawPos.x -= offsetX;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(drawPos, rad);
#endif
    }

    private void Attack()
    {
        anim.SetInteger("state", 2); // animasi attack
        IsAttacking = true;
        cooldownTimer = attackCooldown;

        // Deteksi apakah player ada di dalam area attack
        Collider2D hit = Physics2D.OverlapCircle(attackPos.position, rad, LayerMask.GetMask("Player"));
        if (hit != null)
        {
            // Ambil komponen
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            PlayerMovements playerMovements = hit.GetComponent<PlayerMovements>();

            // Berikan damage
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Knockback
            if (playerMovements != null)
            {
                playerMovements.knockbackCounter = playerMovements.knockbackTotalTime;

                // Tentukan arah knockback berdasarkan posisi relatif
                if (player.transform.position.x <= transform.position.x)
                {
                    playerMovements.knockbackFromRight = true;
                }
                else
                {
                    playerMovements.knockbackFromRight = false;
                }
            }
        }
    }

    private void EndAttack() // dipanggil lewat Animation Event
    {
        anim.SetBool("isAttacking", false);
        IsAttacking = false; // <--- Reset state
    }
}
