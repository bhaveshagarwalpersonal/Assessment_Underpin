using UnityEngine;
public class Floor : MonoBehaviour
{
    [SerializeField]
    private int floorNumber;

    [SerializeField]
    [Tooltip("How long the elevator waits (doors open) at this floor before continuing.")]
    private float waitTime = 2f;

    public int FloorNumber => floorNumber;
    public float WaitTime => waitTime;
}