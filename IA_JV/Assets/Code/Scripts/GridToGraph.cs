using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GridToGraph : MonoBehaviour
{
    [SerializeField] private Transform Grid;
    [SerializeField] private GameObject graphNode;

    Dictionary<Vector2, Transform> elements;
    List<List<Node>> paths;

    Vector2 offset = Vector2.one * 0.5f;

    Graph CreateGraph()
    {
        Graph graph = new Graph();

        foreach (var t in elements)
        {
            Node node = graph.AddNode(new Vector2(t.Key.x, t.Key.y));
            if (t.Value.CompareTag("Start")) node.Status = Node.State.Start;
            if (t.Value.CompareTag("End")) node.Status = Node.State.End;
            if (t.Value.CompareTag("DeactivatedNode")) continue;
            List<Transform> neighboors = Voisins(node.Position);

            foreach (Transform no in neighboors)
            {
                node.AddNeighbor(new Node(no.position), 1);
            }
        }

        List<Node> toRemove = new List<Node>();
        foreach(Node node in graph.Nodes.Values)
        {
            Vector2 dir = Vector2.zero;

            if(IsNotNecessary(node.Position) == 1)
            {
                dir = Vector2.up;
            }
            if (IsNotNecessary(node.Position) == 2)
            {
                dir = Vector2.right;
            }

            if (dir == Vector2.zero) continue;
            if (node.Status == Node.State.Start) continue;
            if (node.Status == Node.State.End) continue;

            Node neigh1 = GetClosestNodeNotInDeletion(graph, dir, node.Position);
            if (neigh1 == null) continue;
            Node neigh2 = GetClosestNodeNotInDeletion(graph, -dir, node.Position);
            if (neigh2 == null) continue;

            graph.RecalculateLength(node, neigh1, neigh2);
            toRemove.Add(node);
            node.Status = Node.State.InDeletion;
        }

        foreach(Node n in toRemove)
        {
            graph.RemoveNode(n);
        }
        return graph;
    }

    Node GetClosestNodeNotInDeletion(Graph graph, Vector2 direction, Vector2 position)
    {
        int id = 1;
        Node found;
        while (true)
        {
            found = graph.GetNodeByVector(position + direction * id);
            if (found != null && found.Status != Node.State.InDeletion)
                return found;
            if (id > 10) return null;
            id++;
        }
    }

    int IsNotNecessary(Vector2 position)
    {
        bool a = elements.TryGetValue(position + Vector2.up, out _);
        bool b = elements.TryGetValue(position + Vector2.down, out _);

        bool c = elements.TryGetValue(position + Vector2.left, out _);
        bool d = elements.TryGetValue(position + Vector2.right, out _);

        if( (a && b) ^ (c && d))
        {
            if (a && b && !c && !d) return 1;
            if (c && d && !a && !b) return 2;
        } 
        return 0;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(graph == null) return;
        Color defaultColor = Color.grey;
        float thickness = 3.0f;
        Handles.color = defaultColor;
        foreach (Node node in graph.Nodes.Values)
        {
            Handles.color = defaultColor;
            if (!elements.TryGetValue(node.Position, out Transform t)) continue;


            foreach (var neighbor in node.Neighbors)
            {
                Handles.DrawLine(
                    node.Position,
                    neighbor.Key.Position,
                    thickness
                );
            }

            if (t.CompareTag("Start"))
            {
                Handles.color = Color.green;
            }
            else if (t.CompareTag("End"))
            {
                Handles.color = Color.red;
            }
            else
            {
                Handles.color = defaultColor;
            }

            if (t.CompareTag("DeactivatedNode")) continue;
            Handles.DrawSolidDisc(
                node.Position, 
                -Vector3.forward, 
                0.2f
            );

        }

        Handles.color = defaultColor;

        if (paths == null) return;
        if (paths.Count == 0) return;

        foreach (List<Node> nodes in paths)
        {
            foreach (Node node in nodes)
            {
                foreach (var neighbor in node.Neighbors)
                {
                    if (nodes.Contains(neighbor.Key)) Handles.color = Color.blue;
                    else Handles.color = defaultColor;

                    Handles.DrawLine(
                        node.Position,
                        neighbor.Key.Position,
                        thickness
                    );
                }
            }
        }

    }
#endif

    private List<Transform> Voisins(Vector2 position)
    {
        List<Transform> list = new();

        if (elements.TryGetValue(position + Vector2.up, out Transform t))
            list.Add(t);
        if (elements.TryGetValue(position + Vector2.down, out t))
            list.Add(t);
        if (elements.TryGetValue(position + Vector2.right, out t))
            list.Add(t);
        if (elements.TryGetValue(position + Vector2.left, out t))
            list.Add(t);

        return list;
    }

    private Dictionary<Vector2, Transform> GetElements()
    {
        Dictionary<Vector2, Transform> elements = new Dictionary<Vector2, Transform>();
        // récupérer selon le tag node

        GameObject[] start = GameObject.FindGameObjectsWithTag("Start");
        foreach(GameObject go in start)
        {
            go.tag = "Node";
        }

        GameObject[] end = GameObject.FindGameObjectsWithTag("End");
        foreach (GameObject go in end)
        {
            go.tag = "Node";
        }

        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        for (int i = 0; i < nodes.Length; ++i)
        {
            Vector2 pos = new Vector2(
                Mathf.Floor(nodes[i].transform.position.x), 
                Mathf.Floor(nodes[i].transform.position.y)
            );

            if (!elements.ContainsKey(pos + offset))
            {
                nodes[i].transform.gameObject.SetActive(true);
                elements.Add(
                    pos + offset,
                    nodes[i].transform
                );
            }
        }

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector2 posPlayerI = player.position;
        posPlayerI.y += player.GetComponent<SpriteRenderer>().bounds.size.y / 2;

        if (elements.TryGetValue(Vector2Int.FloorToInt(posPlayerI) + offset, out Transform t))
        {
            t.gameObject.tag = "Start";
        } 
        else
        {
        }
        return elements;
    }


    bool IsInWaypoint(Vector2 ennemy, Vector2 waypoint)
    {
        Vector2 dir = ennemy - waypoint;
        return Mathf.Abs(dir.x) < 0.05f && Mathf.Abs(dir.y) < 0.05f;
    }

    Vector2 GetHardDirection(Vector2 from, Vector2 to)
    {
        Vector2 dir = from - to;
        if (dir.x != 0 && Mathf.Abs(dir.x) > 0.01) dir.x = 1 * dir.x > 0 ? -1 : 1;
        else dir.x = 0;
        if (dir.y != 0 && Mathf.Abs(dir.y) > 0.02) dir.y = 1 * dir.y > 0 ? -1 : 1;
        else dir.y = 0;
        return dir;
    }


    GameObject start;
    GameObject[] end;
    Graph graph;
    GameObject[] ennemies;

    private GameObject[] SetupEnnemies()
    {
        GameObject[] ennemies = GameObject.FindGameObjectsWithTag("Ennemy");
        foreach (GameObject ennemy in ennemies)
        {
            Vector2 positionEnnemy = ennemy.transform.position;
            
            // résolution du problème d'ancrage
            positionEnnemy.y += ennemy.GetComponent<SpriteRenderer>().bounds.size.y / 2;

            // améliorer le calcul
            Transform t;
            if (elements.TryGetValue(Vector2Int.FloorToInt(positionEnnemy) + offset, out t))
            {
                t.gameObject.tag = "End";
                ennemy.GetComponent<EnnemyController>().PositionInGraph = t.gameObject;
            }
            else
            {
                // s'il n'est pas sur un noeud, on l'arrête
                t = CreateGraphNode(Vector2Int.FloorToInt(positionEnnemy) + offset).transform;
                ennemy.GetComponent<EnnemyController>().PositionInGraph = t.gameObject;
            }
        }
        return ennemies;
    }

    private GameObject CreateGraphNode(Vector3 position)
    {
        GameObject g = Instantiate(graphNode, position, Quaternion.identity);
        g.transform.parent = Grid.transform;
        return g;
    }

    private void CalculatePath()
    {
        foreach (GameObject ennemy in ennemies)
        {
            EnnemyController ennemyController = ennemy.GetComponent<EnnemyController>();
            GameObject posInGraph = ennemyController.PositionInGraph;
            
            if (posInGraph == null)
            {
                ennemyController.WaypointToGo = Vector2.zero;
                continue;
            }


            Vector2 from = (Vector2)posInGraph.transform.position;
            Vector2 to = (Vector2)start.transform.position;
            Node fromNode = graph.GetNodeByPosition(from);
            Node toNode = graph.GetNodeByPosition(to);
            if (fromNode == null || toNode == null) continue;

            List<Node> localPath = graph.GetPath(fromNode, toNode);

            List<Node> lastPath = ennemyController.lastPath;
            Vector2 gotoPosition;

            Vector2 ennemyPos = (Vector2)ennemy.transform.position;


            if (lastPath != null && IsInWaypoint(ennemyPos + 0.5f * Vector2.up, lastPath[1].Position))
            {
                // si on est pile sur le noeud, on va sur le prochain de la liste, 
                // c-à-d le prochain de localPath
                ennemyController.lastPath = localPath;
            }

            if (lastPath != null && lastPath[1].Position == localPath[0].Position)
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
                gotoPosition = localPath[1].Position;
            }
            if (lastPath == null)
                ennemyController.lastPath = localPath;

            ennemyController.WaypointToGo = gotoPosition;
            paths.Add(localPath);
        }
    }

    private void Update()
    {
        //RemoveTemporary();
        // TODO: optimiser récupération des noeuds
        elements = GetElements();

        // TODO: optimiser le positionnement des ennemis
        ennemies = SetupEnnemies();
        
        // TODO: modifier l'appel à cette fonction
        graph = CreateGraph();


        start = GameObject.FindGameObjectWithTag("Start");
        if (start == null)
            return;

        end = GameObject.FindGameObjectsWithTag("End");
        if (end == null)
            return;


        paths = new List<List<Node>>();

        CalculatePath();
    }


    
}
