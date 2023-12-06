using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Astar : Graph
{
    Node LowestFScore(List<Node> openSet, Dictionary<Node, float> fScore)
    {
        if (fScore.Count == 0) return null;

        // get only element of fScore in openSet
        Dictionary<Node, float> localScore = new Dictionary<Node, float>();
        foreach (Node node in openSet)
        {
            localScore.Add(node, fScore[node]);
        }

        if (localScore.Count == 0) return null;


        Node minNode = localScore.ElementAt(0).Key;
        float minValue = localScore.ElementAt(0).Value;

        foreach (var score in localScore)
        {
            if (!openSet.Contains(score.Key)) continue;
            if (score.Value <= minValue)
            {
                minNode = score.Key;
                minValue = score.Value;
            }
        }

        return minNode;
    }

    List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        List<Node> totalPath = new List<Node>
        {
            current
        };

        while (cameFrom.Keys.Contains(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }

        return totalPath;
    }

    float Heuristic(Node n1, Node n2)
    {
        return Vector2.Distance(n1.Position, n2.Position);
    }

    public List<Node> AStar(Node start, Node goal)
    {
        List<Node> openSet = new List<Node>();

        // from -> to : on ne vient que d'un endroit donc dictionnaire fonctionnel;
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, float> gScore = new Dictionary<Node, float>();
        Dictionary<Node, float> fScore = new Dictionary<Node, float>();

        openSet.Add(start);

        for (int i = 0; i < Nodes.Count; i++)
        {
            gScore.Add(Nodes.ElementAt(i).Value, float.PositiveInfinity);
            fScore.Add(Nodes.ElementAt(i).Value, float.PositiveInfinity);
        }
        gScore[start] = 0;
        fScore[start] = Heuristic(start, goal);


        while (openSet.Count > 0)
        {
            Node current = LowestFScore(openSet, fScore);
            if (current == goal)
            {
                // reconstruire le chemin
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);

            foreach (Node neighbor in current.GetNeighboors())
            {
                Node neighReal = GetNodeByPosition(neighbor.Position);


                // g = current.GetLengthTo(neighReal)
                float tentativeScore = gScore[current] + current.GetLengthTo(neighReal);
                if (tentativeScore < gScore[neighReal])
                {
                    cameFrom[neighReal] = current;
                    gScore[neighReal] = tentativeScore;
                    fScore[neighReal] = tentativeScore + Heuristic(neighReal, goal);
                    if (!openSet.Contains(neighReal))
                    {
                        openSet.Add(neighReal);
                    }
                }
            }

        }

        return null;

    }
}
