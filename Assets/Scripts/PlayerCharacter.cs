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

    // how long before level ends after fail
    public float outOfAmmoDelay = 0.75f;

    
    public int minimumLevelBullets = 5;

    private float startingBullets;
    private float outOfAmmoTimer = 0f;


    public int ShotsFired => Mathf.RoundToInt(startingBullets - bulletCount);


    // log for taps. did it tap, or did it not.
    [SerializeField] private bool logTaps = true;

    private bool loggedIgnoredTap = false;
    private bool loggedActionsOff = false;

    private void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("PlayerCharacter: Input Actions isn't assigned on " + name + " in scene '" + gameObject.scene.name + "', so shooting can't work.", this);
            return;
        }

        interactAction = inputActions.FindAction("Tap");
        positionAction = inputActions.FindAction("Position");
    }

    private void OnEnable()
    {
        EnableInput();
    }

    private void EnableInput()
    {
        if (interactAction != null && !interactAction.enabled) interactAction.Enable();
        if (positionAction != null && !positionAction.enabled) positionAction.Enable();
    }


    void Start()
    {
        // Starting health

        paperCollider = Paper != null ? Paper.GetComponent<BoxCollider2D>() : null;

        gameManager = FindAnyObjectByType<GameManager>();
        
        int pool = PlayerInventory.Ammo;
        if (pool < minimumLevelBullets)
        {
            pool = minimumLevelBullets;
            PlayerInventory.SetAmmo(pool);
        }
        bulletCount = pool;
        startingBullets = bulletCount;
        UpdateBulletCountText(bulletCount);

    }

    void Update()
    {
        if (interactAction == null || positionAction == null) return;

        if (!interactAction.enabled || !positionAction.enabled)
        {
            if (!loggedActionsOff)
            {
                loggedActionsOff = true;
                Debug.LogWarning("PlayerCharacter: the Tap/Position input actions were switched off by something else. Turning them back on.", this);
            }
            EnableInput();
        }

        if (interactAction.triggered)
        {
            if (Camera.main == null)
            {
                Debug.LogError("PlayerCharacter: no camera tagged MainCamera in scene '" + gameObject.scene.name + "', so taps can't be placed.", this);
                return;
            }

            tapPosition = Camera.main.ScreenToWorldPoint(positionAction.ReadValue<Vector2>());

            if ((win || lose) && !loggedIgnoredTap)
            {
                loggedIgnoredTap = true;
                Debug.LogWarning("PlayerCharacter: tap ignored because win=" + win + " lose=" + lose + " (level is over). Ammo left: " + Mathf.RoundToInt(bulletCount), this);
            }

            // no bullets left: no more shots while the out-of-ammo timer runs down
            if (win == false && lose == false && bulletCount >= 1)
            {
                GameObject currentBullethole = Instantiate(BulletHole, tapPosition, Quaternion.identity);
                // if paper collider is gone, bullet holes stop appearing. 
                bool paperPresent = paperCollider != null && paperCollider.enabled && paperCollider.gameObject.activeInHierarchy;

                if (!paperPresent || paperCollider.OverlapPoint(tapPosition))
                {
                    //prints on paper for each successful shot
                }
                else
                {
                    BulletHole bulletScript = currentBullethole.GetComponent<BulletHole>();
                    bulletScript.notOnPaper = true;
                }

                if (logTaps)
                {
                    string paperInfo = Paper == null ? "Paper slot EMPTY" : (paperPresent ? (paperCollider.OverlapPoint(tapPosition) ? "on paper" : "OFF paper (hole hidden)") : "paper collider off/missing (holes shown)");
                    Debug.Log("Tap in '" + gameObject.scene.name + "': shot fired, ammo before=" + Mathf.RoundToInt(bulletCount) + ", " + paperInfo, this);
                }

                bulletCount = bulletCount - 1;
                PlayerInventory.SetAmmo(Mathf.RoundToInt(bulletCount)); 
                UpdateBulletCountText(bulletCount);

                if (FMODEvents.instance != null)
                {
                    RuntimeManager.PlayOneShot(FMODEvents.instance.singleShot, transform.position);
                }
            }
            else if (logTaps && win == false && lose == false)
            {
                Debug.LogWarning("Tap in '" + gameObject.scene.name + "': NO SHOT because bulletCount=" + bulletCount + " (shared ammo=" + PlayerInventory.Ammo + ").", this);
            }
        }



        
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
        if (bulletCountText == null) return;

        int displayedBulletCount = Mathf.Max(0, Mathf.RoundToInt(bulletCount));
        bulletCountText.text = "Ammo-" + displayedBulletCount.ToString("D2");
    }



}