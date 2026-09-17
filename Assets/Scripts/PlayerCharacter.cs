using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : MonoBehaviour
{

    public GameObject BulletHole;

    [SerializeField] private InputActionAsset inputActions;
    private InputAction interactAction;
    private InputAction positionAction;

    private Vector2 tapPosition;

    public bool win = false;


    private void Awake()
    {
        interactAction = inputActions.FindAction("Tap");
        positionAction = inputActions.FindAction("Position");



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

            if (win == false)
            {
                GameObject currentBullethole = Instantiate(BulletHole, tapPosition, Quaternion.identity);

            }

        }
    }
}
