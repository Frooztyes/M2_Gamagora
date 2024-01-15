using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HermiteCurve : MonoBehaviour
{
    [SerializeField] private int nbHermite;
    [SerializeField] private Transform point0;
    [SerializeField] private Transform point1;
    [SerializeField] private Vector3 velocity0;
    [SerializeField] private Vector3 velocity1;

    private void OnDrawGizmos()
    {
        List<Vector3> childPoints = new List<Vector3>();
        childPoints.Add(point0.position);
        childPoints.Add(point1.position);

        List<Vector3> points = new List<Vector3>();

        points.Add(childPoints[0]);
        for (float i = 0; i < nbHermite; i++)
        {
            float u = i / nbHermite;
            Vector3 position = HermiteInterpolation(u, childPoints[0], childPoints[1], velocity0, velocity1);
            points.Add(position);

        }
        points.Add(childPoints[1]);

        //Gizmos.color = Color.green;
        //for (int i = 0; i < childPoints.Count - 1; i++)
        //{
        //    Gizmos.DrawLine(childPoints[i], childPoints[i + 1]);
        //}

        Gizmos.color = Color.blue;
        for (int i = 0; i < points.Count - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }

    Vector3 HermiteInterpolation(float u, Vector3 p0, Vector3 p1, Vector3 v0, Vector3 v1)
    {
        float u2 = u * u;
        float u3 = u2 * u;

        float blend1 = 2 * u3 - 3 * u2 + 1;
        float blend2 = -2 * u3 + 3 * u2;
        float blend3 = u3 - 2 * u2 + u;
        float blend4 = u3 - u2;

        Vector3 position = blend1 * p0 + blend2 * p1 + blend3 * v0 + blend4 * v1;

        return position;
    }
}
