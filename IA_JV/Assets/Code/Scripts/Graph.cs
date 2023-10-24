using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph
{
    public Dictionary<Vector2, Node> Nodes { get; set; }


    public Graph()
    {
        Nodes = new Dictionary<Vector2, Node>();
    }

    public void RemoveNode(Node n)
    {
        foreach(var neighbor in n.Neighbors.ToList())
        {
            neighbor.Key.Neighbors.Remove(n);
        }
        Nodes.Remove(n.Position);
    }

    public void CreateLink(Vector2 _d1, Vector2 _d2, float length)
    {
        Node d1 = GetNodeByPosition(_d1);
        Node d2 = GetNodeByPosition(_d2);
        if (d1 == null || d2 == null) return;

        d1.AddNeighbor(d2, length);
        d2.AddNeighbor(d1, length);
    }

    public void CreateLink(Node d1, Node d2, float length)
    {
        d1.AddNeighbor(d2, length);
        d2.AddNeighbor(d1, length);
    }

    public void AddNode(Node d)
    {
        Nodes.Add(d.Position, d);
    }
    public Node AddNode(Vector2 pos)
    {
        Node n = new Node(pos);
        Nodes.Add(pos, n);
        return n;
    }

    public Node GetNodeByPosition(Vector2 position)
    {
        if (Nodes.TryGetValue(position, out Node n))
            return n;
        return null;
    }

    public int Trouve_min(int[] Q, float[] dist)
    {
        float mini = Mathf.Infinity;
        int sommet = -1;

        foreach (int s in Q)
        {
            if(dist[s] < mini)
            {
                mini = dist[s];
                sommet = s;
            }
        }

        return sommet;
    }


    public int GetIndex(Node n)
    {
        return Nodes.Values.ToList().IndexOf(n);
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

        dist[GetIndex(from)] = 0;

        while (Q.Count > 0)
        {
            int u = Trouve_min(Q.ToArray(), dist);
            if(u == -1)
            {
                break;
            }
            Q.Remove(u);
            foreach (Node v in Nodes.ElementAt(u).Value.GetNeighboors())
            {
                int v2 = GetIndex(GetNodeByPosition(v.Position));
                if (Q.Contains(v2))
                {
                    float alt = dist[u] + Nodes.ElementAt(v2).Value.GetLengthTo(Nodes.ElementAt(u).Value);
                    if(alt < dist[v2])
                    {
                        dist[v2] = alt;
                        prev[v2] = u;
                    }
                }

            }
        }


        int sdeb = GetIndex(from);
        int sfin = GetIndex(to);
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

    public override string ToString() 
    {
        string s = "";

        foreach (Node node in Nodes.Values)
        {
            s += node;
        }
        return s;
    }

    public Node GetNodeByVector(Vector2 pos)
    {
        return GetNodeByPosition(pos);
    }

    public void RecalculateLength(Node current, Node neigh1, Node neigh2)
    {
        if (current == null) return;
        if (neigh1 == null) return;
        if (neigh2 == null) return;

        int lengthTo1 = current.GetLengthTo(neigh1);
        int lengthTo2 = current.GetLengthTo(neigh2);

        neigh2.AddNeighbor(neigh1, lengthTo1 + lengthTo2);
        neigh1.AddNeighbor(neigh2, lengthTo1 + lengthTo2);

        neigh1.RemoveNeighbor(current);
        neigh2.RemoveNeighbor(current);
    }
}
