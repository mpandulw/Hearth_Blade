using UnityEditor;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerMovements playerMovements;
    private Animator anim;
    private Rigidbody2D rb;

    private bool isAttacking = false;
    public float attackDuration = 3f;
    public float attackDamage = 25;

    public GameObject attackPos;
    public float rad;
    public LayerMask enemies;

    public bool IsAttacking => isAttacking;

    void Awake()
    {
        playerMovements = GetComponent<PlayerMovements>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void DoAttack()
    {
        if (playerMovements.isGrounded() && !playerMovements.isJumping && !isAttacking)
        {
            isAttacking = true;
            anim.SetBool("isAttacking", true);

            // Hitung posisi serangan berdasarkan arah hadap
            Vector3 adjustedAttackPos = attackPos.transform.position;

            if (!playerMovements.IsFacingRight)
            {
                float offsetX = attackPos.transform.localPosition.x * 2;
                adjustedAttackPos.x -= offsetX;
            }

            Collider2D[] enemy = Physics2D.OverlapCircleAll(adjustedAttackPos, rad, enemies);

            foreach (Collider2D enemyGameObject in enemy)
            {
                Debug.Log("Hit");
                enemyGameObject.GetComponent<EnemyHealth>().TakeDamage(attackDamage);
            }

            rb.linearVelocity = Vector2.zero;

            Invoke(nameof(endAttackBool), attackDuration);
        }
    }

    private void EndAttack()
    {
        anim.SetBool("isAttacking", false);
    }

    private void endAttackBool()
    {
        isAttacking = false;
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (attackPos == null) return;

        // Ambil PlayerMovements untuk cek arah
        PlayerMovements playerMovements = GetComponent<PlayerMovements>();
        Vector3 drawPos = attackPos.transform.position;

        // Jika menghadap kiri, geser posisinya ke kiri
        if (!playerMovements.IsFacingRight)
        {
            float offsetX = (attackPos.transform.localPosition.x * 2);
            drawPos.x -= offsetX;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(drawPos, rad);
#endif
    }

}