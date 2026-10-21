using UnityEngine;

public class BulletHole : MonoBehaviour
{
    [Header("Shape Progress")]
    [Tooltip("Progress-bar fill added per bullet that lands in the target area")]
    [SerializeField] private float baseProgress = 0.025f;

    [Header("Duckshot Ability (permanent upgrade, stronger per level)")]
    [Tooltip("How much bigger the bullet hole (and its collider) gets per Duckshot level. Level 3 with 0.3 = 1.9x size.")]
    [SerializeField] private float duckshotScalePerLevel = 0.3f;
    [Tooltip("Extra progress per bullet, per Duckshot level. Level 3 with 0.2 = 1.6x progress. A trigger only fires once per bullet, so without this a wider hole would count the same as a normal one.")]
    [SerializeField] private float duckshotProgressPerLevel = 0.2f;

    // area around the actual shape that bullets count in
    private GameObject targetArea;
    private GameManager gameManager;
    private float progressMultiplier = 1f;
    private bool spent = false; // true if this shot had no bullet left to use
    public metalLogic metalL;

    private void Awake()
    {
        targetArea = GameObject.Find("TriangleArea");
        gameManager = FindAnyObjectByType<GameManager>();

        // Limited bullets: every shot uses one. If none are left, this shot does nothing.
        if (gameManager != null && !gameManager.TryUseBullet())
        {
            spent = true;
            Collider2D ownCollider = GetComponent<Collider2D>();
            if (ownCollider != null) ownCollider.enabled = false;
            Destroy(gameObject);
            return;
        }

        // Duckshot (permanent upgrade, switched on): scale the whole bullet hole up so it covers a wider area
        // (sprite + collider scale together, so it also pops balloons more easily)
        if (PlayerInventory.IsActive(AbilityType.Duckshot))
        {
            int level = PlayerInventory.GetLevel(AbilityType.Duckshot);
            transform.localScale *= 1f + duckshotScalePerLevel * level;
            progressMultiplier = 1f + duckshotProgressPerLevel * level;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (spent) return;

        // Hit the shape: fill progress bar and count the bullet toward cutting out the shape
        if (collision.gameObject == targetArea)
        {
            if (gameManager == null) return;

            gameManager.UpdateProgress(baseProgress * progressMultiplier);
            gameManager.shapeAmount = gameManager.DecreaseSideAmount(gameManager.shapeAmount);
        }
        // Hit a ticket balloon: it awards its own tickets and destroys itself
        else
        {
            // GetComponentInParent so it still works if the Collider2D is on a child of the balloon
            TicketBalloon balloon = collision.GetComponentInParent<TicketBalloon>();
            if (balloon != null)
            {
                balloon.Pop();
            }
        }

        if (collision.gameObject.tag == "MetalLayer")
        {
            metalL = collision.gameObject.GetComponent<metalLogic>();
            metalL.health = metalL.health - 1;
            Destroy(gameObject);
        }
    }
}