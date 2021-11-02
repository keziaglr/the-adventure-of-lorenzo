using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttack : MonoBehaviour
{
    List<GameObject> vertex;
    Collider[] hitColliders, prevHitColliders;
    private static int V = 5;
    static int[] parent;
    public Player player;

    public GameObject lightningEffect;

    //public LineRenderer radiusLine;
    public LayerMask WhatIsEnemy;
    public int segments = 40;
    public float xradius = 10f;
    public float yradius = 10f;
    public float radius = 10f;
    private bool checkEnemyInRange = false;
    public int electricDamage = 125;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            initSpecialEffect();
        }
    }
    void initSpecialEffect()
    {
        vertex = new List<GameObject>();
        PerformSpecialEffect();
        //initRadius();
    }

    //void initRadius()
    //{
    //    radiusLine.positionCount = segments + 1;
    //    radiusLine.useWorldSpace = false;
    //    CreatePoints();
    //    radiusLine.gameObject.SetActive(false);
    //}

    //void CreatePoints()
    //{
    //    float x;
    //    float y = 0f;
    //    float z;

    //    float angle = 20f;

    //    for (int i = 0; i < (segments + 1); i++)
    //    {
    //        x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
    //        z = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

    //        radiusLine.SetPosition(i, new Vector3(x, y, z));

    //        angle += (360f / segments);
    //    }
    //}

    //IEnumerator turnOnRadiusLine()
    //{
    //    checkEnemyInRange = true;
    //    radiusLine.gameObject.SetActive(true);

    //    yield return new WaitForSeconds(5);

    //    radiusLine.gameObject.SetActive(false);
    //    checkEnemyInRange = false;
    //}

    //IEnumerator SetBombUIEnemyInRadius(Vector3 center)
    //{
    //    float startTime = Time.time;
    //    float timeElapsed = 0;
    //    while (timeElapsed <= 5)
    //    {
    //        timeElapsed = Time.time - startTime;
    //        hitColliders = null;
    //        hitColliders = Physics.OverlapSphere(transform.position, radius, WhatIsEnemy);
    //        V = hitColliders.Length;
    //        if (prevHitColliders != null)
    //        {
    //            foreach (var phc in prevHitColliders)
    //            {
    //                bool flagExist = false;
    //                foreach (var hc in hitColliders)
    //                {
    //                    if (phc.Equals(hc))
    //                    {
    //                        flagExist = true;
    //                        break;
    //                    }
    //                }
    //                if (!flagExist)
    //                {
    //                    phc.GetComponent<EnemyAi>().setInRange(false);
    //                }
    //            }
    //        }
    //        foreach (var h in hitColliders)
    //        {
    //            h.GetComponent<EnemyAi>().setInRange(true);
    //        }

    //        prevHitColliders = hitColliders;
    //        yield return new WaitForSeconds(0);
    //    }
    //    foreach (var phc in prevHitColliders)
    //    {
    //        phc.GetComponent<EnemyAi>().setInRange(false);
    //    }
    //}

    static int minKey(float[] key, bool[] mstSet)
    {
        // Initialize min value
        float min = float.MaxValue;
        int min_index = -1;

        for (int v = 0; v < V; v++)
        {
            if (mstSet[v] == false && key[v] < min)
            {
                min = key[v];
                min_index = v;
            }
        }
        return min_index;
    }

    static void primMST(List<List<float>> graph)
    {

        parent = new int[V];

        float[] key = new float[V];

        bool[] mstSet = new bool[V];

        for (int i = 0; i < V; i++)
        {
            key[i] = float.MaxValue;
            mstSet[i] = false;
        }

        key[0] = 0;
        parent[0] = -1;

        if (V < 2)
        {
            parent[0] = 0;
        }

        for (int count = 0; count < V - 1; count++)
        {
            int u = minKey(key, mstSet);
            mstSet[u] = true;

            for (int v = 0; v < V; v++)
            {
                if (((List<float>)graph[u])[v] != 0 && mstSet[v] == false
                    && ((List<float>)graph[u])[v] < key[v])
                {
                    parent[v] = u;
                    key[v] = ((List<float>)graph[u])[v];
                }
            }
        }
    }

    void GiveElectricDamageAndLightningStrike()
    {
        List<List<string>> vertexConnections = new List<List<string>>(V);
        for (int i = 0; i < V; i++)
        {
            vertexConnections.Add(new List<string>());
        }

        for (int i = V > 1 ? 1 : 0; i < V; i++)
        {
            string vertex1 = vertex[i].name;
            string vertex2 = vertex[parent[i]].name;
            vertexConnections[i].Add(vertex2);


            GameObject lightning = Instantiate(lightningEffect, vertex[parent[i]].transform.position, Quaternion.identity);
            DigitalRuby.LightningBolt.LightningBoltScript lightningScript = lightning.GetComponent<DigitalRuby.LightningBolt.LightningBoltScript>();
            lightningScript.StartObject = vertex[parent[i]].gameObject;
            lightningScript.EndObject = vertex[i].gameObject;
            Destroy(lightning, 6f);

            if (V > 1)
            {
                vertexConnections[parent[i]].Add(vertex1);
            }
        }
        for (int i = 0; i < V; i++)
        {
            if (vertex[i].tag != "Player")
            {
                EnemyAi t = vertex[i].GetComponent<EnemyAi>();
                t.TakeDamage(electricDamage * vertexConnections[i].Count);
            }
        }
        player.DecreaseSkill(75);
    }
    void PerformSpecialEffect()
    {
        SoundManager.PlaySound("LightningSFX");
        if (vertex.Count > 0)
            vertex.Clear();

        var adjacentList = new List<List<float>>();
        makeAdjacencyList(adjacentList);
        primMST(adjacentList);
        GiveElectricDamageAndLightningStrike();
    }

    void makeAdjacencyList(List<List<float>> adjacentList)
    {
        hitColliders = Physics.OverlapSphere(transform.position, radius, WhatIsEnemy);

        foreach (Collider c in hitColliders)
        {
            vertex.Add(c.gameObject);
        }
        vertex.Add(gameObject);

        V = vertex.Count;

        foreach (var i in vertex)
        {
            var adjacentListRow = new List<float>();
            foreach (var j in vertex)
            {
                float edgeWeight;
                if (i.Equals(j))
                    edgeWeight = 0;
                else
                    edgeWeight = Vector3.Distance(j.transform.position, i.transform.position);
                adjacentListRow.Add(edgeWeight);
            }
            adjacentList.Add(adjacentListRow);
        }
    }

}
