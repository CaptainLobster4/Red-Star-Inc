using UnityEngine;

public class metalLogic : MonoBehaviour
{
    public int health = 3;
    public Collider2D targetCollider;
    public SpriteRenderer spriteRenderer;
    public Sprite standardMetal;
    public Sprite standardMetal1;
    public Sprite standardMetal2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetCollider.enabled = false;

        //Sets starting sprite
        spriteRenderer.sprite = standardMetal;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            targetCollider.enabled = true;
            Destroy(gameObject);
        }

        if (health == 2)
        {
            spriteRenderer.sprite = standardMetal1;
        }
        else if (health == 1)
        {
            spriteRenderer.sprite = standardMetal2;
        }

    }
}

