using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class MobController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Movement")]
    [SerializeField] private float attackDistance = 5f;
    [SerializeField] private float updatePathInterval = 0.2f;

    [Header("Score")]
    [SerializeField] private int scoreForKill = 1;

    private NavMeshAgent agent;
    private Animator animator;

    private float pathTimer;
    private bool isDead;
    private bool isAttacking;

    public Transform Target
    {
        get => target;
        set => target = value;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        agent.stoppingDistance = attackDistance;

        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    private void Update()
    {
        if (isDead)
            return;

        if (target == null)
            return;

        UpdateMovement();
        UpdateAttackState();
    }

    private void UpdateMovement()
    {
        pathTimer += Time.deltaTime;

        if (pathTimer >= updatePathInterval)
        {
            pathTimer = 0f;

            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.SetDestination(target.position);
            }
        }
    }

    private void UpdateAttackState()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackDistance)
        {
            if (!isAttacking)
            {
                isAttacking = true;
                animator.SetBool("Attack", true);

                if (agent.enabled && agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                    agent.ResetPath();
                }
            }
        }
        else
        {
            if (isAttacking)
            {
                isAttacking = false;
                animator.SetBool("Attack", false);

                if (agent.enabled && agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                }
            }
        }
    }

    public void Death()
    {
        if (isDead)
            return;

        isDead = true;

        animator.SetBool("Attack", false);
        animator.SetBool("Die", true);

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        Collider monsterCollider = GetComponent<Collider>();

        if (monsterCollider != null)
        {
            monsterCollider.enabled = false;
        }

        ScoreManager.Instance?.AddScore(scoreForKill);
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void DestroyAfterDeathAnimation()
    {
        Die();
    }
}