
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

    public TrailRenderer tracerEffect;

    public float bulletSpeed = 1000.0f;
    List<Bullet> bullets = new List<Bullet>();


    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public Transform raycastOrigin;
    public int maxHealth = 250;
    public int currentHealth;
    public HealthBar healthBar;
    public GameObject coreItem, respawnPoint, enemyObject, bloodUI;

    public bool canChasePlayer;
    public int currentPatrolIndex;
    public bool inPosition, attackEnemyFirstTime;
    public Transform patrolArea;
    public List<Transform> patrolPoints;

    public ActiveWeapon.WeaponSlot weaponSlot;
    public bool isFiring = false;
    public int fireRate = 25;
    public float bulletDrop = 0.0f;
    public ParticleSystem[] muzzleFlash;
    public ParticleSystem hitEffect;
    public Transform raycastDestination;
    public int patrolIndex;
    Player lorenzo;
    public Animator animator;
    public int shot = 0, currBullet = 0;
    private int spawnDelay;

    private bool isDead = false, isReload = false, coreItemActive = false;

    EnemyAi enemy;
    Ray ray;
    RaycastHit hitInfo;
    float accumulatedTime;
    float maxLifetime = 3.0f;

    private void Start()
    {
        initEnemyPatrolAttackChase();

        if(name.ToLower().Contains("kyle") || name.ToLower().Contains("warrior"))
        {
            animator.SetBool("isDeath", false);
        }

        player = GameObject.Find("Ken").transform;
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        if (name.ToLower().Contains("kyle"))
        {
            canChasePlayer = false;
            shot = 15;
            spawnDelay = 30;
        }else if (name.ToLower().Contains("warrior"))
        {
            canChasePlayer = true;
            shot = 20;
            spawnDelay = 40;
        }
        else if (name.ToLower().Contains("drone"))
        {
            shot = 30;
            canChasePlayer = true;
            spawnDelay = 50;
        }
        else if (name.ToLower().Contains("mech"))
        {
            canChasePlayer = false;
            shot = 20;
            spawnDelay = 0;
        }

        currBullet = shot;
             
    }

    private void Update()
    {
        enemyRoutines();
        if (currBullet <= 0)
        {
            animator.SetBool("isReload", true);
            currBullet += shot;
            isReload = true;
            Invoke(nameof(setReloadAnimation), 3f);
        }
        UpdateBullets(Time.deltaTime);
    }

    private void setReloadAnimation()
    {
        animator.SetBool("isReload", false);
        isReload = false;
    }


    private void Patroling()
    {
        transform.LookAt(patrolPoints[currentPatrolIndex].position);
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);

        float distanceToPatrolPoint = Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position);

        
        if (distanceToPatrolPoint < 1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        }
    }

    private void initEnemyPatrolAttackChase()
    {
        currentPatrolIndex = 0;
        inPosition = attackEnemyFirstTime = false;
    }

    private void enemyRoutines()
    {

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (inPosition && !playerInSightRange && !playerInAttackRange && !isDead && !isReload)
        {
            Patroling();
        }

        if (inPosition && playerInSightRange && !playerInAttackRange && canChasePlayer && !isDead)
        {
            ChasePlayer();
        }

        if (playerInAttackRange && playerInSightRange && !isDead && !isReload)
        {
            AttackPlayer();
        }

        if (!inPosition && attackEnemyFirstTime && !isDead)
        {
            ReachPatrolPosition();
        }
    }

    private void ReachPatrolPosition()
    {
        transform.LookAt(patrolArea.position);
        agent.SetDestination(patrolArea.position);
    }

    private void AttackPlayer()
    {
        attackEnemyFirstTime = true;
        transform.LookAt(player);
        agent.SetDestination(transform.position);

        if (!alreadyAttacked)
        {
            Vector3 velocity = (player.position - raycastOrigin.position).normalized * bulletSpeed;
            var bullet = CreateBullet(raycastOrigin.position, velocity);
            bullets.Add(bullet);
            SoundManager.PlaySound("ShotSFX");
            currBullet--;
            if (hitInfo.transform != null && hitInfo.transform.tag.Equals("Player") && !Player.shieldActive)
            {
                lorenzo = hitInfo.collider.gameObject.GetComponent<Player>();
                int damage = 0;
                if (name.ToLower().Contains("kyle"))
                {
                    damage = 15;
                } else if (name.ToLower().Equals("warrior"))
                {
                    damage = 20;
                } else if (name.ToLower().Equals("drone"))
                {
                    damage = 10;
                } else if (name.ToLower().Equals("mech"))
                {
                    damage = 50;
                }
                StartCoroutine(lorenzo.displayBlood());

                lorenzo.TakeDamage(damage);

            }

            if(bullet.tracer != null)
            {
                bullet.tracer.transform.position = hitInfo.point;
            }

            alreadyAttacked = true;
            
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ChasePlayer()
    {
        transform.LookAt(player.position);
        agent.SetDestination(player.position);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0) {
            isDead = true;
            if (!agent.name.Equals("Mech"))
            {
                animator.SetBool("isDeath", true);
                Invoke(nameof(DestroyEnemy), 3.5f);
            }
        }
    }
    private void DestroyEnemy()
    {
        bool alreadyClean = false;
        if (!alreadyClean)
        {
            alreadyClean = true;
            FindObjectOfType<GenerateEnemy>().cleanPatroliExist(name, patrolIndex, spawnDelay);
        }
        Vector3 pos = transform.position;
        Destroy(gameObject, 5f);
        if (!coreItemActive)
        {
            coreItemActive = true;
            Instantiate(coreItem, pos, Quaternion.identity);
        }
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
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);

            bullet.tracer.transform.position = hitInfo.point;
            bullet.time = maxLifetime;

            if(bullet.tracer != null)
            {
                bullet.tracer.transform.position = end;
            }
        }
    }

    public void UpdateFiring(float deltaTime)
    {
        accumulatedTime += deltaTime;
        float fireInterval = 1.0f / fireRate;
        while (accumulatedTime >= 0.0f)
        {
            accumulatedTime -= fireInterval;
        }
    }
    public void StopFiring()
    {
        isFiring = false;
    }

}
