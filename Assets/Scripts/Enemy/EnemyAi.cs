
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyAi : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public List<GameObject> points;


    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public int maxHealth = 250;
    public int currentHealth;
    public HealthBar healthBar;

    private void Start()
    {
        player = GameObject.Find("Ken").transform;
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        dest = start = points[0].transform.position;
        end = points[1].transform.position;
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        Patroling();
        //if (!playerInSightRange && !playerInAttackRange) Patroling();
        //if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }
    Vector3 dest, start, end;
    private void Patroling()
    {
        agent.SetDestination(dest);

        if (agent.remainingDistance <= 0)
        {
            dest = (dest == start) ? end : start;
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

}
