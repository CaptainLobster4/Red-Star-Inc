using UnityEngine;

public class BulletHole : MonoBehaviour
{
    [Header("Shape Progress")]
    [Tooltip("Progress-bar fill added per bullet that lands in the target area")]
    [SerializeField] private float baseProgress = 0.025f;

    [Header("Duckshot Ability")]
    [Tooltip("How much bigger the bullet hole (and its collider) is when Duckshot is equipped")]
    [SerializeField] private float duckshotScale = 2.5f;
    [Tooltip("Progress multiplier while Duckshot is equipped. A trigger only fires once per bullet, so without this a wider hole would count the same as a normal one.")]
    [SerializeField] private float duckshotProgressMultiplier = 2f;

    // area around the actual shape that bullets count in
    private GameObject targetArea;
    private GameManager gameManager;
    private float progressMultiplier = 1f;
    public metalLogic metalL;

    private void Awake()
    {
        targetArea = GameObject.Find("TriangleArea");
        gameManager = FindAnyObjectByType<GameManager>();

        // Duckshot (bought in the shop, lasts one round): scale the whole bullet hole up so it covers a wider area
        // (sprite + collider scale together, so it also pops balloons more easily)
        if (PlayerInventory.HasAbility(AbilityType.Duckshot))
        {
            transform.localScale *= duckshotScale;
            progressMultiplier = duckshotProgressMultiplier;
        }
    }

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
        else if (collision.TryGetComponent(out TicketBalloon balloon))
        {
            balloon.Pop();
        }

        if (collision.gameObject.tag == "MetalLayer")
        {
            metalL = collision.gameObject.GetComponent<metalLogic>();
            metalL.health = metalL.health - 1;
            Destroy(gameObject);
        }
    }
}