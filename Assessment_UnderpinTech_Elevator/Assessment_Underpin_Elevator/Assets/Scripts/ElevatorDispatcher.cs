using UnityEngine;
using System.Collections.Generic;

public class ElevatorDispatcher : MonoBehaviour
{
    [SerializeField] private List<ElevatorController> elevators = new List<ElevatorController>();
    [SerializeField] private FloorManager floorManager; // optional, for distance calculation

    // Raised whenever any elevator arrives at a floor - lets FloorButtons know to reset
    public event System.Action<int> OnFloorServiced;

    private void Awake()
    {
        foreach (var elevator in elevators)
        {
            if (elevator != null)
                elevator.OnArrivedAtFloor += HandleElevatorArrived;
        }
    }

    private void OnDestroy()
    {
        foreach (var elevator in elevators)
        {
            if (elevator != null)
                elevator.OnArrivedAtFloor -= HandleElevatorArrived;
        }
    }

    private void HandleElevatorArrived(int floor)
    {
        OnFloorServiced?.Invoke(floor);
    }

    public void RegisterRequest(ElevatorRequest request)
    {
        Debug.Log("Log 1 || Request Registered");
        ElevatorController bestElevator = null;
        int bestScore = int.MaxValue;

        foreach (var elevator in elevators)
        {
            int score = CalculateScore(elevator, request);
            if (score < bestScore)
            {
                bestScore = score;
                bestElevator = elevator;
            }
        }

        if (bestElevator != null)
        {
            Debug.Log("Log 3 || Request Added");
            bestElevator.AssignRequest(request);
            // If idle, start moving (AssignRequest will call StartMoving if idle, but we call just in case)
            bestElevator.TryStartMoving();
        }
        else
        {
            Debug.LogError("No elevator available!");
        }
    }

    private int CalculateScore(ElevatorController elevator, ElevatorRequest request)
    {
        int floor = request.Floor;
        bool goingUp = request.IsGoingUp;
        int current = elevator.CurrentFloor;
        var dir = elevator.CurrentDirection;
        bool idle = elevator.IsIdle();

        // Base score: distance (in floors)
        int distance = Mathf.Abs(current - floor);
        int score = distance;

        if (idle)
        {
            // Idle elevators are preferred, reduce score
            score -= 1; // slight bonus
        }
        else
        {
            // If moving, check if this request is along the current path
            bool isAlongPath = false;
            if (dir == ElevatorController.Direction.Up && goingUp && floor > current)
                isAlongPath = true;
            else if (dir == ElevatorController.Direction.Down && !goingUp && floor < current)
                isAlongPath = true;

            // If not along path, add a significant penalty
            if (!isAlongPath)
                score += 10; // large penalty to avoid unnecessary reversals

            // Also penalize if the elevator already has a stop at this floor (to avoid duplicates)
            if (elevator.Queue.ContainsFloor(floor))
                score += 20; // very high, but shouldn't happen due to ContainsFloor check
        }

        // Optional: add a small random factor to break ties? Not necessary.

        return score;
    }
}