using UnityEngine;
using System.Collections.Generic;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private List<Floor> floors = new List<Floor>();

    private Dictionary<int, float> floorYPositions = new Dictionary<int, float>();

    private void Awake()
    {
        // Build dictionary for fast lookup
        foreach (var floor in floors)
        {
            if (!floorYPositions.ContainsKey(floor.FloorNumber))
                floorYPositions.Add(floor.FloorNumber, floor.transform.position.y);
            else
                Debug.LogWarning($"Duplicate floor number: {floor.FloorNumber}");
        }
    }

    public float GetFloorY(int floorNumber)
    {
        if (floorYPositions.TryGetValue(floorNumber, out float y))
            return y;
        Debug.LogError($"Floor {floorNumber} not found!");
        return 0f;
    }

    public int GetFloorCount() => floors.Count;

    // Optional: get closest floor from a y position
    public int GetClosestFloor(float y)
    {
        int closest = 0;
        float minDist = float.MaxValue;
        foreach (var kvp in floorYPositions)
        {
            float dist = Mathf.Abs(y - kvp.Value);
            if (dist < minDist)
            {
                minDist = dist;
                closest = kvp.Key;
            }
        }
        return closest;
    }
}