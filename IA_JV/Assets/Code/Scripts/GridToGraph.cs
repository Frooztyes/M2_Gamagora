using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GridToGraph : MonoBehaviour
{
    [SerializeField] private Transform Grid;
    [SerializeField] private GameObject block;

    Dictionary<Vector2, Transform> elements;
    List<List<Node>> paths;

    string PositionToName(Vector2 pos)
    {
        return $"{Mathf.Floor(pos.x)},{Mathf.Floor(pos.y)}";
    }

    Vector3 NameToPosition(string name)
    {
        string[] pos = name.Split(",");
        float x = Mathf.Floor(float.Parse(pos[0]));
        float y = Mathf.Floor(float.Parse(pos[1]));
        return new Vector3(x, y, 0);
    }


    Graph CreateGraph()
    {
        Graph graph = new Graph();

        foreach (var t in elements)
        {
            Node node = graph.AddNode(PositionToName(t.Key));
            if (t.Value.CompareTag("Start")) node.Status = Node.State.Start;
            if (t.Value.CompareTag("End")) node.Status = Node.State.End;
            if (t.Value.CompareTag("DeactivatedNode")) continue;

            List<Transform> neighboors = Voisins(new Vector2(t.Key.x, t.Key.y));

            foreach (Transform no in neighboors)
            {
                node.AddNeighboor(new Node(PositionToName(no.position)), 1);
            }
        }

        List<Node> toRemove = new List<Node>();
        foreach(Node node in graph.Nodes)
        {
            string[] pos = node.Name.Split(",");
            float x = Mathf.Floor(float.Parse(pos[0]));
            float y = Mathf.Floor(float.Parse(pos[1]));

            Vector2 dir = Vector2.zero;

            if(IsNotNecessary(new Vector2(x, y)) == 1)
            {
                dir = Vector2.up;
            }
            if (IsNotNecessary(new Vector2(x, y)) == 2)
            {
                dir = Vector2.right;
            }

            if (dir == Vector2.zero) continue;
            if (node.Status == Node.State.Start) continue;
            if (node.Status == Node.State.End) continue;

            Node neigh1 = GetClosestNodeNotInDeletion(graph, dir, new Vector2(x, y));
            if (neigh1 == null) continue;
            Node neigh2 = GetClosestNodeNotInDeletion(graph, -dir, new Vector2(x, y));
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
            if (id > 100) return null;
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
        Vector3 offset = Vector3.one * 0.5f;
        offset.z = 0;
        if(graph == null) return;
        Color defaultColor = Color.grey;
        float thickness = 3.0f;
        Handles.color = defaultColor;
        foreach (Node node in graph.Nodes)
        {
            if (!elements.TryGetValue(NameToPosition(node.Name), out Transform t)) continue;

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
                NameToPosition(node.Name) + offset, 
                -Vector3.forward, 
                0.2f
            );

            foreach(var neighbor in node.Neighboors)
            {
                Handles.DrawLine(
                    NameToPosition(node.Name) + offset, 
                    NameToPosition(neighbor.Key.Name) + offset, 
                    thickness
                );
            }

        }
        if (paths.Count == 0) return;

        foreach (List<Node> nodes in paths)
        {
            foreach(Node node in nodes)
            {
                foreach (var neighbor in node.Neighboors)
                {
                    if (nodes.Contains(neighbor.Key)) Handles.color = Color.blue;
                    else Handles.color = defaultColor;
                    Handles.DrawLine(
                        NameToPosition(node.Name) + offset,
                        NameToPosition(neighbor.Key.Name) + offset,
                        thickness
                    );
                }
            }
        }       

    }
#endif

    private Dictionary<Vector2, Transform> GetElements()
    {
        Dictionary<Vector2, Transform> elements = new Dictionary<Vector2, Transform>();
        int children = Grid.childCount;
        for (int i = 0; i < children; ++i)
        {
            Vector2 pos = new(Mathf.Floor(Grid.GetChild(i).position.x), Mathf.Floor(Grid.GetChild(i).position.y));
            if (!elements.ContainsKey(pos))
            {
                Grid.GetChild(i).gameObject.SetActive(true);
                //Grid.GetChild(i).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                elements.Add(
                    pos,
                    Grid.GetChild(i)
                );
                Grid.GetChild(i).name = PositionToName(Grid.GetChild(i).position);
                if(Grid.GetChild(i).CompareTag("Start") || Grid.GetChild(i).CompareTag("End"))
                {
                    Grid.GetChild(i).tag = "Node";
                }
                //Grid.GetChild(i).tag = "Untagged";
            }
        }

        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector2 posPlayerI = player.position;
        posPlayerI.y += player.GetComponent<SpriteRenderer>().bounds.size.y / 2;

        Vector2 posPlayer = new(Mathf.Floor(posPlayerI.x), Mathf.Floor(posPlayerI.y));
        if (elements.TryGetValue(posPlayer, out Transform t))
        {
            t.gameObject.tag = "Start";
        } else
        {
            //Debug.Log("Not");
        }

        //if (!elements.ContainsKey(posPlayer))
        //{
        //     instantier place perso à la position +1 et retirer à chaque boucle si pas le meme

        //    start = Instantiate(block, player.transform.position, Quaternion.identity);
        //    elements.Add(
        //        posPlayer,
        //        player
        //    );
        //}
        return elements;
    }


    bool IsInWaypoint(Vector2 ennemy, Vector2 waypoint)
    {
        Vector2 dir = ennemy - waypoint;
        return Mathf.Abs(dir.x) < 0.05f && Mathf.Abs(dir.y) < 0.15f;
    }

    Vector2 GetDirection(Vector2 from, Vector2 to)
    {
        Vector2 dir = from - to;
        if (dir.x != 0 && Mathf.Abs(dir.x) > 0.01) dir.x = 1 * dir.x > 0 ? 1 : -1;
        else dir.x = 0;
        if (dir.y != 0 && Mathf.Abs(dir.y) > 0.02) dir.y = 1 * dir.y > 0 ? 1 : -1;
        else dir.y = 0;
        return dir;
    }

    GameObject start;
    GameObject[] end;
    Graph graph;
    GameObject[] ennemies;
    private void Update()
    {
        float opacity = 0f;
        elements = GetElements();


        ennemies = GameObject.FindGameObjectsWithTag("Ennemy");

        foreach (GameObject ennemy in ennemies)
        {
            Vector2 posI = ennemy.transform.position;
            posI.y += ennemy.GetComponent<SpriteRenderer>().bounds.size.y / 2;

            Vector2 pos = new(Mathf.Floor(posI.x), Mathf.Floor(posI.y));

            if (elements.TryGetValue(pos, out Transform t))
            {
                t.gameObject.tag = "End";
                ennemy.GetComponent<EnnemyController>().PositionInGraph = t.gameObject;
            } else
            {
                ennemy.GetComponent<EnnemyController>().Stop();
            }
        }


        int children = Grid.childCount;
        for (int i = 0; i < children; ++i)
        {
            Grid.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, opacity);
        }

        graph = CreateGraph();
        start = GameObject.FindGameObjectWithTag("Start");
        if (start == null)
            return;

        end = GameObject.FindGameObjectsWithTag("End");
        if (end == null)
            return;


        paths = new List<List<Node>>();

        foreach (GameObject ennemy in ennemies)
        {
            EnnemyController ennemyController = ennemy.GetComponent<EnnemyController>();
            GameObject posInGraph = ennemyController.PositionInGraph;
            Debug.Log(posInGraph);
            if (posInGraph == null)
            {
                ennemyController.WaypointToGo = Vector2.zero;
                continue;
            }

            List<Node> localPath = graph.GetPath2(
                graph.GetNodeByName(PositionToName(posInGraph.transform.position)),
                graph.GetNodeByName(PositionToName(start.transform.position))
            );

            if(localPath.Count <= 1)
            {
                ennemyController.WaypointToGo = Vector2.zero;
                continue;
            }

            Vector2 offset = Vector2.one * 0.5f;


            Vector2 ennemyPos = (Vector2) ennemy.transform.position + offset * Vector2.up;
            Vector2 firstWP = (Vector2)NameToPosition(localPath[0].Name) + offset;
            Vector2 secondWP = (Vector2)NameToPosition(localPath[1].Name) + offset;

            Vector2 dirToFirstWP = GetDirection(ennemyPos, firstWP); 
            Vector2 dirToSecondWP = GetDirection(ennemyPos, secondWP);

            int nextWP;
            if (IsInWaypoint(ennemyPos, firstWP))
            {
                nextWP = 1; 
            } 
            else
            {
                if (dirToFirstWP.x == dirToSecondWP.x)
                {
                    if(dirToFirstWP.y == dirToSecondWP.y)
                    {
                        nextWP = 1;
                    } 
                    else
                    {
                        nextWP = 0;
                    }
                }
                else
                {
                    if (dirToFirstWP.y == dirToSecondWP.y && dirToFirstWP.y != 0)
                    {
                        nextWP = 0;
                    }
                    else
                    {
                        nextWP = 1;
                    }
                }
            }

            Vector2 dirInt = Vector2.zero;
            if (nextWP == 1)
            {
                dirInt =
                    (Vector2)NameToPosition(PositionToName((Vector2) posInGraph.transform.position))
                    -
                    (Vector2)NameToPosition(localPath[nextWP].Name);
                dirInt.Normalize();
            }

            if(nextWP == 0)
            {
                dirInt = (
                    (Vector2)NameToPosition(localPath[nextWP].Name) + offset
                    -
                    ((Vector2)ennemy.transform.position + Vector2.up * offset)
                );
                dirInt.x *= -1;
                dirInt.y *= dirToSecondWP.y;
                dirInt.Normalize();
            }

            ennemyController.WaypointToGo = dirInt;
            paths.Add(localPath);
        }
    }


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
}
