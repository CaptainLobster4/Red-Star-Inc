using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : MonoBehaviour
{

    public GameObject BulletHole;
    private GameObject currentBullethole;
    private GameObject side1;

    [SerializeField] private InputActionAsset inputActions;
    private InputAction interactAction;
    private InputAction positionAction;

    private Vector2 tapPosition;

    private void Awake()
    {
        interactAction = inputActions.FindAction("Tap");
        positionAction = inputActions.FindAction("Position");

        side1 = GameObject.Find("Side1");

    }

    void Start()
    {
        // Starting health
    }

    void Update()
    {
        // Decrease health over time in bosss fights

        if (interactAction.triggered)
        {
            tapPosition = Camera.main.ScreenToWorldPoint(positionAction.ReadValue<Vector2>());

            GameObject currentBullethole = Instantiate(BulletHole, tapPosition, Quaternion.identity);


        }

    }
}
