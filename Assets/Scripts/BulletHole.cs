using UnityEngine;

public class BulletHole : MonoBehaviour
{
    [Header("Shape Progress")]
    [Tooltip("Progress-bar fill added per bullet that lands in the target area")]
    [SerializeField] private float baseProgress = 0.05f;

    [Header("Duckshot Ability")]
    [Tooltip("How much bigger the bullet hole (and its collider) is when Duckshot is equipped")]
    [SerializeField] private float duckshotScale = 2.5f;
    [Tooltip("Progress multiplier while Duckshot is equipped. A trigger only fires once per bullet, so without this a wider hole would count the same as a normal one.")]
    [SerializeField] private float duckshotProgressMultiplier = 2f;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
            Destroy(doubleShotInstance);
            Destroy(gameObject);
        }


    }

    private void Awake()
    {
        // this was for the prototype but stuck, can later be changed to be set in inspector, as it's not always a triangle
        targetArea = GameObject.Find("TriangleArea");
        gameManager = FindAnyObjectByType<GameManager>();


        // Duckshot (bought in the shop, lasts one round): scale the whole bullet hole up so it covers a wider area
        // (sprite + collider scale together, so it also pops balloons more easily)
        if (PlayerInventory.IsActive(AbilityType.Duckshot))
        {
            transform.localScale *= duckshotScale;
            progressMultiplier = duckshotProgressMultiplier;
        }

        if (PlayerInventory.IsActive(AbilityType.DoubleShot))
        {
                // doubleShotOffset is the offset that the second shot is from the first. currently, it is set to be just to the right, but it can be changed
                doubleShotOffset = new Vector2(0.3f, 0f);
                doubleShotInstance = Instantiate(DoubleShotBulletHole, (new Vector2(transform.position.x, transform.position.y)) + doubleShotOffset, Quaternion.identity);
                progressMultiplier = doubleshotProgressMultiplier;
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
    }
}
