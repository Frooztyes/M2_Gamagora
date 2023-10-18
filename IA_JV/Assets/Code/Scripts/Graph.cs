using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    List<Node> nodes;

    public enum TypeLink
    {
        OneWay,
        TwoWay
    }


    public Graph()
    {
        nodes = new List<Node>();
    }

    public void CreateLink(string _d1, string _d2, float length, TypeLink typeLink = TypeLink.TwoWay)
    {
        Node d1 = GetNodeByName(_d1);
        Node d2 = GetNodeByName(_d2);
        if (d1 == null || d2 == null) return;

        d1.AddNeighboor(d2, length);
        if (typeLink == TypeLink.TwoWay)
            d2.AddNeighboor(d1, length);
    }

    public void CreateLink(Node d1, Node d2, float length, TypeLink typeLink = TypeLink.TwoWay)
    {
        d1.AddNeighboor(d2, length);
        if(typeLink == TypeLink.TwoWay)
            d2.AddNeighboor(d1, length);
    }

    public void AddNode(Node d)
    {
        nodes.Add(d);
    }
    public Node AddNode(string d)
    {
        Node n = new Node(d);
        nodes.Add(n);
        return n;
    }

    public Node GetNodeByName(string name)
    {
        foreach (Node node in nodes)
        {
            if (node.Name == name) return node;
        }
        return null;
    }

    int minDistance(int[] dist, bool[] sptSet)
    {
        int min = int.MaxValue, min_index = -1;

        for (int v = 0; v < nodes.Count; v++)
            if (sptSet[v] == false && dist[v] <= min)
            {
                min = dist[v];
                min_index = v;
            }

        return min_index;
    }

    public Node Trouve_min(Node[] Q, float[] dist)
    {
        float mini = Mathf.Infinity;
        Node sommet = null;

        for (int i = 0; i < Q.Length; i++)
        {
            if(dist[i] <= mini)
            {
                mini = dist[i];
                sommet = Q[i];
            }
        }

        return sommet;
    }

    private void maj_distances(Node _s1, Node _s2, float[] dist, Node[] prev)
    {
        int s1 = nodes.IndexOf(GetNodeByName(_s1.Name));
        int s2 = nodes.IndexOf(GetNodeByName(_s2.Name));
        
        if (dist[s2] > dist[s1] + _s1.GetLengthTo(_s2))
        {
            dist[s2] = dist[s1] + _s1.GetLengthTo(_s2);
            prev[s2] = _s1;
        }

    }

    public List<Node> GetPath2(Node from, Node to)
    {
        int[] dist = new int[nodes.Count];
        bool[] sptSet = new bool[nodes.Count];

        for (int i = 0; i < nodes.Count; i++)
        {
            dist[i] = int.MaxValue;
            sptSet[i] = false;
        }

        dist[nodes.IndexOf(from)] = 0;

        for (int count = 0; count < nodes.Count - 1; count++)
        {
            int u = minDistance(dist, sptSet);
            sptSet[u] = true;
            for (int v = 0; v < nodes.Count; v++)
            {
                int l = nodes[u].GetLengthTo(nodes[v]);

                if (!sptSet[v] && l != 0 &&
                     dist[u] != int.MaxValue && dist[u] + l < dist[v])
                    dist[v] = dist[u] + l;
            }
        }

        List<Node> suite = new List<Node>();
        for (int i = 0; i < nodes.Count; i++)
        {
            Debug.Log(dist[i]);
            suite.Add(nodes[i]);
        }


        return suite;
    }

    public List<Node> GetPath(Node from, Node to)
    {
        if (!nodes.Contains(from) || !nodes.Contains(to))
            return null;

        List<Node> Q = new List<Node>();
        float[] dist = new float[nodes.Count];
        Node[] prev = new Node[nodes.Count];
        for (int i = 0; i < nodes.Count; i++)
        {
            dist[i] = Mathf.Infinity;
            prev[i] = null;
            Q.Add(nodes[i]);
        }

        dist[nodes.IndexOf(from)] = 0;

        while (Q.Count > 0)
        {
            Node u = Trouve_min(Q.ToArray(), dist);
            Q.Remove(u);
            foreach (Node v in u.GetNeighboors())
            {
                maj_distances(u, v, dist, prev);
            }
        }


        Node s = to;
        List<Node> suite = new List<Node>();
        while(s.Name != from.Name)
        {
            suite.Add(s);
            s = prev[nodes.IndexOf(s)];
        }

        suite.Add(from);
        suite.Reverse();

        return suite;
    }

    public override string ToString() 
    {
        string s = "";

        foreach (Node node in nodes)
        {
            s += node;
        }
        return s;
    }


}
