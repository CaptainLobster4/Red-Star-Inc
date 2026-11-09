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

    [Tooltip("Filled in at the start of each level from the player's shared ammo (PlayerInventory). Whatever is typed here is ignored.")]
    public float bulletCount = 30;
    public TextMeshProUGUI bulletCountText;

    [Tooltip("After the last bullet is fired, seconds to wait for it to land before the level is lost (so the last shot can still win).")]
    public float outOfAmmoDelay = 0.75f;

    [Tooltip("Every level starts with at least this many bullets, even if the shared ammo is lower, so the player is never stuck with nothing. 0 = no safety net.")]
    public int minimumLevelBullets = 5;

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

        paperCollider = Paper != null ? Paper.GetComponent<BoxCollider2D>() : null;

        gameManager = FindAnyObjectByType<GameManager>();
        // One shared ammo pool: bullets carry over between levels and are bought in the shop
        int pool = PlayerInventory.Ammo;
        if (pool < minimumLevelBullets)
        {
            pool = minimumLevelBullets;
            PlayerInventory.SetAmmo(pool);
        }
        bulletCount = pool;
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
                // Once the paper is gone (destroyed, switched off, or its collider disabled) its bounds collapse to
                // nothing, so every tap would count as "off the paper" and the hole would be hidden.
                // With no paper there is nothing to be off of, so holes show wherever the player taps.
                bool paperPresent = paperCollider != null && paperCollider.enabled && paperCollider.gameObject.activeInHierarchy;

                if (!paperPresent || paperCollider.OverlapPoint(tapPosition))
                {
                    //Debug.Log("on paper");
                }
                else
                {
                    BulletHole bulletScript = currentBullethole.GetComponent<BulletHole>();
                    bulletScript.notOnPaper = true;
                }

                bulletCount = bulletCount - 1;
                PlayerInventory.SetAmmo(Mathf.RoundToInt(bulletCount)); // spend it from the saved pool
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