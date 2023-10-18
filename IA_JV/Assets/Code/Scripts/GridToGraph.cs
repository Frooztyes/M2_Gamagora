using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridToGraph : MonoBehaviour
{
    [SerializeField] private Transform Grid;

    Dictionary<Vector2, Transform> elements;
    List<Node> path;

    // Start is called before the first frame update
    void Start()
    { 
        elements = new Dictionary<Vector2, Transform>();
        int children = Grid.childCount;
        for (int i = 0; i < children; ++i)
        {
            if(!elements.ContainsKey((Vector2)Grid.GetChild(i).position))
            {
                elements.Add(
                    (Vector2)Grid.GetChild(i).position,
                    Grid.GetChild(i)
                );
                Grid.GetChild(i).name = $"{Grid.GetChild(i).position.x},{Grid.GetChild(i).position.y}";
            } else
            {
                Destroy(Grid.GetChild(i).gameObject);
            }
        }

        Graph graph = new Graph();

        foreach (var t in elements)
        {
            string name = $"{t.Key.x},{t.Key.y}";
            Node node = graph.AddNode(name);

            List<Transform> neighboors = Voisins(new Vector2(t.Key.x, t.Key.y));

            foreach (Transform no in neighboors)
            {
                name = $"{no.position.x},{no.position.y}";
                node.AddNeighboor(new Node(name), 1);
            }
        }

        Debug.Log(graph);

        Transform start = GameObject.FindGameObjectWithTag("Start").transform;
        if (start == null) return;
        start.gameObject.GetComponent<SpriteRenderer>().color = Color.green;

        Transform end = GameObject.FindGameObjectWithTag("End").transform;
        if (end == null) return;
        end.gameObject.GetComponent<SpriteRenderer>().color = Color.red;

        string fromName = $"{start.position.x},{start.position.y}";
        string toName = $"{end.position.x},{end.position.y}";;

        path = graph.GetPath2(
            graph.GetNodeByName(fromName),
            graph.GetNodeByName(toName)
        );

        //foreach (Node node in path)
        //{
        //    Debug.Log(node.Name);
        //}

    }

    private void OnDrawGizmos()
    {
        int children = Grid.childCount;
        for (int i = 0; i < children; ++i)
        {
            Grid.GetChild(i).GetComponent<SpriteRenderer>().color = Color.white;
        }

        if(path != null)
        {
            foreach (Node node in path)
            {
                string[] pos = node.Name.Split(",");
                float x = float.Parse(pos[0]);
                float y = float.Parse(pos[1]);
                if (elements.TryGetValue(new Vector2(x, y), out Transform t))
                    t.GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }

        GameObject start = GameObject.FindGameObjectWithTag("Start");
        if (start != null)
        {
            start.GetComponent<SpriteRenderer>().color = Color.green;
        }

        GameObject end = GameObject.FindGameObjectWithTag("End");
        if (end != null)
        {
            end.GetComponent<SpriteRenderer>().color = Color.red;
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
