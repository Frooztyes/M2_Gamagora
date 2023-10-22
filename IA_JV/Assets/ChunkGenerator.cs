using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ChunkGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap chunkTemplate;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap ladders;
    [SerializeField] private Tilemap background;


    [SerializeField] private Transform grid;
    [SerializeField] private TileBase ladderTile;
    [SerializeField] private TileBase ladderTopTile;
    [SerializeField] private TileBase backgroundTile;
    [SerializeField] private GameObject grid2; 

    [SerializeField] private GameObject graphNode;
    
    [SerializeField] private TileBase lightTile;
    [SerializeField] private GameObject lightPrefab;

    [SerializeField] private int RenderDistance = 5;

    Chunk chunk;

    Dictionary<int, List<GameObject>> chunks;

    Transform player;
    Vector3Int startPoint;
    int firstChunk;


    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        player = GameObject.FindGameObjectWithTag("Player").transform;

        chunk = new Chunk(chunkTemplate);
        chunks = new Dictionary<int, List<GameObject>>();

        currentChunkId = GetPlayerChunk();

        startPoint = Vector3Int.FloorToInt(transform.position - chunk.Left * Vector3.right - chunk.Top * Vector3.up);

        firstChunk = startPoint.y;

        //chunks.Add(currentChunkId, new List<GameObject>());

        //GenerateChunk();
        //startPoint = Vector3Int.FloorToInt(transform.position - chunk.Left * Vector3.right - chunk.Top * Vector3.up);
        //currentChunkId++;
    }
    private int GetPlayerChunk()
    {
        return (int) (player.position.y / (chunk.Bottom - chunk.Top)) - 2;
    }

    int currentChunkId;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        return;
        if(chunk == null) chunk = new Chunk(chunkTemplate);
        Vector3Int startPoint = Vector3Int.FloorToInt(transform.position - chunk.Left * Vector3.right - chunk.Top * Vector3.up);
        for (int x = 0; x < chunk.Width; x++)
        {
            for (int y = 0; y < chunk.Height; y++)
            {
                TileBase tile = chunk.allTiles[x + y * chunk.Width];
                Gizmos.color = Color.grey;
                if (tile != null)
                {
                    Gizmos.DrawCube(startPoint + new Vector3(x + 0.5f, y + 0.5f, 0), Vector3.one);
                }

            }
        }

        //int randomLadderX = Random.Range(chunk.InnerLeft, chunk.InnerRight);
        //Gizmos.color = Color.red;
        //Gizmos.DrawCube(startPoint + new Vector3(randomLadderX + 0.5f, chunk.InnerTop + 0.5f, 0), Vector3.one);

        //Debug.Log(chunk.InnerWidth);
        //Debug.Log(chunk.InnerHeight);

    }
#endif

    private void CreateGraphNode(Vector3 position)
    {
        GameObject g = Instantiate(graphNode, position + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
        g.transform.parent = grid2.transform;
        if(chunks.TryGetValue(currentChunkId, out List<GameObject> nodes)) {
            nodes.Add(g);
        }
    }

    private List<int> GenerateLadderInChunk()
    {
        List<int> laddersPosition = new List<int>();
        int previousX = int.MaxValue;
        int nbLadder = Random.value < 0.4 ? 2 : 1;
        for(int f = 0; f < nbLadder; f++)
        {
            int x;
            do
            {
                x = Random.Range(chunk.InnerLeft, chunk.InnerRight);
            } while (previousX - 1 <= x && x <= previousX + 1) ;
            laddersPosition.Add(x);
            for (int i = 0; i < chunk.InnerHeight + 3; i++)
            {
                Vector3Int position = startPoint + new Vector3Int(x, chunk.InnerTop + i, 0);
                ladders.SetTile(
                    position,
                    ladderTile
                );
                tilemap.SetTile(position, null);
                background.SetTile(position, backgroundTile);
                if (i > 0)
                    CreateGraphNode(position);
            }
            ladders.SetTile(
                startPoint + new Vector3Int(x, chunk.InnerTop + chunk.InnerHeight + 3, 0),
                ladderTopTile
            );
            tilemap.SetTile(startPoint + new Vector3Int(x, chunk.InnerTop + chunk.InnerHeight + 3, 0), null);
            previousX = x;
        }
        return laddersPosition;
    }

    private void GenerateLights(List<int> laddersPosition)
    {
        int nbLights = Random.Range(3, 5);
        Vector2 previousPosition = new Vector2(float.MaxValue, float.MaxValue);
        for (int f = 0; f < nbLights; f++)
        {
            int x;
            do
            {
                x = Random.Range(chunk.InnerLeft, chunk.InnerRight);
            } while (previousPosition.x - 1 <= x && x <= previousPosition.x + 1 || laddersPosition.Contains(x));

            int y;
            do
            {
                y = Random.Range(chunk.InnerTop, chunk.InnerBottom);
            } while (previousPosition.y - 1 <= y && y <= previousPosition.y + 1);

            Vector3Int position = startPoint + new Vector3Int(x, y, 0);
            background.SetTile(position, lightTile);
            Instantiate(lightPrefab, position + 0.5f * Vector3.one, Quaternion.identity);
        }
    }

    private void GenerateChunk()
    {

        for (int x = 0; x < chunk.Width; x++)
        {
            for (int y = 0; y < chunk.Height; y++)
            {
                TileBase tile = chunk.allTiles[x + y * chunk.Width];
                if (tile != null)
                {
                    tilemap.SetTile(startPoint + new Vector3Int(x, y, 0), tile);
                } 
                else
                {
                    if(x >= chunk.InnerLeft && x <= chunk.InnerRight && y <= chunk.InnerBottom && y >= chunk.InnerTop)
                    {
                        background.SetTile(startPoint + new Vector3Int(x, y, 0), backgroundTile);
                    }
                }
            } 
        }

        for (int i = 1; i <= chunk.InnerWidth + 1; i++)
        {
            Vector3Int position = startPoint + new Vector3Int(i + chunk.InnerWidth, chunk.InnerTop, 0);
            CreateGraphNode(position);
        }

        List<int> ladderPosition = GenerateLadderInChunk();

        GenerateLights(ladderPosition);

    }

    void DeactivateChunks()
    {
        foreach (List<GameObject> nodes in chunks.Values)
        {
            foreach(GameObject node in nodes)
            {
                if(!node.CompareTag("Start") || !node.CompareTag("End"))
                    node.tag = "DeactivatedNode";
            }
        }

        int borneInf = RenderDistance / 2;
        int borneSup = RenderDistance % 2 == 0 ? RenderDistance / 2 : (RenderDistance / 2) + 1;
        for (int i = GetPlayerChunk() - borneInf; i < GetPlayerChunk() + borneSup; i++)
        {
            if(chunks.TryGetValue(i, out List<GameObject> nodes))
            {
                foreach(GameObject node in nodes)
                {
                    if (!node.CompareTag("Start") || !node.CompareTag("End"))
                        node.tag = "Node";
                }
            } else if(i >= 0)
            {
                startPoint = Vector3Int.FloorToInt(transform.position - chunk.Left * Vector3.right - chunk.Top * Vector3.up);
                startPoint.y -= (chunk.Top - chunk.Bottom) * i;
                chunks.Add(i, new List<GameObject>());
                currentChunkId = i;
                GenerateChunk();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        DeactivateChunks();
    }
}
