using UnityEngine;

public class FloorButton : MonoBehaviour
{
    [SerializeField]
    private ElevatorDispatcher dispatcher;

    [SerializeField]
    private int floor;

    [SerializeField]
    private bool goingUp;

    [Header("Visuals")]
    [SerializeField]
    [Tooltip("Renderer whose material color changes to show button state.")]
    private Renderer buttonRenderer;

    [SerializeField]
    private Color idleColor = Color.white;

    [SerializeField]
    private Color waitingColor = Color.red;

    private bool isWaiting;

    private void OnEnable()
    {
        if (dispatcher != null)
            dispatcher.OnFloorServiced += HandleFloorServiced;

        SetColor(idleColor);
    }

    private void OnDisable()
    {
        if (dispatcher != null)
            dispatcher.OnFloorServiced -= HandleFloorServiced;
    }

    public void PressButton()
    {
        if (isWaiting)
            return; // already requested for this floor/direction, ignore repeat presses

        isWaiting = true;
        SetColor(waitingColor);

        dispatcher.RegisterRequest(new ElevatorRequest(floor, goingUp));
    }

    private void HandleFloorServiced(int servicedFloor)
    {
        if (servicedFloor != floor)
            return;

        isWaiting = false;
        SetColor(idleColor);
    }

    private void SetColor(Color color)
    {
        if (buttonRenderer != null)
            buttonRenderer.material.color = color;
    }
}