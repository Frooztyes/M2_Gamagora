using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class AstarHandler : MonoBehaviour
{
    public AstarConstructor AstarConstructor { get; set; }
    public BirdController Bird { get; set; }

    private Transform end;
    private Transform player;
    private Vector3 size;
    private bool generated = false;

    private VisualHandlers visuals;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        size = player.GetComponent<SpriteRenderer>().size;
        visuals = GameObject.FindGameObjectWithTag("VisualHandlers").GetComponent<VisualHandlers>();
    }

    public void Generate(List<GameObject> rawNodes)
    {
        AstarConstructor = new AstarConstructor(rawNodes, GameObject.FindGameObjectWithTag("VisualHandlers").GetComponent<VisualHandlers>());
        generated = true;
    }

    private void Update()
    {
        if (!generated) return;

        if (player == null) return;

        end = AstarConstructor.UpdateEndPosition(player.position, size);
        if (end == null)
            return;

        if (Bird == null || Bird.gameObject == null)
        {
            visuals.ResetAstarVisuals();
            return;
        }

        if (AstarConstructor.CalculateNextWaypoint(Bird, end, out Vector2 dir))
        {
            Bird.WaypointToGo = dir;
        }
        else
        {
            Bird.Stop();
        }
    }


}
