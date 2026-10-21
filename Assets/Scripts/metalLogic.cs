using UnityEngine;

public class metalLogic : MonoBehaviour
{
    public int maxHealth = 3;
    public int health;
    public Collider2D targetCollider;
    public SpriteRenderer spriteRenderer;
    public Sprite[] metalSprites;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;

        targetCollider.enabled = false;

        //Sets starting sprite
        spriteRenderer.sprite = metalSprites[0];
    }


    public void TakeDamage(int damage)
    {
        if (health <= 0 || damage <= 0)
        {
            return;
        }

        health -= damage;

        if (health <= 0)
        {
            health = 0;
            targetCollider.enabled = true;
            Destroy(gameObject);
            return;
        }

        UpdateDamageSprite();


    }

    void UpdateDamageSprite()
    {
        if (health <= 0 || maxHealth <= 0 || metalSprites == null || metalSprites.Length == 0)
        {
            return;
        }

        int spriteIndex = (maxHealth - health) * metalSprites.Length / maxHealth;

        spriteIndex = Mathf.Clamp(spriteIndex, 0, metalSprites.Length - 1);

        spriteRenderer.sprite = metalSprites[spriteIndex];

    }
}

