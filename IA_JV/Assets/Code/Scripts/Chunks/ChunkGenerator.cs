using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ChunkGenerator : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap chunkTemplate;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap ladders;
    [SerializeField] private Tilemap background;

    [Header("Type tiles")]
    [SerializeField] private TileBase ladderTile;
    [SerializeField] private TileBase ladderTopTile;
    [SerializeField] private TileBase backgroundTile;
    [SerializeField] private List<TileBase> platformsTile;

    [Header("Lights")]
    [SerializeField] private TileBase lightTile;
    [SerializeField] private GameObject lightPrefab;

    [Header("Nodes")]
    [SerializeField] private GameObject gridNodes;
    [SerializeField] private GameObject graphNode;

    [Header("Miscellaneous")]
    [SerializeField] private Transform grid;
    [SerializeField] private GameObject jewelPrefab;
    [SerializeField] private int RenderDistance = 5;
    [SerializeField] private GameObject spawnerTrigger;


    [Header("Containers")]
    [SerializeField] private Transform skeletonsContainer;
    [SerializeField] private Transform birdsContainer;
    [SerializeField] private Transform spawnersContainer;
    [SerializeField] private Transform lightsContainer;
    [SerializeField] private Transform jewelsContainer;

    [Header("A*")]
    [SerializeField] private GameObject AstarHandlers;
    [SerializeField] private GameObject AstarHandlerPrefab;
    [SerializeField] private GameObject BirdPrefab;

    [Header("Random")]
    [SerializeField] private float probabilityDoubleLadder = 0.4f;
    [SerializeField] private float probabilitySpawnerOnLadder = 0.2f;
    [SerializeField] private int minNbLights = 3;
    [SerializeField] private int maxNbLights = 5;
    [SerializeField] private int minNbJewels = 1;
    [SerializeField] private int maxNbJewels = 3;


    DijkstraConstructor dijkstraConstructor;
    Chunk chunk;
    Dictionary<int, ChunkInfo> chunks;
    Transform player;
    Vector3Int startPoint;
    TextMeshProUGUI floorText;
    VisualHandlers visualHandlers;

    // Start is called before the first frame update
    void Start()
    {
        dijkstraConstructor = DijkstraConstructor.Instance;

        Random.InitState(System.DateTime.Now.Millisecond);
        player = GameObject.FindGameObjectWithTag("Player").transform;

        chunk = new Chunk(chunkTemplate);
        chunks = new Dictionary<int, ChunkInfo>();

        startPoint = Vector3Int.FloorToInt(transform.position - chunk.Left * Vector3.right - chunk.Top * Vector3.up);

        visualHandlers = GameObject.FindGameObjectWithTag("VisualHandlers").GetComponent<VisualHandlers>();

        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
        foreach(GameObject node in nodes)
        {
            CreateGraphNode(node.transform.position - new Vector3(0.5f, 0.5f, 0));
        }
    }

    private int GetPlayerChunk()
    {
        if (player == null) return 0;
        if (chunk == null) return 0;
        return (int) (player.position.y / (chunk.Bottom - chunk.Top)) - 2;
    }

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

    private GameObject CreateGraphNode(Vector3 position, GameObject astarHandler = null)
    {
        GameObject g = Instantiate(graphNode, position + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
        if(astarHandler != null)
        {
            g.tag = "AstarNodes";
            g.transform.SetParent(astarHandler.transform);
        } 
        else
        {
            dijkstraConstructor.AddNodeAutoCheckNeighbor(position + new Vector3(0.5f, 0.5f, 0));
            g.transform.SetParent(gridNodes.transform);
        }
        return g;
    }

    private List<int> GenerateLadderInChunk(int nbLadder, List<GameObject> nodes, Dictionary<Vector3Int, TileBase> tilesWall)
    {
        List<int> laddersPosition = new List<int>();
        int previousX = int.MaxValue;
        for(int f = 0; f < nbLadder; f++)
        {
            int x;
            do
            {
                x = Random.Range(chunk.InnerLeft, chunk.InnerRight);
            } while (previousX - 1 <= x && x <= previousX + 1) ;
            laddersPosition.Add(x);
            for (int i = 0; i < chunk.InnerHeight + 4; i++)
            {
                Vector3Int position = startPoint + new Vector3Int(x, chunk.InnerTop + i, 0);
                ladders.SetTile(
                    position,
                    ladderTile
                );
                tilemap.SetTile(position, null);
                tilesWall.Remove(position);
                background.SetTile(position, backgroundTile);
                if (i > 0)
                    nodes.Add(CreateGraphNode(position));
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

    private void DestroyCrossNodes(Dictionary<Vector3, GameObject> astarNodes, Vector3 position)
    {
        if (astarNodes.TryGetValue(position, out GameObject go))
        {
            Destroy(go);
            astarNodes.Remove(position);
        }

        //if (astarNodes.TryGetValue(position + Vector3.up, out GameObject go))
        //    Destroy(go);

        //if (astarNodes.TryGetValue(position + Vector3.down, out go))
        //    Destroy(go);

        //if (astarNodes.TryGetValue(position + Vector3.right, out go))
        //    Destroy(go);

        //if (astarNodes.TryGetValue(position + Vector3.left, out go))
        //    Destroy(go);

    }

    private List<GameObject> GenerateLights(int nbLights, List<int> laddersPosition, Dictionary<Vector3Int, TileBase> tilesBg, Dictionary<Vector3, GameObject> astarNodes)
    {
        List<GameObject> lights = new List<GameObject>();
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
            tilesBg.Remove(position);
            tilesBg.Add(position, lightTile);
            GameObject g = Instantiate(lightPrefab, position + 0.5f * Vector3.one, Quaternion.identity);
            g.transform.SetParent(lightsContainer);
            g.transform.position = position + 0.5f * Vector3.one;
            lights.Add(g);
            DestroyCrossNodes(astarNodes, position);
        }
        return lights;
    }

    void GenerateSpawner(float ladderPosition)
    {
        Vector3 spawnPoint;
        if(Random.value < 0.5)
        {
            spawnPoint = startPoint + new Vector3(chunk.InnerLeft, chunk.InnerTop);
        } 
        else
        {
            spawnPoint = startPoint + new Vector3(chunk.InnerRight, chunk.InnerTop);
        }
        Vector3 spawnTriggerPosition = startPoint + new Vector3(ladderPosition, chunk.InnerTop);
        SpawnEnnemy s = Instantiate(spawnerTrigger, spawnTriggerPosition, Quaternion.identity).GetComponent<SpawnEnnemy>();
        s.EnnemySpawnPoint = spawnPoint;
        s.parent = skeletonsContainer;
        s.transform.SetParent(spawnersContainer);
        s.transform.position = spawnTriggerPosition;
    }

    private List<GameObject> GenerateJewels(int nbJewels)
    {
        List<GameObject> jewels = new List<GameObject>();
        float previousX = float.MaxValue;
        for (int f = 0; f < nbJewels; f++)
        {
            float x;
            do
            {
                x = Random.Range(chunk.InnerLeft, chunk.InnerRight);
            } while (previousX - 1 <= x && x <= previousX + 1);

            Vector3 position = startPoint + new Vector3(x + 0.5f, chunk.InnerTop + 0.5f, 0);

            GameObject go = Instantiate(jewelPrefab, position, Quaternion.identity);
            jewels.Add(go);
            go.transform.SetParent(jewelsContainer);
            go.transform.position = position;
            previousX = x;
        }
        return jewels;
    }

    AstarHandler GenerateAstarHandler()
    {
        GameObject handler = Instantiate(AstarHandlerPrefab, Vector3.zero, Quaternion.identity);
        handler.transform.SetParent(AstarHandlers.transform);
        return handler.GetComponent<AstarHandler>();
    }

    private BirdController GenerateBird(int platformPosition)
    {
        GameObject bird = Instantiate(BirdPrefab, transform.position, Quaternion.identity);
        bird.transform.SetParent(birdsContainer);
        bird.transform.position = startPoint + new Vector3(platformPosition, chunk.InnerBottom) + Vector3.one * 0.5f;
        return bird.GetComponent<BirdController>();
    }

    private int GeneratePlatform(List<int> laddersPosition, Dictionary<Vector3Int, TileBase> tilesBg)
    {
        int platformPosition = -1;
        if(laddersPosition.Count == 2)
        {
            if (laddersPosition.Contains(chunk.InnerLeft) && laddersPosition.Contains(chunk.InnerRight))
            {
                // pas d'oiseau
                return platformPosition;
            }
        }

        int idPlatform;
        if (laddersPosition.Contains(chunk.InnerRight))
        {
            platformPosition = chunk.InnerLeft;
            idPlatform = 0;
        }
        else if (laddersPosition.Contains(chunk.InnerLeft))
        {
            platformPosition = chunk.InnerRight;
            idPlatform = 1;
        }
        else if (Random.value < 0.5)
        {

            platformPosition = chunk.InnerRight;
            idPlatform = 1;
        }
        else
        {
            platformPosition = chunk.InnerLeft;
            idPlatform = 0;
        }

        int y = chunk.InnerBottom - 1;
        background.SetTile(startPoint + new Vector3Int(platformPosition, y, 0), platformsTile[idPlatform]);
        tilesBg.Remove(startPoint + new Vector3Int(platformPosition, y, 0));
        tilesBg.Add(startPoint + new Vector3Int(platformPosition, y, 0), platformsTile[idPlatform]);
        return platformPosition;
    }

    private ChunkInfo GenerateChunk()
    {
        AstarHandler astarHandler = GenerateAstarHandler();

        Dictionary<Vector3Int, TileBase> tilesWall      = new();
        Dictionary<Vector3Int, TileBase> tilesBg        = new();
        List<GameObject> nodes                          = new();
        Dictionary<Vector3, GameObject> astarNodes      = new();

        int nbLights = Random.Range(minNbLights, maxNbLights);
        int nbLadder = Random.value < probabilityDoubleLadder ? 2 : 1;
        int nbJewels = Random.Range(minNbJewels, maxNbJewels);


        for (int x = 0; x < chunk.Width; x++)
        {
            for (int y = 0; y < chunk.Height; y++)
            {
                TileBase tile = chunk.allTiles[x + y * chunk.Width];
                if (tile != null)
                {
                    tilemap.SetTile(startPoint + new Vector3Int(x, y, 0), tile);
                    tilesWall.Add(startPoint + new Vector3Int(x, y, 0), tile);
                } 
                else
                {
                    if(x >= chunk.InnerLeft && x <= chunk.InnerRight && y <= chunk.InnerBottom && y >= chunk.InnerTop)
                    {
                        background.SetTile(startPoint + new Vector3Int(x, y, 0), backgroundTile);
                        tilesBg.Add(startPoint + new Vector3Int(x, y, 0), backgroundTile);
                        astarNodes.Add(startPoint + new Vector3Int(x, y, 0), CreateGraphNode(startPoint + new Vector3Int(x, y, 0), astarHandler.gameObject));
                    }
                }
            }
        }

        for (int i = 0; i < chunk.InnerWidth + 1; i++)
        {
            Vector3Int position = startPoint + new Vector3Int(i + chunk.InnerWidth + 1, chunk.InnerTop, 0);
            nodes.Add(CreateGraphNode(position));
        }

        List<int> ladderPosition = GenerateLadderInChunk(nbLadder, nodes, tilesWall);

        if(Random.value < probabilitySpawnerOnLadder)
        {
            GenerateSpawner(ladderPosition[Random.Range(0, ladderPosition.Count)]);
        }

        List<GameObject> lights = GenerateLights(nbLights, ladderPosition, tilesBg, astarNodes);
        List<GameObject> jewels = GenerateJewels(nbJewels);
        int birdPlatform = GeneratePlatform(ladderPosition, tilesBg);
        astarHandler.Bird = GenerateBird(birdPlatform);


        ChunkInfo chunkInfo = new(tilesWall, tilesBg, lights, nodes, astarNodes.Values.ToList(), jewels);

        astarHandler.Generate(astarNodes.Values.ToList());
        chunkInfo.DeactivateChunk(tilemap, background);
        dijkstraConstructor.UpdateVisuals();

        return chunkInfo;
    }

    void DeactivateChunks()
    {
        if (chunks == null) return;
        int borneInf = RenderDistance / 2;
        int borneSup = RenderDistance % 2 == 0 ? RenderDistance / 2 : (RenderDistance / 2) + 1;

        if (chunks.TryGetValue(GetPlayerChunk() - borneInf - 1, out ChunkInfo chunkInfo))
        {
            chunkInfo.DeactivateChunk(tilemap, background);
        }

        for (int i = GetPlayerChunk() - borneInf; i < GetPlayerChunk() + borneSup; i++)
        {
            if(chunks.TryGetValue(i, out chunkInfo))
            {
                chunkInfo.ActivateChunk(tilemap, background);
            } 
            else if(i >= 0)
            {
                startPoint = Vector3Int.FloorToInt(transform.position - chunk.Left * Vector3.right - chunk.Top * Vector3.up);
                startPoint.y -= (chunk.Top - chunk.Bottom) * i;
                ChunkInfo c = GenerateChunk();
                chunks.Add(i, c);
                c.ActivateChunk(tilemap, background);
            }
        }

        if (chunks.TryGetValue(GetPlayerChunk() + borneSup, out chunkInfo))
        {
            chunkInfo.DeactivateChunk(tilemap, background);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DeactivateChunks();

        if(floorText == null)
        {
            GameObject go;
            if(go = GameObject.FindGameObjectWithTag("FloorText"))
            {
                floorText = go.GetComponent<TextMeshProUGUI>();
            }
        } 
        else
        {
            floorText.text = "Etage " + (Mathf.Clamp(GetPlayerChunk() + 1, 0, float.PositiveInfinity)).ToString();
        }
    }
}
