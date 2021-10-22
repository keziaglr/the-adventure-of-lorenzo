
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyAi : MonoBehaviour
{
    class Bullet
    {
        public float time;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        public TrailRenderer tracer;
    }
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public List<GameObject> points;
    public TrailRenderer tracerEffect;

    public float bulletSpeed = 1000.0f;
    List<Bullet> bullets = new List<Bullet>();


    public Vector3 walkPoint;
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public Transform raycastOrigin;
    public int maxHealth = 250;
    public int currentHealth;
    public HealthBar healthBar;


    public ActiveWeapon.WeaponSlot weaponSlot;
    public bool isFiring = false;
    public int fireRate = 25;
    public float bulletDrop = 0.0f;
    public ParticleSystem[] muzzleFlash;
    public ParticleSystem hitEffect;
    public Transform raycastDestination;
    Player lorenzo;


    EnemyAi enemy;
    Ray ray;
    RaycastHit hitInfo;
    float accumulatedTime;
    float maxLifetime = 3.0f;

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
        //Patroling();
        if (!playerInSightRange && !playerInAttackRange)
        {
            Patroling();
        }
        if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
        }
        UpdateBullets(Time.deltaTime);
    }
    Vector3 dest, start, end;
    private void Patroling()
    {
        agent.SetDestination(dest);

        //Debug.Log(agent.name + " : " +  agent.remainingDistance);
        if (agent.remainingDistance <= 0)
        {
            //Debug.Log(agent.name);
            dest = (dest == start) ? end : start;
        }

        //Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //if (distanceToWalkPoint.magnitude < 1f)
        //    walkPointSet = false;
    }

    private void AttackPlayer()
    {
        //agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            //Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            //rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            //rb.AddForce(transform.up * 8f, ForceMode.Impulse);

            Vector3 velocity = (player.position - raycastOrigin.position).normalized * bulletSpeed;
            var bullet = CreateBullet(raycastOrigin.position, velocity);
            bullets.Add(bullet);

            if (hitInfo.transform != null && hitInfo.transform.tag.Equals("Player"))
            {
                lorenzo = hitInfo.collider.gameObject.GetComponent<Player>();
                lorenzo.TakeDamage(15);
                //Debug.Log("Player attacked");

            }

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

    Vector3 GetPosition(Bullet bullet)
    {
        Vector3 gravity = Vector3.down * bulletDrop;
        return (bullet.initialPosition) + (bullet.initialVelocity * bullet.time) + (0.5f * gravity * bullet.time * bullet.time);
    }

    Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        Bullet bullet = new Bullet();
        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.tracer = Instantiate(tracerEffect, position, Quaternion.identity);
        bullet.tracer.AddPosition(position);
        return bullet;
    }

    public void StartFiring()
    {
        isFiring = true;
        accumulatedTime = 0.0f;
        //FireBullet();
    }

    public void UpdateBullets(float deltaTime)
    {
        SimulatedBullets(deltaTime);
        DestroyBullets();
    }

    void SimulatedBullets(float deltaTime)
    {
        bullets.ForEach(bullet => {
            Vector3 p0 = GetPosition(bullet);
            bullet.time += deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
        });
    }

    void DestroyBullets()
    {
        bullets.RemoveAll(bullet => bullet.time >= maxLifetime);
    }

    void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        ray.origin = start;
        ray.direction = end - start;
        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            //Debug.Log(hitInfo.collider.gameObject.name);
            //Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);

            bullet.tracer.transform.position = hitInfo.point;
            bullet.time = maxLifetime;

            if (hitInfo.collider.gameObject.tag.Equals("Enemy"))
            {
                enemy = hitInfo.collider.gameObject.GetComponent<EnemyAi>();
                enemy.TakeDamage(10);
                Debug.Log(enemy.name);
            }
            Debug.Log(hitInfo.collider.gameObject.tag);

            if(bullet.tracer != null)
            {
                bullet.tracer.transform.position = end;
            }
        }
        //else
        //{
            
        //}
    }

    public void UpdateFiring(float deltaTime)
    {
        accumulatedTime += deltaTime;
        float fireInterval = 1.0f / fireRate;
        while (accumulatedTime >= 0.0f)
        {
            //FireBullet();
            accumulatedTime -= fireInterval;
        }
    }

    public void StopFiring()
    {
        isFiring = false;
    }

}
