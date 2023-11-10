using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph
{
    int index = 0;
    public Dictionary<Vector2, Node> Nodes { get; set; }
    public Dictionary<int, Node> NodesIndex { get; set; }

    protected Graph()
    {
        Nodes = new Dictionary<Vector2, Node>();
        NodesIndex = new Dictionary<int, Node>();
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

    public Node AddNode(Vector2 pos)
    {
        if (GetNodeByPosition(pos) != null) return GetNodeByPosition(pos);

        Node n = new Node(pos, Nodes.Count);
        Nodes.Add(pos, n);
        NodesIndex.Add(n.Index, n);
        return n;
    }

    public Node GetNodeByPosition(Vector2 position)
    {
        if (Nodes.TryGetValue(position, out Node n))
            return n;
        return null;
    }

    public int GetIndex(Node n)
    {
        return n.Index;
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

    public void RecalculateLength(Node current, Node neigh1, Node neigh2)
    {
        if (current == null) return;
        if (neigh1 == null) return;
        if (neigh2 == null) return;

        float lengthTo1 = current.GetLengthTo(neigh1);
        float lengthTo2 = current.GetLengthTo(neigh2);

        neigh2.AddNeighbor(neigh1, lengthTo1 + lengthTo2);
        neigh1.AddNeighbor(neigh2, lengthTo1 + lengthTo2);

        neigh1.RemoveNeighbor(current);
        neigh2.RemoveNeighbor(current);
    }
}
