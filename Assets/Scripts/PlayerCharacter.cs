using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using TMPro;


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

    private GameManager gameManager;

    public float bulletCount = 30;
    public TextMeshProUGUI bulletCountText;

    [Tooltip("After the last bullet is fired, seconds to wait for it to land before the level is lost (so the last shot can still win).")]
    public float outOfAmmoDelay = 0.75f;

    private float startingBullets;
    private float outOfAmmoTimer = 0f;

    
    public int ShotsFired => Mathf.RoundToInt(startingBullets - bulletCount);


    private void Awake()
    {
        interactAction = inputActions.FindAction("Tap");
        positionAction = inputActions.FindAction("Position");

    }


    void Start()
    {
        // Starting health

        paperCollider = Paper.GetComponent<BoxCollider2D>();
        
        gameManager = FindAnyObjectByType<GameManager>();
        startingBullets = bulletCount;
        bulletCountText.text = "Bullets: " + bulletCount;

    }

    void Update()
    {
        
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

                bulletCount = bulletCount - 1;
                UpdateBulletCountText(bulletCount);

                RuntimeManager.PlayOneShot(FMODEvents.instance.singleShot, transform.position);
            }
        }



        // out of bullets: wait a moment so the last shot can land (and still win), then lose
        if (bulletCount <= 0 && win == false && lose == false)
        {
            outOfAmmoTimer += Time.deltaTime;
            if (outOfAmmoTimer >= outOfAmmoDelay && gameManager != null)
            {
                gameManager.TriggerLose();
            }
        }
    }


    private void UpdateBulletCountText(float bulletCount)
    {
        bulletCountText.text = "Bullets: " + bulletCount;
    }



}