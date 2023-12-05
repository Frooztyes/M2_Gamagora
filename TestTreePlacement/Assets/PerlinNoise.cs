using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerlinNoise : MonoBehaviour
{
    [Range(5, 20)]
    [SerializeField] private int bounds;
    [Range(0.2f, 5)]
    [SerializeField] private float _minDist;
    [Range(1, 2000)]
    [SerializeField] private int seed = 10;
    [Range(1, 20)]
    [SerializeField] private int newPoints = 1;

    struct RandomQueue
    {
        List<Vector2> points;

        public bool empty()
        {
            return points.Count == 0;
        }

        public void push(Vector2 value)
        {
            points.Add(value);
        }

        public Vector2 pop()
        {
            int randomIndex = Random.Range(0, points.Count - 1);
            Vector2 pt = points[randomIndex];
            points.RemoveAt(randomIndex);
            return pt;
        }

        public RandomQueue(int i)
        {
            points = new List<Vector2>();
        }
    }




    int Rand(int max)
    {
        return Random.Range(0, max);
    }

    Vector2Int imageToGrid(Vector2 point, float cellSize)
    {
        int gridX = (int) (point.x / cellSize);
        int gridY = (int) (point.y / cellSize);
        return new Vector2Int(gridX, gridY);
    }

    List<Vector2> generatePoisson(int width, int height, float minDist, int newPointsCount)
    {
        float cellSize = minDist / Mathf.Sqrt(2);
        Vector2?[,] grid = new Vector2?[(int)Mathf.Ceil(width / cellSize), (int)Mathf.Ceil(height / cellSize)];

        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = null;
            }
        }

        RandomQueue processList = new RandomQueue(0);
        List<Vector2> samplePoints = new List<Vector2>();

        Vector2 firstPoint = new Vector2(Rand(width), Rand(height));

        processList.push(firstPoint);
        samplePoints.Add(firstPoint);
        grid[imageToGrid(firstPoint, cellSize).x, imageToGrid(firstPoint, cellSize).y] = firstPoint;
        while(!processList.empty())
        {
            Vector2 point = processList.pop();

            for (int i = 0; i < newPointsCount; i++)
            {
                Vector2 newPoint = generateRandomPointAround(point, minDist);

                if(inRectangle(newPoint, width, height) && !inNeighbourhood(grid, newPoint, minDist, cellSize))
                {
                    processList.push(newPoint);
                    samplePoints.Add(newPoint);
                    Vector2Int pointOnGrid = imageToGrid(newPoint, cellSize);
                    grid[pointOnGrid.x, pointOnGrid.y] = newPoint;
                }
            }
        }

        return samplePoints;
    }

    bool inRectangle(Vector2 point, int width, int height)
    {
        return point.x >= 0 && point.x < width && point.y >= 0 && point.y < height;
    }

    Vector2 generateRandomPointAround(Vector2 point, float mindist)
    {
        float r1 = Random.value;
        float r2 = Random.value;

        // max dist = 2 * min dist
        float radius = mindist * (r1 + 1);
        float angle = 2 * Mathf.PI * r2;
        float newX = point.x + radius * Mathf.Cos(angle);
        float newY = point.y + radius * Mathf.Sin(angle);
        return new Vector2(newX, newY);
    }

    bool inNeighbourhood(Vector2?[,] grid, Vector2 point, float minDist, float cellSize)
    {
        Vector2 gridPoint = imageToGrid(point, cellSize);

        List<Vector2?> cellsAroundPoint = squareAroundPoint(grid, gridPoint, 1);
        foreach (Vector2? cell in cellsAroundPoint)
        {
            if(cell is Vector2 cellV)
            {
                if(Vector2.Distance(cellV, point) < minDist)
                {
                    return true;
                }
            }
        }
        return false;
    }

    List<Vector2?> squareAroundPoint(Vector2?[,] grid, Vector2 gridPoint, float radius)
    {
        List<Vector2?> pts = new List<Vector2?>();
        //for (int i = 0; i < grid.GetLength(0); i++)
        //{
        //    for (int j = 0; j < grid.GetLength(1); j++)
        //    {
        //        if(Vector2.Distance(grid[i,j], gridPoint) < radius)
        //        {
        //            pts.Add(grid[i, j]);
        //        }
        //    }
        //}
        //return pts;
        float minIndexX = gridPoint.x - 5;
        minIndexX = minIndexX <= 0 ? 0 : minIndexX;

        float maxIndexX = gridPoint.x + 5;
        maxIndexX = maxIndexX > grid.GetLength(0) ? grid.GetLength(0) - 1 : maxIndexX;


        float minIndexY = gridPoint.y - 5;
        minIndexY = minIndexY <= 0 ? 0 : minIndexY;

        float maxIndexY = gridPoint.y + 5;
        maxIndexY = maxIndexY > grid.GetLength(1) ? grid.GetLength(1) - 1 : maxIndexY;

        for (float i = minIndexX; i < maxIndexX; i++)
        {
            for (float j = minIndexY; j < maxIndexY; j++)
            {
                Vector2? value = grid[(int)i, (int) j];
                pts.Add(value);
            }
        }

        return pts;
    }

    RawImage myImage;

    public Texture2D DrawCircle(Texture2D tex, Color color, int x, int y, int radius = 3)
    {
        float rSquared = radius * radius;

        for (int u = x - radius; u < x + radius + 1; u++)
            for (int v = y - radius; v < y + radius + 1; v++)
                if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                {
                    if (u < 0 || v < 0 || u > tex.width || v > tex.height) continue;
                    tex.SetPixel(u, v, color);
                }

        return tex;
    }

    private void Start()
    {
        myImage = GetComponent<RawImage>();

        List<Vector2> myPts = generatePoisson(
            (int)myImage.rectTransform.rect.width, 
            (int)myImage.rectTransform.rect.height, _minDist, newPoints);

        Texture2D tex = new((int)myImage.rectTransform.rect.width, (int) myImage.rectTransform.rect.height);
        var colorData = tex.GetPixels32();
        for (int i = 0; i < colorData.Length; i++)
        {
            colorData[i] = new Color(0, 0, 0, 0);
        }
        tex.SetPixels32(colorData);

        foreach (Vector2 pt in myPts)
        {
            DrawCircle(tex, Color.red, (int) pt.x, (int) pt.y, 30);
        }

        

        tex.Apply();
        myImage.texture = tex;
    }


    private void OnDrawGizmosSelected()
    {
        bounds = 1000;
        _minDist = 100;
        newPoints = 1;
        Random.InitState(seed);
        List<Vector2> myPts = generatePoisson(bounds, bounds, _minDist, newPoints);
        Gizmos.color = Color.red;

        Gizmos.DrawLine(
            new Vector3(0, 0, bounds),
            new Vector3(bounds, 0, bounds)
        );

        Gizmos.DrawLine(
            new Vector3(bounds, 0, 0),
            new Vector3(bounds, 0, bounds)
        );

        Gizmos.DrawLine(
            new Vector3(0, 0, 0),
            new Vector3(0, 0, bounds)
        );

        Gizmos.DrawLine(
            new Vector3(0, 0, 0),
            new Vector3(bounds, 0, 0)
        );

        foreach (Vector2 pt in myPts)
        {
            Gizmos.DrawSphere(new Vector3(pt.x, pt.y, 0), 5f);
        }
    }
}
