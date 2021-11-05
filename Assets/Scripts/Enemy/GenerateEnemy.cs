using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GenerateEnemy : MonoBehaviour
{
    public class EnemyGenerated
    {
        public GameObject enemy;
        public NavMeshAgent agent;
        public int patrolIndex;
        public Transform patrolPos;
        public bool arrived;

        public EnemyGenerated(GameObject e, NavMeshAgent a, int pI, Transform pP)
        {
            enemy = e;
            agent = a;
            patrolIndex = pI;
            patrolPos = pP;
            arrived = false;
        }

    }

    [System.Serializable]
    public class Patroli
    {
        public string enemyType;
        public GameObject enemyPrefabs;
        public Transform[] patrolPos;
        public Transform spawnPosition;
        public List<EnemyGenerated> enemyList;
        public bool[] satpamExist;
    }

    public List<Patroli> patrolPositions = new List<Patroli>();

    void Start()
    {
        foreach (Patroli p in patrolPositions)
        {
            p.satpamExist = new bool[p.patrolPos.Length];
            p.enemyList = new List<EnemyGenerated>();
        }
    }

    void Update()
    {
        foreach (Patroli p in patrolPositions)
        {
            if (p.enemyList.Count < p.patrolPos.Length)
                generateEnemy(p);
            updateEnemyArrival(p);
        }
    }

    void updateEnemyArrival(Patroli p)
    {
        foreach (EnemyGenerated e in p.enemyList)
        {
            if (e.enemy == null)
            {
                p.enemyList.Remove(e);
                break;
            }
            else
            {
                if (!e.arrived)
                {
                    if (Vector3.Distance(e.enemy.transform.position, e.patrolPos.position) < 0.5f)
                    {
                        e.arrived = true;
                        //e.agent.SetDestination(e.enemy.transform.position);
                        e.enemy.GetComponent<EnemyAi>().inPosition = true;
                    }
                    else
                    {
                        e.agent.SetDestination(e.patrolPos.position);
                    }
                }

            }

        }
    }

    void generateEnemy(Patroli p)
    {
        for (int i = 0; i < p.satpamExist.Length; i++)
        {
            if (!p.satpamExist[i])
            {
                spawnEnemy(p, i);
                p.satpamExist[i] = true;
            }
        }
    }

    void spawnEnemy(Patroli p, int patrolIndex)
    {
        Transform destination = p.patrolPos[patrolIndex];
        GameObject enemy = Instantiate(p.enemyPrefabs, p.spawnPosition.position, Quaternion.identity);
        enemy.GetComponent<EnemyAi>().patrolIndex = patrolIndex;
        enemy.GetComponent<EnemyAi>().patrolArea = destination;
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        for (int i = 0; i < destination.transform.childCount; i++)
        {
            enemy.GetComponent<EnemyAi>().patrolPoints.Add(destination.transform.GetChild(i));
        }

        agent.transform.LookAt(destination);
        agent.SetDestination(destination.position);
        p.enemyList.Add(new EnemyGenerated(enemy, agent, patrolIndex, destination));
    }

    Patroli findPatroli(string enemyType)
    {
        foreach (Patroli p in patrolPositions)
        {
            if (enemyType.ToLower().Contains(p.enemyType.ToLower()))
                return p;
        }
        return null;
    }

    public void cleanPatroliExist(string enemyType, int patrolIndex, int delay)
    {
        StartCoroutine(makePatrolPointEmpty(enemyType, patrolIndex, delay));
    }

    public IEnumerator makePatrolPointEmpty(string enemyType, int patrolIndex, int delay)
    {
        Patroli p = findPatroli(enemyType);
        if (p != null)
        {
            yield return new WaitForSeconds(delay);
            p.satpamExist[patrolIndex] = false;
        }
        yield return null;
    }
}
