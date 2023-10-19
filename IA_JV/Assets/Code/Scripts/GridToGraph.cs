using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GridToGraph : MonoBehaviour
{
    [SerializeField] private Transform Grid;

    Dictionary<Vector2, Transform> elements;
    List<Node> path;

    string PositionToName(Vector2 pos)
    {
        return $"{pos.x},{pos.y}";
    }

    Graph CreateGraph()
    {
        Graph graph = new Graph();

        foreach (var t in elements)
        {
            Node node = graph.AddNode(PositionToName(t.Key));

            List<Transform> neighboors = Voisins(new Vector2(t.Key.x, t.Key.y));

            foreach (Transform no in neighboors)
            {
                node.AddNeighboor(new Node(PositionToName(no.position)), 1);
            }
        }

        Debug.Log(graph);
        return graph;
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

    [Range(0, 5)]
    public float noise = 5f;

    private Dictionary<Vector2, Transform> GetElements()
    {
        Dictionary<Vector2, Transform> elements = new Dictionary<Vector2, Transform>();
        int children = Grid.childCount;
        for (int i = 0; i < children; ++i)
        {
            if (!elements.ContainsKey(Grid.GetChild(i).position))
            {
                float sample = Mathf.PerlinNoise(
                    Grid.GetChild(i).position.x*1f/ noise,
                    Grid.GetChild(i).position.y*1f/ noise
                );
                Debug.Log(sample);
                if(sample < 0.5)
                {
                    Grid.GetChild(i).gameObject.SetActive(false);
                    continue;
                }


                Grid.GetChild(i).gameObject.SetActive(true);
                elements.Add(
                    Grid.GetChild(i).position,
                    Grid.GetChild(i)
                );
                Grid.GetChild(i).name = PositionToName(Grid.GetChild(i).position);
            }
        }

        if (this.elements == elements) return this.elements;
        return elements;
    }


    private void Update()
    {
        elements = GetElements();

        Graph graph = CreateGraph();

        Transform start = GameObject.FindGameObjectWithTag("Start").transform;
        if (start == null) return;
        start.gameObject.GetComponent<SpriteRenderer>().color = Color.green;

        Transform end = GameObject.FindGameObjectWithTag("End").transform;
        if (end == null) return;
        end.gameObject.GetComponent<SpriteRenderer>().color = Color.red;

        path = graph.GetPath2(
            graph.GetNodeByName(PositionToName(start.position)),
            graph.GetNodeByName(PositionToName(end.position))
        );
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
