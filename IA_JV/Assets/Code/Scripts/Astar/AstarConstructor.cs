using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AstarConstructor
{
    private VisualHandlers visualHandlers;
    public Astar AstarGraph { get; private set; }

    private Dictionary<Vector2, Transform> elements;
    private Vector2 offset = Vector2.one * 0.5f;

    private Dictionary<Transform, float> Voisins(Vector2 position)
    {
        Dictionary<Transform, float> list = new();

        if (elements.TryGetValue(position + Vector2.up, out Transform t))
            list.Add(t, 1);
        if (elements.TryGetValue(position + Vector2.down, out t))
            list.Add(t, 1);
        if (elements.TryGetValue(position + Vector2.right, out t))
            list.Add(t, 1);
        if (elements.TryGetValue(position + Vector2.left, out t))
            list.Add(t, 1);

        if (elements.TryGetValue(position + Vector2.left + Vector2.up, out t))
            list.Add(t, Mathf.Sqrt(2.0f));
        if (elements.TryGetValue(position + Vector2.left + Vector2.down, out t))
            list.Add(t, Mathf.Sqrt(2.0f));

        if (elements.TryGetValue(position + Vector2.right + Vector2.up, out t))
            list.Add(t, Mathf.Sqrt(2.0f));
        if (elements.TryGetValue(position + Vector2.right + Vector2.down, out t))
            list.Add(t, Mathf.Sqrt(2.0f));

        return list;
    }

    private Dictionary<Vector2, Transform> GetElements(List<GameObject> rawNodes)
    {
        Dictionary<Vector2, Transform> elements = new Dictionary<Vector2, Transform>();

        for (int i = 0; i < rawNodes.Count; ++i)
        {
            Vector2 pos = Vector2Int.FloorToInt(rawNodes[i].transform.position);

            if (!elements.ContainsKey(pos + offset))
            {
                rawNodes[i].transform.gameObject.SetActive(true);
                elements.Add(
                    pos + offset,
                    rawNodes[i].transform
                );
            }

        }

        return elements;
    }
    
    Astar CreateGraph()
    {
        Astar graph = new();

        foreach (var t in elements)
        {
            Node node = graph.AddNode(new Vector2(t.Key.x, t.Key.y));
            if (t.Value.CompareTag("AstarStart")) node.Status = Node.State.Start;
            if (t.Value.CompareTag("End")) node.Status = Node.State.End;
            if (t.Value.CompareTag("DeactivatedNode")) continue;
            Dictionary<Transform, float> neighboors = Voisins(node.Position);

            foreach (var no in neighboors)
            {
                Vector2 pos = new Vector2(
                    Mathf.Floor(no.Key.position.x),
                    Mathf.Floor(no.Key.position.y)
                );
                node.AddNeighbor(new Node(pos + offset), no.Value);
            }
            visualHandlers.AddAstarNode(node);
        }
        return graph;
    }

    public Transform UpdateEndPosition(Vector3 position, Vector3 size)
    {
        GameObject end = GameObject.FindGameObjectWithTag("End");
        if (end != null)
        {
            end.tag = "AstarNodes";
        }

        if (elements.TryGetValue(Vector2Int.FloorToInt(position) + offset, out Transform t))
        {
            t.gameObject.tag = "End";
        }
        return t;
    }

    public AstarConstructor(List<GameObject> nodes, VisualHandlers vh)
    {
        visualHandlers = vh;
        
        elements = GetElements(nodes);

        AstarGraph = CreateGraph();
    }

    protected bool IsInWaypoint(Vector2 ennemy, Vector2 waypoint)
    {
        Vector2 dir = ennemy - waypoint;
        return Mathf.Abs(dir.x) < 0.05f && Mathf.Abs(dir.y) < 0.05f;
    }

    public bool CalculateNextWaypoint(BirdController birdController, Transform end, out Vector2 dir)
    {
        dir = Vector2.zero;

        Vector2 positionEnnemy = birdController.transform.position;

        if (elements.TryGetValue(Vector2Int.FloorToInt(positionEnnemy) + offset, out Transform t))
        {
            t.gameObject.tag = "AstarStart";
            birdController.PositionInGraph = t.gameObject;
        }

        GameObject posInGraph = birdController.PositionInGraph;
        if (posInGraph == null)
        {
            return false;
        }

        Vector2 from = (Vector2)posInGraph.transform.position;
        Vector2 to = (Vector2)end.position;
        Node fromNode = AstarGraph.GetNodeByPosition(from);
        Node toNode = AstarGraph.GetNodeByPosition(to);
        if (fromNode == null || toNode == null)
            return false;

        Vector2 ennemyPos = (Vector2)birdController.transform.position;

        List<Node> localPath = AstarGraph.AStar(fromNode, toNode);
        if (localPath.Count < 1)
        {
            birdController.lastPath = null;
            return false;
        } else if(localPath.Count < 2 && IsInWaypoint(ennemyPos, localPath[0].Position)) {
            birdController.lastPath = null;
            return false;
        }

        List<Node> lastPath = birdController.lastPath;
        Vector2 gotoPosition;


        if (lastPath != null && IsInWaypoint(ennemyPos, lastPath[1].Position))
        {
            // si on est pile sur le noeud, on va sur le prochain de la liste, 
            // c-à-d le prochain de localPath
            birdController.lastPath = localPath;
        }

        if (lastPath != null && lastPath.Count > 1 && lastPath[1].Position == localPath[0].Position)
        {
            // si on est sur le noeud et que le noeud du joueur est le meme
            // localPath[0] est égal à la position de l'ennemi
            // dans ce cas là le premier noeud de notre chemin est le même que le deuxième de notre ancien chemin
            gotoPosition = lastPath[1].Position;
        }
        else
        {
            // on se rend au deuxième noeud de notre graphe
            // car on n'est pas sur la position
            if(localPath.Count >= 2)
            {
                gotoPosition = localPath[1].Position;
            } 
            else
            {
                birdController.lastPath = null;
                return false;
            }
        }
        if (lastPath == null)
            birdController.lastPath = localPath;

        visualHandlers.PrintAstarPath(fromNode, toNode, localPath);
        dir = gotoPosition;
        return true;
    }




}
