using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class VisualHandlers : MonoBehaviour
{
    [SerializeField] private GameObject visualNode;

    private Transform dijkstraVisuals;
    private Transform astarVisuals;

    private void Start()
    {
        dijkstraVisuals = transform.Find("Dijkstra");
        astarVisuals = transform.Find("A*");
        aStarGraphics = new Dictionary<Vector2, GraphicNode>();
        dijkstraGraphics = new Dictionary<Vector2, GraphicNode>();
    }

    public void RemoveDijkstraNodes()
    {
        foreach (Transform child in dijkstraVisuals)
        {
            Destroy(child.gameObject);
        }
        dijkstraGraphics = new Dictionary<Vector2, GraphicNode>();
    }

    public struct GraphicNode
    {
        public VisualNode vn;

        public Dictionary<Node, LineRenderer> neigh;

        public GraphicNode(VisualNode vn, Dictionary<Node, LineRenderer> lines)
        {
            neigh = lines;
            this.vn = vn;
        }
    }

    Dictionary<Vector2, GraphicNode> aStarGraphics;
    Dictionary<Vector2, GraphicNode> dijkstraGraphics;

    public void ResetAstarVisuals()
    {
        foreach (var n in aStarGraphics)
        {
            n.Value.vn.SetNode(VisualNode.State.INACTIVE);
            foreach (var neigh in n.Value.neigh)
            {
                n.Value.vn.SetLineColor(VisualNode.State.INACTIVE, neigh.Value);
            }
        }
    }
    public void ResetDijkstraVisuals()
    {
        foreach (var n in dijkstraGraphics)
        {
            n.Value.vn.SetNode(VisualNode.State.INACTIVE);
            foreach (var neigh in n.Value.neigh)
            {
                n.Value.vn.SetLineColor(VisualNode.State.INACTIVE, neigh.Value);
            }
        }
    }

    public void PrintAstarPath(Node start, Node end, List<Node> path)
    {
        ResetAstarVisuals();

        foreach (Node node in path)
        {
            GraphicNode gn = aStarGraphics[node.Position];
            gn.vn.SetNode(VisualNode.State.PATH);
            foreach(var neigh in gn.neigh)
            {
                int index = path.FindIndex(item => item.Position == neigh.Key.Position);
                if(index >= 0)
                    gn.vn.SetLineColor(VisualNode.State.PATH, neigh.Value);
            }
        }

        aStarGraphics[start.Position].vn.SetNode(VisualNode.State.START);
        aStarGraphics[end.Position].vn.SetNode(VisualNode.State.END);
    }

    public void PrintDijkstraPath(List<Node> starts, Node end, List<Node> path)
    {
        ResetDijkstraVisuals();

        foreach (Node node in path)
        {
            GraphicNode gn = dijkstraGraphics[node.Position];
            gn.vn.SetNode(VisualNode.State.PATH);
            foreach (var neigh in gn.neigh)
            {
                int index = path.FindIndex(item => item.Position == neigh.Key.Position);
                if (index >= 0)
                    gn.vn.SetLineColor(VisualNode.State.PATH, neigh.Value);
            }
        }

        foreach (Node start in starts)
        {
            dijkstraGraphics[start.Position].vn.SetNode(VisualNode.State.START);
        }

        dijkstraGraphics[end.Position].vn.SetNode(VisualNode.State.END);
    }


    private void AddNode(int type, Node n, float scale)
    {
        VisualNode vn = Instantiate(visualNode, n.Position, Quaternion.identity).GetComponent<VisualNode>();
        vn.transform.position = n.Position;
        Dictionary<Node, LineRenderer> lines = new();
        foreach (var neigh in n.Neighbors)
        {
            Vector2 dir = neigh.Key.Position - n.Position;
            lines.Add(neigh.Key, vn.AddLine(neigh.Value, dir.normalized, scale));
        }

        if(type == 0)
        {
            vn.transform.SetParent(dijkstraVisuals);
            GraphicNode gn = new(vn, lines);
            dijkstraGraphics.Add(n.Position, gn);
        } 
        else if(type == 1)
        {
            vn.transform.SetParent(astarVisuals);
            GraphicNode gn = new(vn, lines);
            aStarGraphics.Add(n.Position, gn);
        }

    }

    public void AddDijkstraNode(Node n)
    {
        AddNode(0, n, 1f);
    }

    public void AddAstarNode(Node n)
    {
        AddNode(1, n, 0.5f);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            dijkstraVisuals.gameObject.SetActive(!dijkstraVisuals.gameObject.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            astarVisuals.gameObject.SetActive(!astarVisuals.gameObject.activeSelf);
        }
    }

}
