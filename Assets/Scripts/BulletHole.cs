using UnityEngine;

public class BulletHole : MonoBehaviour
{
    [Header("Shape Progress")]
    [Tooltip("Progress-bar fill added per bullet that lands in the target area")]
    [SerializeField] private float baseProgress = 0.05f;

    [Header("Duckshot Ability (permanent upgrade, stronger per level)")]
    [Tooltip("How much bigger the bullet hole (and its collider) gets per Duckshot level. Level 3 with 0.3 = 1.9x size.")]
    [SerializeField] private float duckshotScalePerLevel = 0.3f;
    [Tooltip("Extra progress per bullet, per Duckshot level. Level 3 with 0.2 = 1.6x progress. A trigger only fires once per bullet, so without this a wider hole would count the same as a normal one.")]
    [SerializeField] private float duckshotProgressPerLevel = 0.2f;

    [Header("Double Shot Ability")]
    [Tooltip("Progress multiplier while Double Shot is equipped. A trigger only fires once per bullet (which in this case is two bullet holes), so multiply by two.")]
    [SerializeField] private float doubleshotProgressMultiplier = 2f;
    public GameObject DoubleShotBulletHole;
    private Vector2 doubleShotOffset;
    private GameObject doubleShotInstance;


    // area around the actual shape that bullets count in
    private GameObject targetArea;
    private GameManager gameManager;
    private float progressMultiplier = 1f;
    public metalLogic metalL;
    public bool notOnPaper = false;

    // checking for overlap
    private bool overlapping = false;
    private bool hitTarget = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Hit the shape: fill progress bar and count the bullet toward cutting out the shape
        if (collision.gameObject == targetArea)
        {
            if (gameManager == null) return;
            hitTarget = true;
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
            metalL.TakeDamage(1);
            Destroy(doubleShotInstance);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            Collider2D myCollider = GetComponent<Collider2D>();
            //Debug.Log("overlapping other bullet");

            // check if overlapping collider center is within this collider
            if (myCollider.bounds.Contains(collision.gameObject.transform.position))
            {
                overlapping = true;
                //Debug.Log("overlappinggg");
            }
        }



    }

    private void Awake()
    {
        // this was for the prototype but stuck, can later be changed to be set in inspector, as it's not always a triangle
        targetArea = GameObject.Find("TriangleArea");
        gameManager = FindAnyObjectByType<GameManager>();


        // Abilities are permanent upgrades the player can switch on/off (IsActive = owned AND switched on).
        float sizeScale = 1f;

        // Duckshot: scale the whole bullet hole up so it covers a wider area, more per level
        // (sprite + collider scale together, so it also pops balloons more easily)
        if (PlayerInventory.IsActive(AbilityType.Duckshot))
        {
            int level = PlayerInventory.GetLevel(AbilityType.Duckshot);
            sizeScale = 1f + duckshotScalePerLevel * level;
            transform.localScale *= sizeScale;
            progressMultiplier *= 1f + duckshotProgressPerLevel * level;
        }

        if (PlayerInventory.IsActive(AbilityType.DoubleShot) && DoubleShotBulletHole != null)
        {
            // doubleShotOffset is the offset that the second shot is from the first. currently, it is set to be just to the right, but it can be changed
            doubleShotOffset = new Vector2(0.3f, 0f);
            doubleShotInstance = Instantiate(DoubleShotBulletHole, (new Vector2(transform.position.x, transform.position.y)) + doubleShotOffset, Quaternion.identity);
            doubleShotInstance.transform.localScale *= sizeScale; // match Duckshot's size if both are on
            progressMultiplier *= doubleshotProgressMultiplier;   // stacks with Duckshot instead of replacing it
        }

    }

    private void Update()
    {
        // if not on paper (which is set from player character script), then set invisible (so the other logic still runs, but the bullet hole does not appear) 
        if (notOnPaper)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            Destroy(doubleShotInstance);
        }

        if (!overlapping && hitTarget)
        {
            gameManager.UpdateProgress(baseProgress * progressMultiplier);
            hitTarget = false;
        }

    }
}