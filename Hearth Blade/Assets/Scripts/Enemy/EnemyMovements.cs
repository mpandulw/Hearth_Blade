using System.Collections;
using UnityEngine;

public class EnemyMovements : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float moveSpeed;
    public int patrolDestination;
    public float idleDuration;
    public GameObject player;
    public float chaseRange = 5f;
    public float attackRange = 1.5f; // Jarak untuk mulai attack

    private bool isWaiting;
    private Animator anim;
    private EnemyDamage enemyDamage; // Referensi ke script attack

    private Vector3 originalScale;

    private enum MovementsState
    {
        idle,
        run,
        attack
    }

    void Awake()
    {
        anim = GetComponent<Animator>();
        enemyDamage = GetComponent<EnemyDamage>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isWaiting) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        // Jika dalam jarak attack, stop dan animasi attack
        if (distanceToPlayer <= attackRange)
        {
            anim.SetInteger("state", (int)MovementsState.attack);

            // Flip ke arah player
            if (player.transform.position.x > transform.position.x)
                transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            else
                transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

            return; // Jangan gerak lagi
        }

        // Jika dalam range chase tapi belum cukup untuk attack
        if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void ChasePlayer()
    {
        // Ambil posisi musuh sekarang
        Vector2 currentPosition = transform.position;

        // Buat posisi target hanya berbeda di sumbu X, Y tetap
        Vector2 targetPosition = new Vector2(player.transform.position.x, currentPosition.y);

        // Gerakkan musuh hanya ke arah X
        transform.position = Vector2.MoveTowards(currentPosition, targetPosition, moveSpeed * Time.deltaTime);

        // Balik arah sprite kalau perlu
        if (player.transform.position.x > transform.position.x)
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        // Jalankan animasi jalan/lari
        anim.SetInteger("state", (int)MovementsState.run);
    }


    private void Patrol()
    {
        Transform targetPoint = patrolPoints[patrolDestination];

        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        if (targetPoint.position.x > transform.position.x)
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f && !isWaiting)
        {
            StartCoroutine(WaitAtPatrolPoint());
            return;
        }

        anim.SetInteger("state", (int)MovementsState.run);
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        isWaiting = true;
        anim.SetInteger("state", (int)MovementsState.idle);

        yield return new WaitForSeconds(idleDuration);

        patrolDestination = (patrolDestination + 1) % patrolPoints.Length;
        isWaiting = false;
    }
}
