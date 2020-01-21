using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyPath
{
    [SerializeField]
    Vector3[] points;
    public int numPoints { get => points.Length; }

    public int GetRequiredIndex(Vector3 position, int currentIndex, float threshold = 0.1f)
    {
        float sqrDist = (points[currentIndex] - position).sqrMagnitude;
        if (sqrDist <= threshold)
            return (currentIndex + 1) % points.Length;
        else
            return currentIndex;
    }

    public Vector3 GetPoint(int pointIndex)
    {
        return points[pointIndex];
    }
}
