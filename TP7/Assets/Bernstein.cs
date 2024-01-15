using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bernstein : MonoBehaviour
{
    [SerializeField] private Transform[] controlPoints;
    [Range(0.01f, 1)]
    [SerializeField] private float interval = 0.01f;

    private void OnDrawGizmos()
    {
        List<Vector3> myPoints = PointList3(controlPoints, interval);
        Gizmos.color = Color.red;
        for (int i = 0; i < myPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(myPoints[i], myPoints[i + 1]);
        }
    }

    public float Factorial(int f)
    {
        if (f == 0)
            return 1;
        else
            return f * Factorial(f - 1);
    }

    float MyBernstein(int n, int i, float t)
    {
        return 
            Factorial(n) 
            / 
            (Factorial(i) * Factorial(n - i)) 
            *
            Mathf.Pow(t, i)
            * 
            Mathf.Pow(1 - t, n - i);
    }

    private Vector3 Point3(float t, Transform[] controlPoints)
    {
        Vector3 p = new();

        if (t <= 0) return controlPoints[0].position;
        if (t >= 1) return controlPoints[^1].position;

        for (int i = 0; i < controlPoints.Length; ++i)
        {
            p += MyBernstein(controlPoints.Length - 1, i, t) * controlPoints[i].position;
        }

        return p;
    }

    private List<Vector3> PointList3(Transform[] controlPoints, float interval = 0.01f)
    {
        List<Vector3> points = new();

        for (float t = 0.0f; t <= 1.0f + interval - 0.0001f; t += interval)
        {
            points.Add(Point3(t, controlPoints));
        }

        return points;
    }
}
