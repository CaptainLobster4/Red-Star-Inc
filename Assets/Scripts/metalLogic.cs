using UnityEngine;

public class metalLogic : MonoBehaviour
{
    public int health = 3;
    public Collider2D targetCollider;
    public SpriteRenderer spriteRenderer;
    public Sprite metalSpriteBase;
    public Sprite metalSpriteLightDMG;
    public Sprite metealSpriteHeavyDMG;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetCollider.enabled = false;

        //Sets starting sprite
        spriteRenderer.sprite = metalSpriteBase;
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
            spriteRenderer.sprite = metalSpriteLightDMG;
        }
        else if (health == 1)
        {
            spriteRenderer.sprite = metealSpriteHeavyDMG;
        }

    }
}
