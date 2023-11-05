using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DijkstraHandler : MonoBehaviour
{
    public DijkstraConstructor DijkstraConstructor { get; set; }

    [SerializeField] private Transform Grid;
    [SerializeField] private GameObject graphNode;

    VisualHandlers visualHandlers;
    GameObject end;
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        DijkstraConstructor = DijkstraConstructor.Instance;

        visualHandlers = GameObject.FindGameObjectWithTag("VisualHandlers").GetComponent<VisualHandlers>();
    }

    private void Update()
    {
        Node endNode = DijkstraConstructor.UpdateEndPosition(player.position);
        if (endNode == null) return;

        List<EnnemyController> ennemies = new List<EnnemyController>();
        List<Node> startNodes = DijkstraConstructor.SetupEnnemies(out ennemies);

        visualHandlers.ResetDijkstraVisuals();

        foreach (EnnemyController ennemy in ennemies)
        {
            if (DijkstraConstructor.CalculateNextWaypoint(ennemy, endNode.Position, out Vector2 dir, out List<Node> path))
            {
                ennemy.WaypointToGo = dir;
                visualHandlers.PrintDijkstraPath(startNodes, endNode, path);
            }
            else
            {
                ennemy.Stop();
            }
        }
    }


    
}
