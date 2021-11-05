using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialAttack : MonoBehaviour
{
    List<GameObject> vertex;
    Collider[] hitColliders, prevHitColliders;
    private static int V = 5;
    static int[] parent;
    public Player player;

    public GameObject lightningEffect;

    public LayerMask WhatIsEnemy;
    public int segments = 40;
    public float xradius = 10f;
    public float yradius = 10f;
    public float radius = 10f;
    public int electricDamage = 125;
    public GameObject alertObj;
    public Text alertTxt;
    public PauseMenu pm;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && Player.currentSkill >= 75)
        {
            initSpecialEffect();
        }else if(Input.GetKeyDown(KeyCode.Z) && Player.currentSkill < 75)
        {
            StartCoroutine(pm.setAlertText("Not Enough Skill"));
        }
    }
    void initSpecialEffect()
    {
        vertex = new List<GameObject>();
        PerformSpecialEffect();
    }

   

    static int minKey(float[] key, bool[] mstSet)
    {
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
