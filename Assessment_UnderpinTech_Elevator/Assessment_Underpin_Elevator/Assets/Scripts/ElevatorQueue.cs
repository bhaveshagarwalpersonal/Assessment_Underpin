using System.Collections.Generic;
using UnityEngine;

public class ElevatorQueue
{
    private readonly List<ElevatorRequest> requests = new List<ElevatorRequest>();

    public int Count => requests.Count;

    public bool ContainsFloor(int floor)
    {
        return requests.Exists(r => r.Floor == floor);
    }

    public void Add(ElevatorRequest request)
    {
        if (ContainsFloor(request.Floor))
            return;
        requests.Add(request);
        Debug.Log("Log 7 || Floor Request Added" + request.Floor);
    }

    public ElevatorRequest Peek()
    {
        if (requests.Count == 0)
            return null;
        return requests[0];
    }

    public void RemoveCurrent()
    {
        if (requests.Count == 0)
            return;
        requests.RemoveAt(0);
    }

    // Remove a specific request by floor
    public bool RemoveRequest(int floor)
    {
        var req = requests.Find(r => r.Floor == floor);
        if (req != null)
        {
            requests.Remove(req);
            return true;
        }
        return false;
    }

    public IReadOnlyList<ElevatorRequest> Requests => requests;
}