using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;

public class PlayerCharacter : MonoBehaviour
{

    public GameObject BulletHole;
    public GameObject Paper;
    private BoxCollider2D paperCollider;

    [SerializeField] private InputActionAsset inputActions;
    private InputAction interactAction;
    private InputAction positionAction;

    private Vector2 tapPosition;

    public bool win = false;
    public bool lose = false;


    private void Awake()
    {
        interactAction = inputActions.FindAction("Tap");
        positionAction = inputActions.FindAction("Position");

    }


    void Start()
    {
        // Starting health

        paperCollider = Paper.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        // Decrease health over time in bosss fights
        if (interactAction.triggered)
        {
            tapPosition = Camera.main.ScreenToWorldPoint(positionAction.ReadValue<Vector2>());

            if (win == false && lose == false)
            {
                GameObject currentBullethole = Instantiate(BulletHole, tapPosition, Quaternion.identity);
                if (paperCollider.bounds.Contains(tapPosition))
                {
                    //Debug.Log("on paper");
                }
                else
                {
                    BulletHole bulletScript = currentBullethole.GetComponent<BulletHole>();
                    bulletScript.notOnPaper = true;
                }

                    RuntimeManager.PlayOneShot(FMODEvents.instance.singleShot, transform.position);
            }
        }
    }

}