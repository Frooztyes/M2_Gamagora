using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public string Name { get; }
    public List<NodeTime> Neighboors { get; }

    public Node(string name)
    {
        Neighboors = new List<NodeTime>();
        this.Name = name;
    }

    public struct NodeTime
    {
        public NodeTime(Node n, float length)
        {
            this.n = n;
            this.length = length;
        }

        public Node n { get; }
        public float length { get; }

        public override string ToString()
        {
            return $"{n.Name} ({length})";
        }
    };

    public void AddNeighboor(Node neighboor, float length)
    {
        Neighboors.Add(new NodeTime(neighboor, length));
    }

    public Node GetClosestNeighboor()
    {
        if (Neighboors.Count == 0) return null;
        NodeTime closest = Neighboors[0];
        foreach (NodeTime neighboor in Neighboors)
        {
            if(neighboor.length < closest.length)
            {
                closest = neighboor;
            }
        }
        return closest.n;
    }

    public override string ToString()
    {
        string s = $"Node ({Name}) - Voisins ({Neighboors.Count}): \n";
        foreach (NodeTime node in Neighboors)
        {
            s += "\t Node " + node;
        }
        return s + "\n";
    }
}
