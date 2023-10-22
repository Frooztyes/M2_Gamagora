using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{
    public string Name { get; }

    public Dictionary<Node, float> Neighboors { get; }

    public State Status { get; set; }

    public enum State
    {
        Present,
        InDeletion,
        Start,
        End
    }

    public Node(string name)
    {
        Neighboors = new Dictionary<Node, float>();
        Name = name;
        Status = State.Present;
    }

    public void AddNeighboor(Node n, float cost)
    {
        Neighboors.Add(n, cost);
    }

    public Node GetClosestNeighboor()
    {
        if (Neighboors.Count == 0) return null;
        var closest = Neighboors.First();
        foreach (var neighboor in Neighboors)
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
        foreach(var no in Neighboors)
        {
            if(no.Key.Name == neigh.Name) return (int) no.Value;
        }
        return 0;
    }

    public List<Node> GetNeighboors()
    {
        List<Node> n = new();
        foreach (var nodes in Neighboors)
        {
            n.Add(nodes.Key);
        }
        return n;
    }

    public override string ToString()
    {
        string s = $"Node ({Name}) - Voisins ({Neighboors.Count}): \n";
        foreach (var node in Neighboors)
        {
            s += "\t Node " + node.Key.Name;
        }
        return s + "\n";
    }
}
