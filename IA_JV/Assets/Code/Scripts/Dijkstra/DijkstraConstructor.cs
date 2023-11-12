using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class DijkstraConstructor
{
    private VisualHandlers visualHandlers;

    Dictionary<Vector2, Node> elements;
    public Dijkstra DijkstraGraph { get; private set; }

    Vector2 offset = Vector2.one * 0.5f;

    private static DijkstraConstructor instance = null;

    public static DijkstraConstructor Instance
    {
        get
        {
            instance ??= new DijkstraConstructor(GameObject.FindGameObjectWithTag("VisualHandlers").GetComponent<VisualHandlers>());
            return instance;
        }
    }

    public void AddNodeAutoCheckNeighbor(Vector2 position)
    {
        Node node = DijkstraGraph.AddNode(position);

        List<Node> neighboors = Voisins(position);

        foreach (Node no in neighboors)
        {
            node.AddNeighbor(no, 1);
            no.AddNeighbor(node, 1);
        }

        elements.TryAdd(position, node);
    }


    private DijkstraConstructor(VisualHandlers vh)
    {
        visualHandlers = vh;
        elements = new Dictionary<Vector2, Node>();
        DijkstraGraph = new Dijkstra();
    }

    public void UpdateVisuals()
    {
        visualHandlers.RemoveDijkstraNodes();
        foreach (var n in DijkstraGraph.Nodes)
        {
            visualHandlers.AddDijkstraNode(n.Value);
        }
    }

    private Dictionary<Vector2, Transform> GetElements()
    {
        Dictionary<Vector2, Transform> elements = new Dictionary<Vector2, Transform>();
        // récupérer selon le tag node

        GameObject[] start = GameObject.FindGameObjectsWithTag("Start");
        foreach (GameObject go in start)
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


    private List<Node> Voisins(Vector2 position)
    {
        List<Node> list = new();

        if (elements.TryGetValue(position + Vector2.up, out Node n))
            list.Add(n);
        if (elements.TryGetValue(position + Vector2.down, out n))
            list.Add(n);
        if (elements.TryGetValue(position + Vector2.right, out n))
            list.Add(n);
        if (elements.TryGetValue(position + Vector2.left, out n))
            list.Add(n);

        return list;
    }

    //Dijkstra CreateGraph()
    //{
    //    visualHandlers.RemoveDijkstraNodes();
    //    Dijkstra graph = new();

    //    foreach (var t in elements)
    //    {
    //        Node node = graph.AddNode(new Vector2(t.Key.x, t.Key.y));
    //        if (t.Value.CompareTag("Start")) node.Status = Node.State.Start;
    //        if (t.Value.CompareTag("End")) node.Status = Node.State.End;
    //        if (t.Value.CompareTag("DeactivatedNode")) continue;
    //        List<Transform> neighboors = Voisins(node.Position);

    //        foreach (Transform no in neighboors)
    //        {
    //            node.AddNeighbor(new Node(no.position), 1);
    //        }
    //    }

    //    List<Node> toRemove = new();
    //    foreach (Node node in graph.Nodes.Values)
    //    {
    //        Vector2 dir = Vector2.zero;

    //        if (IsNotNecessary(node.Position) == 1)
    //        {
    //            dir = Vector2.up;
    //        }
    //        if (IsNotNecessary(node.Position) == 2)
    //        {
    //            dir = Vector2.right;
    //        }

    //        if (dir == Vector2.zero) continue;
    //        if (node.Status == Node.State.Start) continue;
    //        if (node.Status == Node.State.End) continue;

    //        Node neigh1 = GetClosestNodeNotInDeletion(graph, dir, node.Position);
    //        if (neigh1 == null) continue;
    //        Node neigh2 = GetClosestNodeNotInDeletion(graph, -dir, node.Position);
    //        if (neigh2 == null) continue;

    //        graph.RecalculateLength(node, neigh1, neigh2);
    //        toRemove.Add(node);
    //        node.Status = Node.State.InDeletion;
    //    }

    //    foreach (Node n in toRemove)
    //    {
    //        graph.RemoveNode(n);
    //    }
    //    foreach(var n in graph.Nodes)
    //    {
    //        visualHandlers.AddDijkstraNode(n.Value);
    //    }
    //    return graph;
    //}

    int IsNotNecessary(Vector2 position)
    {
        bool a = elements.TryGetValue(position + Vector2.up, out _);
        bool b = elements.TryGetValue(position + Vector2.down, out _);

        bool c = elements.TryGetValue(position + Vector2.left, out _);
        bool d = elements.TryGetValue(position + Vector2.right, out _);

        if ((a && b) ^ (c && d))
        {
            if (a && b && !c && !d) return 1;
            if (c && d && !a && !b) return 2;
        }
        return 0;
    }

    Node GetClosestNodeNotInDeletion(Graph graph, Vector2 direction, Vector2 position)
    {
        int id = 1;
        Node found;
        while (true)
        {
            found = graph.GetNodeByPosition(position + direction * id);
            if (found != null && found.Status != Node.State.InDeletion)
                return found;
            if (id > 10) return null;
            id++;
        }
    }

    public List<Node> SetupEnnemies(out List<EnnemyController> ennemies)
    {
        ennemies = new List<EnnemyController>();

        GameObject[] ennemiesGo = GameObject.FindGameObjectsWithTag("Ennemy");
        List<Node> ennemiesNode = new List<Node>();

        foreach (GameObject enemy in ennemiesGo)
        {
            EnnemyController ec = enemy.GetComponent<EnnemyController>();
            if (ec.IsDead) continue;
            Vector2 positionEnnemy = enemy.transform.position;

            // résolution du problème d'ancrage
            positionEnnemy.y += enemy.GetComponent<SpriteRenderer>().bounds.size.y / 2;

            // améliorer le calcul
            if (elements.TryGetValue(Vector2Int.FloorToInt(positionEnnemy) + offset, out Node n))
            {
                //t.gameObject.tag = "End";
                ec.PositionInGraph = n.Position;
                ennemies.Add(ec);
                ennemiesNode.Add(n);
            } 
            else
            {
                ec.PositionInGraph = null;
            }
        }
        return ennemiesNode;
    }

    public Node UpdateEndPosition(Vector3 position)
    {
        elements.TryGetValue(Vector2Int.FloorToInt(position) + offset, out Node n);
        return n;
    }

    bool IsInWaypoint(Vector2 ennemy, Vector2 waypoint)
    {
        Vector2 dir = ennemy - waypoint;
        return Mathf.Abs(dir.x) < 0.05f && Mathf.Abs(dir.y) < 0.05f;
    }

    public bool CalculateNextWaypoint(EnnemyController ennemyController, Vector2 end, out Vector2 dir, out List<Node> path)
    {
        Vector2? posInGraph = ennemyController.PositionInGraph;
        dir = Vector2.zero;
        path = new List<Node>();

        if (posInGraph == null)
        {
            return false;
        }


        Vector2 from = (Vector2)posInGraph;
        Vector2 to = end;
        Node fromNode = DijkstraGraph.GetNodeByPosition(from);
        Node toNode = DijkstraGraph.GetNodeByPosition(to);
        if (fromNode == null || toNode == null) return false;
        List<Node> localPath = DijkstraGraph.GetPath(fromNode, toNode);
        List<Node> lastPath = ennemyController.lastPath;
        Vector2 gotoPosition;

        Vector2 ennemyPos = (Vector2)ennemyController.transform.position;

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

            if (localPath.Count >= 2)
            {
                gotoPosition = localPath[1].Position;
            }
            else
            {
                ennemyController.lastPath = null;
            }
        }
        
        if (lastPath == null)
            ennemyController.lastPath = localPath;

        dir = gotoPosition;
        path = localPath;
        return true;
    }
}
