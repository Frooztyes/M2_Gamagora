using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Graph graph = new Graph();

        graph.AddNode(new Node("Source"));
        graph.AddNode(new Node("1"));
        graph.AddNode(new Node("2"));
        graph.AddNode(new Node("3"));

        graph.CreateLink(
            graph.GetNodeByName("Source"),
            graph.GetNodeByName("1"),
            10,
            Graph.TypeLink.OneWay
        );

        Debug.Log(graph);
    }
}
