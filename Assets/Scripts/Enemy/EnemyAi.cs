
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
    public GameObject coreItem, respawnPoint, enemyObject, bloodUI;


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

    private bool isDead = false, isReload = false, coreItemActive = false;

    EnemyAi enemy;
    Ray ray;
    RaycastHit hitInfo;
    float accumulatedTime;
    float maxLifetime = 3.0f;

    private void Start()
    {
        animator.SetBool("isDeath", false);
        player = GameObject.Find("Ken").transform;
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        //Debug.Log(agent.name);
        if (agent.name.Equals("Kyle"))
        {
            shot = 15;
        }else if (agent.name.Equals("Warrior"))
        {
            shot = 20;
        }
        else if (agent.name.Equals("Drone"))
        {
            shot = 30;
        }
        else if (agent.name.Equals("Mech"))
        {
            shot = 20;
        }

        currBullet = shot;
        dest = start = points[0].transform.position;
        end = points[1].transform.position;
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        //Patroling();
        if (!playerInSightRange && !playerInAttackRange && !isDead && !isReload)
        {
            Patroling();
        }
        if (playerInAttackRange && playerInSightRange && !isDead && !isReload)
        {
            AttackPlayer();
        }

        if(currBullet <= 0)
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

    public Vector3 dest, start, end;
    private void Patroling()
    {

        //Debug.Log(agent.name + " : " +  agent.remainingDistance);
        //Debug.Log(start);
        //Debug.Log(dest);
        //Debug.Log(agent.name);
        if (agent.remainingDistance <= 0)
        {
            dest = (dest == start) ? end : start;
        }
        transform.LookAt(dest);
        agent.SetDestination(dest);

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
            SoundManager.PlaySound("ShotSFX");
            currBullet--;

            if (hitInfo.transform != null && hitInfo.transform.tag.Equals("Player"))
            {
                lorenzo = hitInfo.collider.gameObject.GetComponent<Player>();
                int damage = 0;
                if (agent.name.Equals("Kyle"))
                {
                    damage = 15;
                } else if (agent.name.Equals("Warrior"))
                {
                    damage = 20;
                } else if (agent.name.Equals("Drone"))
                {
                    damage = 10;
                } else if (agent.name.Equals("Mech"))
                {
                    damage = 50;
                }
                bloodUI.SetActive(true);

                lorenzo.TakeDamage(damage);
                //Debug.Log("Player attacked");

            }

            alreadyAttacked = true;
            
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        bloodUI.SetActive(false);
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0 && !agent.name.Equals("Mech")) {
            animator.SetBool("isDeath", true);
            Invoke(nameof(DestroyEnemy), 3.5f);
        }
    }
    private void DestroyEnemy()
    {
        Vector3 pos = transform.position;
        Destroy(gameObject);
        //gen.cleanPatroliExist("Kyle", patrolIndex, 30);
        if (!coreItemActive)
        {
            coreItemActive = true;
            Instantiate(coreItem, pos, Quaternion.identity);
        }
        //Instantiate(enemyObject, respawnPoint.transform.position, Quaternion.identity);
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

            if (hitInfo.collider.gameObject.tag.Equals("Enemy"))
            {
                enemy = hitInfo.collider.gameObject.GetComponent<EnemyAi>();
                enemy.TakeDamage(10);
            }

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
            //FireBullet();
            accumulatedTime -= fireInterval;
        }
    }
    public void StopFiring()
    {
        isFiring = false;
    }

}
