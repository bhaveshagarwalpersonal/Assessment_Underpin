using System;

[Serializable]
public class ElevatorRequest
{
    public int Floor;
    public bool IsGoingUp;

    public ElevatorRequest(int floor, bool goingUp)
    {
        Floor = floor;
        IsGoingUp = goingUp;
    }
}