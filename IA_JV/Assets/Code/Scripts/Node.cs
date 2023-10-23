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
        Neighbors.Add(n, cost);
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

    public int GetLengthTo(Node neigh)
    {
        foreach(var neighborNode in Neighbors)
        {
            if(neighborNode.Key.Position == neigh.Position) return (int) neighborNode.Value;
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
        return "";
        //string s = $"Node ({Name}) - Voisins ({Neighboors.Count}): \n";
        //foreach (var node in Neighboors)
        //{
        //    s += "\t Node " + node.Key.Name;
        //}
        //return s + "\n";
    }
}
