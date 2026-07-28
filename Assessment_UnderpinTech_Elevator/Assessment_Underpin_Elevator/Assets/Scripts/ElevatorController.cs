using UnityEngine;
using System.Collections.Generic;

public class ElevatorController : MonoBehaviour
{
    public enum Direction { Idle, Up, Down }

    [Header("Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float stopThreshold = 0.02f;
    [SerializeField] private int currentFloor;
    [SerializeField] private FloorManager floorManager;

    [Header("Debug")]
    [SerializeField] private Direction currentDirection = Direction.Idle;
    [SerializeField] private List<int> stopFloors = new List<int>();
    private int stopIndex = 0;
    private bool isMoving = false;
    private float targetY;

    public int CurrentFloor => currentFloor;
    public ElevatorQueue Queue { get; private set; }
    public Direction CurrentDirection => currentDirection;

    // Event triggered when elevator arrives at a floor (useful for UI/doors)
    public System.Action<int> OnArrivedAtFloor;

    private void Awake()
    {
        Queue = new ElevatorQueue();
    }

    private void Update()
    {
        if (!isMoving) return;

        Debug.Log("Log 7 || Moving");

        // Move toward target
        Vector3 pos = transform.position;
        float newY = Mathf.MoveTowards(pos.y, targetY, speed * Time.deltaTime);
        pos.y = newY;
        transform.position = pos;

        // Check arrival
        if (Mathf.Abs(newY - targetY) < stopThreshold)
        {
            // Snap to exact floor position
            pos.y = targetY;
            transform.position = pos;
            ArriveAtFloor();
        }
    }

    public void AssignRequest(ElevatorRequest request)
    {
        //Debug.Log("Log 9 || Assign Request 1");

        if (Queue.ContainsFloor(request.Floor))
            return;

        Queue.Add(request);
        //Debug.Log("Log 9 || Assign Request 2");

        // If idle, start moving
        if (IsIdle())
        {
            Debug.Log("Log 4 || Start Moving" + gameObject.name);
            StartMoving();
        }
        else
        {
            // If moving, try to insert this stop into the current trip if possible
            Debug.Log("Log 5 || InsertStop" + gameObject.name);
            InsertStopIfPossible(request.Floor);
        }
    }

    private void StartMoving()
    {
        if (Queue.Count == 0) return;

        Debug.Log("Log 7 || Start Moving");

        // Determine initial direction toward the nearest request (or just first)
        var first = Queue.Peek();
        int targetFloor = first.Floor;
        currentDirection = (targetFloor > currentFloor) ? Direction.Up : Direction.Down;

        BuildStopList();
        if (stopFloors.Count > 0)
        {
            stopIndex = 0;
            GoToNextStop();
        }
        else
        {
            // No stops in that direction? Reverse and try again
            ReverseDirection();
            BuildStopList();
            if (stopFloors.Count > 0)
            {
                stopIndex = 0;
                GoToNextStop();
            }
        }
    }

    private void BuildStopList()
    {
        stopFloors.Clear();

        // Collect all requests that are in the current direction from current floor
        foreach (var req in Queue.Requests)
        {
            if (currentDirection == Direction.Up && req.Floor > currentFloor)
                stopFloors.Add(req.Floor);
            else if (currentDirection == Direction.Down && req.Floor < currentFloor)
                stopFloors.Add(req.Floor);
        }

        // Sort ascending for Up, descending for Down
        if (currentDirection == Direction.Up)
            stopFloors.Sort();
        else
            stopFloors.Sort((a, b) => b.CompareTo(a)); // descending
    }

    private void GoToNextStop()
    {
        if (stopIndex >= stopFloors.Count)
        {
            // No more stops in current direction
            ReverseDirection();
            BuildStopList();
            stopIndex = 0;
            if (stopFloors.Count == 0)
            {
                isMoving = false;
                currentDirection = Direction.Idle;
                return;
            }
        }

        int nextFloor = stopFloors[stopIndex];
        targetY = floorManager.GetFloorY(nextFloor);
        isMoving = true;
    }

    private void ArriveAtFloor()
    {
        // We are at target floor
        int arrivedFloor = stopFloors[stopIndex];
        currentFloor = arrivedFloor;

        // Remove the request from the queue
        Queue.RemoveRequest(arrivedFloor);

        // Notify (e.g., for UI or door control)
        OnArrivedAtFloor?.Invoke(arrivedFloor);

        // Move to next stop
        stopIndex++;
        GoToNextStop();
    }

    private void ReverseDirection()
    {
        currentDirection = (currentDirection == Direction.Up) ? Direction.Down : Direction.Up;
    }

    private void InsertStopIfPossible(int floor)
    {
        // If moving and the new floor is in the same direction and ahead, insert into stopFloors
        if (currentDirection == Direction.Up && floor > currentFloor)
        {
            if (!stopFloors.Contains(floor))
            {
                stopFloors.Add(floor);
                stopFloors.Sort();
                // If we haven't passed it yet, adjust index
                if (floor < stopFloors[stopIndex]) // inserted before current target? (shouldn't happen if we only insert ahead)
                {
                    // Rebuild list to be safe
                    BuildStopList();
                    stopIndex = 0;
                }
            }
        }
        else if (currentDirection == Direction.Down && floor < currentFloor)
        {
            if (!stopFloors.Contains(floor))
            {
                stopFloors.Add(floor);
                stopFloors.Sort((a, b) => b.CompareTo(a));
                if (floor > stopFloors[stopIndex])
                {
                    BuildStopList();
                    stopIndex = 0;
                }
            }
        }
        // Otherwise, it will be handled after reversal
    }

    public bool IsIdle() => !isMoving;

    // Optional: set floor manager reference
    public void SetFloorManager(FloorManager fm) => floorManager = fm;

    // Called by dispatcher after assignment to start if idle
    public void TryStartMoving()
    {
        if (IsIdle() && Queue.Count > 0)
            StartMoving();
    }
}