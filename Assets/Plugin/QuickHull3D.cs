using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Простенька заглушка для QuickHull3D.
/// Для реального застосування потрібно використати більш точну реалізацію (наприклад, VHACD або QHull).
/// </summary>
public class QuickHull3D
{
    private List<Vector3> points;
    private List<Vector3> hullVertices;
    private List<int> hullTriangles;

    public QuickHull3D()
    {
        points = new List<Vector3>();
        hullVertices = new List<Vector3>();
        hullTriangles = new List<int>();
    }

    /// <summary>
    /// Побудова опуклої оболонки з набору точок.
    /// </summary>
    /// <param name="pts">Масив точок (Vector3[])</param>
    public void Build(Vector3[] pts)
    {
        points.Clear();
        points.AddRange(pts);
        ComputeHull();
    }

    /// <summary>
    /// Проста (placeholder) реалізація: використовуємо Bounding Box.
    /// </summary>
    private void ComputeHull()
    {
        hullVertices.Clear();
        hullTriangles.Clear();

        if (points.Count < 4)
        {
            hullVertices.AddRange(points);
            return;
        }

        // Обчислюємо мінімальні та максимальні координати (bounding box)
        Vector3 min = points[0], max = points[0];
        foreach (Vector3 p in points)
        {
            min = Vector3.Min(min, p);
            max = Vector3.Max(max, p);
        }

        // Створюємо вершини куба, який охоплює всі точки
        hullVertices.Add(new Vector3(min.x, min.y, min.z));
        hullVertices.Add(new Vector3(max.x, min.y, min.z));
        hullVertices.Add(new Vector3(max.x, max.y, min.z));
        hullVertices.Add(new Vector3(min.x, max.y, min.z));
        hullVertices.Add(new Vector3(min.x, min.y, max.z));
        hullVertices.Add(new Vector3(max.x, min.y, max.z));
        hullVertices.Add(new Vector3(max.x, max.y, max.z));
        hullVertices.Add(new Vector3(min.x, max.y, max.z));

        // Задаємо трикутники для куба (опукла оболонка)
        int[] cubeTriangles = new int[]
        {
            0,1,2, 0,2,3, // front face
            1,5,6, 1,6,2, // right face
            5,4,7, 5,7,6, // back face
            4,0,3, 4,3,7, // left face
            3,2,6, 3,6,7, // top face
            4,5,1, 4,1,0  // bottom face
        };
        hullTriangles.AddRange(cubeTriangles);
    }

    /// <summary>
    /// Повертає вершини опуклої оболонки.
    /// </summary>
    public Vector3[] GetVertices()
    {
        return hullVertices.ToArray();
    }

    /// <summary>
    /// Повертає трикутники опуклої оболонки.
    /// </summary>
    public int[] GetTriangles()
    {
        return hullTriangles.ToArray();
    }
}
