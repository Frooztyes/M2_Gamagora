using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
    public Vector2 Position { get; }

    public Dictionary<Node, float> Neighbors { get; }

    public State Status { get; set; }

    public enum State
    {
        Present,
        InDeletion,
        Start,
        End
    }

    public Node(Vector2 pos)
    {
        Neighbors = new Dictionary<Node, float>();
        Position = pos;
        Status = State.Present;
    }

    public void AddNeighbor(Node n, float cost)
    {
        if (!Neighbors.ContainsKey(n))
        {
            Neighbors.Add(n, cost);
        }
    }

    public void RemoveNeighbor(Node n)
    {
        Neighbors.Remove(Neighbors.Where(z => z.Key.Position == n.Position).FirstOrDefault().Key);
    }

    public Node GetClosestNeighbor()
    {
        if (Neighbors.Count == 0) return null;
        var closest = Neighbors.First();
        foreach (var neighboor in Neighbors)
        {
            if(neighboor.Value < closest.Value)
            {
                closest = neighboor;
            }
        }
        return closest.Key;
    }

    public float GetLengthTo(Node neigh)
    {
        foreach(var neighborNode in Neighbors)
        {
            if(neighborNode.Key.Position == neigh.Position) return neighborNode.Value;
        }
        return 0;
    }

    public List<Node> GetNeighboors()
    {
        List<Node> n = new();
        foreach (var nodes in Neighbors)
        {
            n.Add(nodes.Key);
        }
        return n;
    }

    public override string ToString()
    {
        string s = $"Node ({Position}) - Voisins ({Neighbors.Count}): \n";
        foreach (var node in Neighbors)
        {
            s += "\t Node " + node.Key.Position + " -> " + node.Value;
        }
        return s + "\n";
    }
}
