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

    public int Trouve_min2(int[] Q, float[] dist)
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
        if (!nodes.Contains(from) || !nodes.Contains(to))
            return null;

        List<int> Q = new List<int>();
        float[] dist = new float[nodes.Count];
        int[] prev = new int[nodes.Count];
        for (int i = 0; i < nodes.Count; i++)
        {
            dist[i] = Mathf.Infinity;
            prev[i] = -1;
            Q.Add(i);
        }

        dist[nodes.IndexOf(from)] = 0;

        while (Q.Count > 0)
        {
            int u = Trouve_min2(Q.ToArray(), dist);
            if(u == -1)
            {
                break;
            }
            Q.Remove(u);
            foreach (Node v in nodes[u].GetNeighboors())
            {
                int v2 = nodes.IndexOf(GetNodeByName(v.Name));
                if (Q.Contains(v2))
                {
                    float alt = dist[u] + nodes[v2].GetLengthTo(nodes[u]);
                    if(alt < dist[v2])
                    {
                        dist[v2] = alt;
                        prev[v2] = u;
                    }
                }

            }
        }


        int sdeb = nodes.IndexOf(from);
        int sfin = nodes.IndexOf(to);
        int s = sfin;
        List<Node> suite = new List<Node>();
        while (s != sdeb && s != -1)
        {
            suite.Add(nodes[s]);
            s = prev[s];
        }

        suite.Add(from);
        suite.Reverse();

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
                if(Q.Contains(v))
                {

                    maj_distances(u, v, dist, prev);




                }
               
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
