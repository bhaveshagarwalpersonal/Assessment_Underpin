using UnityEngine;

public class FloorButton : MonoBehaviour
{
    [SerializeField]
    private ElevatorDispatcher dispatcher;

    [SerializeField]
    private int floor;

    [SerializeField]
    private bool goingUp;

    public void PressButton()
    {
        dispatcher.RegisterRequest(
            new ElevatorRequest(floor, goingUp));
    }
}