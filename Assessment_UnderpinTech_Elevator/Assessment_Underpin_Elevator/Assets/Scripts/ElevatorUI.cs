using UnityEngine;
using TMPro; // or use UnityEngine.UI.Text

public class ElevatorUI : MonoBehaviour
{
    [SerializeField] private TextMeshPro floorDisplay; // drag your UI Text (TMP) or Text
    [SerializeField] private ElevatorController elevatorController;

    private void Start()
    {
        if (elevatorController == null)
            elevatorController = GetComponent<ElevatorController>();

        if (floorDisplay == null)
            Debug.LogError("Floor display not assigned!", this);

        // Subscribe to arrival event
        elevatorController.OnArrivedAtFloor += UpdateDisplay;
        // Initial update
        UpdateDisplay(elevatorController.CurrentFloor);
    }

    private void UpdateDisplay(int floor)
    {
        if (floorDisplay != null)
            floorDisplay.text = floor.ToString();
    }

    private void OnDestroy()
    {
        if (elevatorController != null)
            elevatorController.OnArrivedAtFloor -= UpdateDisplay;
    }
}