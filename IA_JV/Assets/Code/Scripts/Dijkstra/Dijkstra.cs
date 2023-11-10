using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dijkstra : Graph
{
    public int Trouve_min(int[] Q, float[] dist)
    {
        float mini = Mathf.Infinity;
        int sommet = -1;

        foreach (int s in Q)
        {
            if (dist[s] < mini)
            {
                mini = dist[s];
                sommet = s;
            }
        }

        return sommet;
    }

    public List<Node> GetPath(Node from, Node to)
    {
        if (!Nodes.ContainsKey(from.Position) || !Nodes.ContainsKey(to.Position))
            return null;

        List<int> Q = new List<int>();
        float[] dist = new float[Nodes.Count];
        int[] prev = new int[Nodes.Count];
        for (int i = 0; i < Nodes.Count; i++)
        {
            dist[i] = Mathf.Infinity;
            prev[i] = -1;
            Q.Add(i);
        }

        dist[from.Index] = 0;

        while (Q.Count > 0)
        {
            int u = Trouve_min(Q.ToArray(), dist);
            if (u == -1)
            {
                break;
            }
            Q.Remove(u);
            foreach (Node v in NodesIndex[u].GetNeighboors())
            {
                int v2 = GetNodeByPosition(v.Position).Index;
                if (Q.Contains(v2))
                {
                    float alt = dist[u] + Nodes.ElementAt(v2).Value.GetLengthTo(Nodes.ElementAt(u).Value);
                    if (alt < dist[v2])
                    {
                        dist[v2] = alt;
                        prev[v2] = u;
                    }
                }

            }
        }


        int sdeb = from.Index;
        int sfin = to.Index;
        int s = sfin;
        List<Node> suite = new List<Node>();
        while (s != sdeb && s != -1)
        {
            suite.Add(Nodes.ElementAt(s).Value);
            s = prev[s];
        }

        suite.Add(from);
        suite.Reverse();

        return suite;
    }

}
