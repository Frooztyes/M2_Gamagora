using System.Collections;
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

    public Node GetNodeByName(string name)
    {
        foreach (Node node in nodes)
        {
            if (node.Name == name) return node;
        }
        return null;
    }

    public Node Trouve_min(Node[] Q, float[] dist)
    {
        float mini = Mathf.Infinity;
        Node sommet = null;

        for (int i = 0; i < Q.Length; i++)
        {
            if(dist[i] < mini)
            {
                mini = dist[i];
                sommet = Q[i];
            }
        }

        return sommet;
    }


    public List<Node> GetPath(Node from, Node to)
    {
        if (!nodes.Contains(from) || !nodes.Contains(to))
            return null;

        Node[] Q = new Node[nodes.Count];

        float[] dist = new float[nodes.Count];
        Node[] prev = new Node[nodes.Count];
        for (int i = 0; i < nodes.Count; i++)
        {
            dist[i] = Mathf.Infinity;
            prev[i] = null;
            Q[i] = nodes[i];
        }

        dist[nodes.IndexOf(from)] = 0;
        int idU = 0;
        while (Q.Length > 0)
        {
            Node u = Q[idU].GetClosestNeighboor();
            
        }


        return null;
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
