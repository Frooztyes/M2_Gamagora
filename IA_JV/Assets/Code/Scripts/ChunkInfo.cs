using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkInfo
{
    readonly Dictionary<Vector3Int, TileBase> wallsPosition;
    readonly Dictionary<Vector3Int, TileBase> backgroundPosition;
    readonly List<GameObject> lights;
    readonly List<GameObject> nodes;

    public bool IsActive;

    public void DeactivateChunk(Tilemap walls, Tilemap background)
    {
        if (!IsActive) return;
        foreach (var wallPos in wallsPosition)
        {
            walls.SetTile(wallPos.Key, null);
        }

        foreach (var bgPos in backgroundPosition)
        {
            background.SetTile(bgPos.Key, null);
        }

        foreach(GameObject light in lights)
        {
            light.SetActive(false);
        }

        foreach (GameObject node in nodes)
        {
            if (!node.CompareTag("Start") || !node.CompareTag("End"))
                node.tag = "DeactivatedNode";
        }
        IsActive = false;
    }

    public void ActivateChunk(Tilemap walls, Tilemap background)
    {
        if (IsActive) return;
        foreach (var wallPos in wallsPosition)
        {
            walls.SetTile(wallPos.Key, wallPos.Value);
        }

        foreach (var bgPos in backgroundPosition)
        {
            background.SetTile(bgPos.Key, bgPos.Value);
        }

        foreach (GameObject light in lights)
        {
            light.SetActive(true);
        }

        foreach (GameObject node in nodes)
        {
            if (!node.CompareTag("Start") || !node.CompareTag("End"))
                node.tag = "Node";
        }
        IsActive = true;
    }


    public ChunkInfo(
        Dictionary<Vector3Int, TileBase> wallsPosition, 
        Dictionary<Vector3Int, TileBase> backgroundPosition, 
        List<GameObject> lights, 
        List<GameObject> nodes)
    {
        IsActive = false;
        this.wallsPosition = wallsPosition;
        this.backgroundPosition = backgroundPosition;
        this.lights = lights;
        this.nodes = nodes;
    }
    
}
