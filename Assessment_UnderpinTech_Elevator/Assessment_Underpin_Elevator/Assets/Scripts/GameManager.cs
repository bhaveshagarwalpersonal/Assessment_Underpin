using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private ElevatorDispatcher dispatcher;

    private void Awake()
    {
        Application.targetFrameRate = 120;
    }
}